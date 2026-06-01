from fastapi import APIRouter, Depends
from Configuracion.bbdd import coleccion_notificaciones
from Utilidades.seguridad import obtener_usuario_actual

router = APIRouter(prefix="/notificaciones", tags=["Notificaciones"])

@router.get("/mis-notificaciones")
async def obtener_notificaciones(usuario_logueado: str = Depends(obtener_usuario_actual)):
    # Busca las notificaciones del usuario logueado, ordenadas por las más recientes
    lista = await coleccion_notificaciones.find({"usuarioDestino": usuario_logueado}).sort("fecha", -1).to_list(20)
    
    for n in lista:
        n["_id"] = str(n["_id"])
    return lista

@router.post("/marcar-leida")
async def marcar_como_leida(usuario_logueado: str = Depends(obtener_usuario_actual)):
    # Marca todas las notificaciones del usuario como leidas
    await coleccion_notificaciones.update_many(
        {"usuarioDestino": usuario_logueado, "leida": False},
        {"$set": {"leida": True}}
    )
    return {"mensaje": "Notificaciones marcadas como leidas"}