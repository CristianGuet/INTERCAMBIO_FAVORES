from fastapi import Depends, HTTPException
from fastapi.security import OAuth2PasswordBearer
from jose import jwt, JWTError
import os

oauth2_esquema = OAuth2PasswordBearer(tokenUrl="usuarios/login")

async def obtener_usuario_actual(token: str = Depends(oauth2_esquema)): 
    try:
        payload = jwt.decode(
            token, 
            os.getenv("JWT_CLAVE"), 
            os.getenv("JWT_ALGORITMO")
        )
        correo: str = payload.get("sub")
        if correo is None:
            raise HTTPException(status_code=401, detail="Token invalido")
        return correo # Devolve el correo del que esta logueado
    
    except JWTError:
        raise HTTPException(status_code=401, detail="No se pudo validar la sesión")
    
# Nueva función para obtener el rol de un usuario
async def obtener_rol_usuario(correo: str):
    from Configuracion.bbdd import coleccion_usuarios
    usuario = await coleccion_usuarios.find_one({"correo": correo})
    if usuario:
        return usuario.get("rol", "Usuario")
    return None

# Dependencia para verificar que el usuario es administrador
async def obtener_usuario_admin(token: str = Depends(oauth2_esquema)):
    correo = await obtener_usuario_actual(token)
    rol = await obtener_rol_usuario(correo)
    if rol != "Admin":
        raise HTTPException(status_code=403, detail="Se requieren permisos de administrador")
    return correo