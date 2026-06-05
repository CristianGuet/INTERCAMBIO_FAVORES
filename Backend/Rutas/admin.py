from fastapi import APIRouter, Depends, HTTPException
from bson import ObjectId
from Configuracion.bbdd import coleccion_usuarios, coleccion_favores, coleccion_grupos
from Utilidades.seguridad import obtener_usuario_admin

router = APIRouter(prefix="/admin", tags=["Administración"])

# --- Usuarios ---
@router.get("/usuarios")
async def listar_usuarios(admin: str = Depends(obtener_usuario_admin)):
    usuarios = await coleccion_usuarios.find().to_list(100)
    for u in usuarios:
        u["_id"] = str(u["_id"])
    return usuarios

@router.delete("/usuarios/{correo}")
async def eliminar_usuario(correo: str, admin: str = Depends(obtener_usuario_admin)):
    if correo == admin:
        raise HTTPException(status_code=400, detail="No puedes eliminar tu propia cuenta")
    resultado = await coleccion_usuarios.delete_one({"correo": correo})
    if resultado.deleted_count == 0:
        raise HTTPException(status_code=404, detail="Usuario no encontrado")
    return {"mensaje": f"Usuario {correo} eliminado"}

@router.patch("/usuarios/{correo}/rol")
async def cambiar_rol(correo: str, nuevo_rol: str, admin: str = Depends(obtener_usuario_admin)):
    if nuevo_rol not in ["Usuario", "Admin"]:
        raise HTTPException(status_code=400, detail="Rol no válido")
    if correo == admin and nuevo_rol != "Admin":
        raise HTTPException(status_code=400, detail="No puedes degradarte a ti mismo")
    resultado = await coleccion_usuarios.update_one({"correo": correo}, {"$set": {"rol": nuevo_rol}})
    if resultado.modified_count == 0:
        raise HTTPException(status_code=404, detail="Usuario no encontrado")
    return {"mensaje": f"Usuario {correo} ahora tiene rol {nuevo_rol}"}

# --- Favores ---
@router.get("/favores")
async def listar_favores(admin: str = Depends(obtener_usuario_admin)):
    favores = await coleccion_favores.find().to_list(100)
    for f in favores:
        f["_id"] = str(f["_id"])
    return favores

@router.delete("/favores/{id_favor}")
async def eliminar_favor_admin(id_favor: str, admin: str = Depends(obtener_usuario_admin)):
    if not ObjectId.is_valid(id_favor):
        raise HTTPException(status_code=400, detail="ID inválido")
    resultado = await coleccion_favores.delete_one({"_id": ObjectId(id_favor)})
    if resultado.deleted_count == 0:
        raise HTTPException(status_code=404, detail="Favor no encontrado")
    return {"mensaje": "Favor eliminado por administrador"}

# --- Grupos ---
@router.get("/grupos")
async def listar_grupos(admin: str = Depends(obtener_usuario_admin)):
    grupos = await coleccion_grupos.find().to_list(100)
    for g in grupos:
        g["_id"] = str(g["_id"])
    return grupos

@router.delete("/grupos/{id_grupo}")
async def eliminar_grupo_admin(id_grupo: str, admin: str = Depends(obtener_usuario_admin)):
    if not ObjectId.is_valid(id_grupo):
        raise HTTPException(status_code=400, detail="ID inválido")
    resultado = await coleccion_grupos.delete_one({"_id": ObjectId(id_grupo)})
    if resultado.deleted_count == 0:
        raise HTTPException(status_code=404, detail="Grupo no encontrado")
    return {"mensaje": "Grupo eliminado por administrador"}