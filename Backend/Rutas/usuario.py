from fastapi import APIRouter, Depends, HTTPException, status, BackgroundTasks
from Utilidades.correo import enviar_correo_notificacion
from Utilidades.seguridad import obtener_usuario_actual
from Modelos.usuario import UsuarioCreacion, UsuarioPublico, Valoracion
from Configuracion.bbdd import coleccion_usuarios
from Utilidades.formateador import Formateador
from passlib.context import CryptContext
from bson import ObjectId
from datetime import datetime, timedelta
from jose import jwt
import os
from fastapi.security import OAuth2PasswordRequestForm

router = APIRouter(prefix="/usuarios", tags=["Usuarios"])

# Configuracion para encriptar contraseñas
contrasenia_contexto = CryptContext(schemes=["bcrypt"], deprecated="auto")

@router.post("/registro", response_model=UsuarioPublico, status_code=status.HTTP_201_CREATED)
async def registrar_usuario(usuario: UsuarioCreacion, background_tasks: BackgroundTasks):

    # Quitar minusculas y espacios el correo
    correo_a_comprobar = usuario.correo.lower().strip()

    # Buscar si ya existe alguien con ese correo
    usuario_existente = await coleccion_usuarios.find_one({"correo": correo_a_comprobar})

    if usuario_existente:
        raise HTTPException(status_code=400, detail="Ya existe un usuario registrado con este correo electronico")

    nuevo_usuario = usuario.model_dump()
    nuevo_usuario["correo"] = correo_a_comprobar
    nuevo_usuario["rol"] = "Usuario"
    nuevo_usuario["contrasenia"] = contrasenia_contexto.hash(usuario.contrasenia)
    
    # Insertar en la base de datos
    resultado = await coleccion_usuarios.insert_one(nuevo_usuario)
    
    # Recuperar el usuario
    usuario_creado = await coleccion_usuarios.find_one({"_id": resultado.inserted_id})

    # Convertir el ObjectId
    if usuario_creado:
        # Convertir el objeto _id de MongoDB a un texto normal
        usuario_creado["_id"] = str(usuario_creado["_id"])
        usuario_creado["id"] = usuario_creado["_id"]
    
    background_tasks.add_task(
        enviar_correo_notificacion,
        usuario_destino=correo_a_comprobar,
        asunto="¡Bienvenido a FavorApp!",
        contenido_texto=f"Hola {usuario.nombreUsuario}, gracias por registrarte en Favsy. ¡Esperamos que disfrutes la experiencia!"
    )

    return usuario_creado

CLAVE = os.getenv("JWT_CLAVE")
ALGORITMO = os.getenv("JWT_ALGORITMO")
TIEMPO_ESPIRACION = int(os.getenv("TOKEN_TIEMPO_ESPIRACION"))

@router.post("/login")
async def login(datos: OAuth2PasswordRequestForm = Depends()): 
    correo = Formateador.conversor_minusculas(datos.username)
    contrasenia = datos.password

    # Busca al usuario
    usuario_db = await coleccion_usuarios.find_one({"correo": correo})
    if not usuario_db:
        raise HTTPException(status_code=400, detail="Credenciales incorrectas")

    # Verificar la contraseña (compara la contraseña sin encriptar con la encriptada)
    es_valida = contrasenia_contexto.verify(contrasenia, usuario_db["contrasenia"])
    if not es_valida:
        raise HTTPException(status_code=400, detail="Credenciales incorrectas")
    
    # Crea el token JWT
    tiempo_expiracion = datetime.utcnow() + timedelta(minutes=TIEMPO_ESPIRACION)
    
    # Mete la info del usuario en el token (payload)
    info_token = {
        "sub": usuario_db["correo"], #Para seber de quien es el token
        "id": str(usuario_db["_id"]),
        "exp": tiempo_expiracion
    }
    
    token_jwt = jwt.encode(info_token, CLAVE, ALGORITMO)

    # Devuelve el token y los datos basicos del usuario
    return {
        "access_token": token_jwt,
        "token_type": "bearer",
        "usuario": {
            "nombre": usuario_db["nombreUsuario"],
            "correo": usuario_db["correo"],
            "id": str(usuario_db["_id"]),
            "rol": usuario_db.get("rol", "Usuario") 
        }
    }

# Calificar usuario
@router.post("/{correo_usuario}/calificar")
async def calificar_usuario(correo_usuario: str, valoracion: Valoracion, background_tasks: BackgroundTasks, _auth = Depends(obtener_usuario_actual)):
    # Limpiar el correo
    correo_destino = correo_usuario.lower().strip()
    
    usuario = await coleccion_usuarios.find_one({"correo": correo_destino})
    if not usuario:
        raise HTTPException(status_code=404, detail="Usuario no encontrado")
        
    # Guarda la nueva nota en una lista oculta llamada "historialNotas" usando $push
    await coleccion_usuarios.update_one(
        {"correo": correo_destino},
        {"$push": {"historialNotas": valoracion.puntuacion}}
    )
    
    # Recalcula la media
    usuario_actualizado = await coleccion_usuarios.find_one({"correo": correo_destino})
    notas = usuario_actualizado.get("historialNotas", [])
    
    if notas:
            media = round(sum(notas) / len(notas),1)
    else:
            media = None
        
    # Guarda la calificacionMedia calculada
    await coleccion_usuarios.update_one(
        {"correo": correo_destino},
        {"$set": {"calificacionMedia": media}}
    )

    background_tasks.add_task(
        enviar_correo_notificacion,
        usuario_destino=correo_destino,
        asunto="¡Tienes una nueva calificación!",
        contenido_texto=f"Hola, {correo_usuario} te ha valorado con una valoración de {valoracion.puntuacion} estrellas. Tu nueva media es de {media}."
    )
    
    return {
        "mensaje": "Usuario calificado con éxito", 
        "nuevaMedia": media
    }