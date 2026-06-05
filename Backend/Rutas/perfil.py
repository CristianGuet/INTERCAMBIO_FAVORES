from fastapi import APIRouter, Depends, HTTPException
from Modelos.usuario import UsuarioPublico, UsuarioEdicion
from Configuracion.bbdd import coleccion_usuarios
from Utilidades.seguridad import obtener_usuario_actual
from bson import ObjectId

router = APIRouter(prefix="/perfil", tags=["Perfiles"])

# Obtener los propios datos
@router.get("/mis-datos", response_model=UsuarioPublico)
async def obtener_mi_perfil(correo_usuario: str = Depends(obtener_usuario_actual)):
    usuario = await coleccion_usuarios.find_one({"correo": correo_usuario})
    if not usuario:
        raise HTTPException(status_code=404, detail="Usuario no encontrado")
    usuario["_id"] = str(usuario["_id"])
    usuario["fotoPerfil"] = usuario.get("fotoPerfil", "default.png")
    return usuario

# Editar los propios datos
@router.put("/editar", response_model=UsuarioPublico)
async def editar_perfil(datos_nuevos: UsuarioEdicion, correo_usuario: str = Depends(obtener_usuario_actual)):
    # 1. Extraemos los campos enviados en el JSON, ignorando los no enviados y los None explícitos
    campos_limpios = datos_nuevos.model_dump(exclude_unset=True, exclude_none=True)

    # Evitar guardar strings vacíos "", espacios "   " o el texto de prueba "string"
    campos_a_actualizar = {}
    for k, v in campos_limpios.items():
        if isinstance(v, str):
            texto_limpio = v.strip()
            # Si al limpiar espacios no queda vacío y no es la palabra por defecto "string", se actualiza
            if texto_limpio != "" and texto_limpio.lower() != "string":
                campos_a_actualizar[k] = texto_limpio
        else:
            # Si el campo no es un String (por ejemplo, números o booleanos en un futuro), pasa directo
            campos_a_actualizar[k] = v

    # Si después del filtro no quedan campos válidos para actualizar, lanzamos error
    if not campos_a_actualizar:
        raise HTTPException(status_code=400, detail="No se enviaron datos válidos o con contenido para actualizar")

    # Actualizar en MongoDB usando el operador $set
    await coleccion_usuarios.update_one(
        {"correo": correo_usuario},
        {"$set": campos_a_actualizar}
    )

    # Buscar el usuario actualizado para devolverlo a la app
    usuario_actualizado = await coleccion_usuarios.find_one({"correo": correo_usuario})
    
    # Tambien convertimos el _id a string
    usuario_actualizado["_id"] = str(usuario_actualizado["_id"])
    return usuario_actualizado