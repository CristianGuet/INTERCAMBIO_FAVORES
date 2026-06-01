from fastapi import APIRouter, BackgroundTasks, Depends, HTTPException, Query
from Utilidades.formateador import Formateador
from Modelos.favor import FavorCreacion, FavorPublico
from Modelos.notificacion import Notificacion
from Configuracion.bbdd import coleccion_favores, coleccion_notificaciones
from Utilidades.seguridad import obtener_usuario_actual
from Utilidades.correo import enviar_correo_notificacion
from typing import Optional
from bson import ObjectId

router = APIRouter(prefix="/favores", tags=["Favores"])

@router.post("/crear", response_model=FavorPublico, response_model_exclude_none=True)
async def crear_favor(favor: FavorCreacion, usuario_actual: str = Depends(obtener_usuario_actual)):
    # Forzar que el "idUsuarioOfrece" sea el del token por seguridad
    nuevo_favor = favor.model_dump(exclude_none=True) # "exclude_none=True" hace que los campos que esten vacíos (None) no pasen al diccionario

    # Rellena los campos automaticos
    nuevo_favor["idUsuarioOfrece"] = usuario_actual
    nuevo_favor["estado"] = "disponible"
    nuevo_favor["fechaCreacion"] = Formateador.obtener_fecha_actual()
    nuevo_favor["idUsuarioSolicita"] = None
    
    # Insertar en base de datos
    resultado = await coleccion_favores.insert_one(nuevo_favor)
    
    # Devolver respuesta
    favor_en_db = await coleccion_favores.find_one({"_id": resultado.inserted_id})
    favor_en_db["_id"] = str(favor_en_db["_id"])
    return favor_en_db

@router.get("/lista")
async def obtener_favores(  
    estado: Optional[str] = Query(None), 
    modalidad: Optional[str] = Query(None),
    usuario_logueado: str = Depends(obtener_usuario_actual)
):
    query = {}

    # Publico para todos
    if modalidad:
        query["modalidad"] = modalidad

    # Privado para solo el usuario logeado
    if estado == "disponible" or estado is None:
        query["estado"] = "disponible"
    else:
        # solo muestra los favores donde el sea el creador o el ayudante
        query["estado"] = estado
        query["$or"] = [
            {"idUsuarioOfrece": usuario_logueado},
            {"idUsuarioSolicita": usuario_logueado}
        ]

    favores = await coleccion_favores.find(query).to_list(100)

    for favor in favores:
        favor["_id"] = str(favor["_id"])

    return favores

# Aceptar un favor
@router.put("/{id_favor}/aceptar")
async def aceptar_favor(id_favor: str, background_tasks: BackgroundTasks, usuario_logueado: str = Depends(obtener_usuario_actual)):
    favor = await coleccion_favores.find_one({"_id": ObjectId(id_favor)})
    
    if not favor:
        raise HTTPException(status_code=404, detail="Favor no encontrado")
    if favor["estado"] != "disponible":
        raise HTTPException(status_code=400, detail="Este favor ya ha sido aceptado o finalizado")
    if favor["idUsuarioOfrece"] == usuario_logueado:
        raise HTTPException(status_code=400, detail="No puedes aceptar tu propio favor")

    # Actualizar el estado y registrar quien lo ha aceptado
    await coleccion_favores.update_one(
        {"_id": ObjectId(id_favor)},
        {"$set": {"estado": "aceptado", "idUsuarioSolicita": usuario_logueado}}
    )

    nueva_notificacion = Notificacion(
        usuarioDestino=favor["idUsuarioOfrece"],
        mensaje=f"¡Buenas noticias! {usuario_logueado} ha aceptado tu favor: {favor.get('titulo', 'Sin título')}",
        tipo="exito",
        idReferencia=id_favor
    )

    await coleccion_notificaciones.insert_one(nueva_notificacion.model_dump())

    background_tasks.add_task(
        enviar_correo_notificacion,
        usuario_destino=favor["idUsuarioOfrece"],
        asunto="¡Han aceptado tu favor!",
        contenido_texto=f"¡Buenas noticias! El usuario {usuario_logueado} ha aceptado tu favor '{favor.get('titulo')}'. Abre la aplicación para ir al chat."
    )

    return {"mensaje": "Favor aceptado correctamente"}

@router.put("/{id_favor}/cancelar")
async def cancelar_favor(id_favor: str, background_tasks: BackgroundTasks, usuario_logueado: str = Depends(obtener_usuario_actual)):
    favor = await coleccion_favores.find_one({"_id": ObjectId(id_favor)})
    
    if not favor or favor["estado"] != "aceptado":
        raise HTTPException(status_code=400, detail="Solo se pueden cancelar favores en curso")

    # Cambia estado a cancelado
    await coleccion_favores.update_one(
        {"_id": ObjectId(id_favor)},
        {"$set": {"estado": "cancelado"}}
    )

    correo_del_otro = favor["idUsuarioOfrece"] if usuario_logueado == favor["idUsuarioSolicita"] else favor["idUsuarioSolicita"]
    
    nueva_notificacion = Notificacion(
        usuarioDestino=correo_del_otro,
        mensaje=f"El usuario {usuario_logueado} ha cancelado el favor: {favor.get('titulo', 'Sin título')}",
        tipo="alerta",
        idReferencia=id_favor
    )

    await coleccion_notificaciones.insert_one(nueva_notificacion.model_dump())
    
    background_tasks.add_task(
        enviar_correo_notificacion,
        usuario_destino=correo_del_otro,
        asunto="Favor Cancelado",
        contenido_texto=f"Lamentamos informarte que el usuario {usuario_logueado} ha cancelado el favor '{favor.get('titulo')}'."
    )
    
    return {"mensaje": "Favor cancelado y notificación enviada"}

# Finalizar un favor
@router.put("/{id_favor}/finalizar")
async def finalizar_favor(id_favor: str, 
    background_tasks: BackgroundTasks, # Inyeccion de tareas en segundo plano
    usuario_logueado: str = Depends(obtener_usuario_actual)
):
    # Buscar el favor en la base de datos
    favor = await coleccion_favores.find_one({"_id": ObjectId(id_favor)})
    
    if not favor:
        raise HTTPException(status_code=404, detail="El favor no existe")
        
    # Verificar que el usuario que intenta finalizar sea parte de este favor
    if usuario_logueado != favor["idUsuarioOfrece"] and usuario_logueado != favor.get("idUsuarioSolicita"):
        raise HTTPException(status_code=403, detail="No tienes permiso para interactuar con este favor")

    # Solo permite finalizar si el favor estaba "aceptado" previamente
    resultado = await coleccion_favores.update_one(
        {"_id": ObjectId(id_favor), "estado": "aceptado"},
        {"$set": {"estado": "finalizado"}}
    )
    
    # Si la base de datos se modifico con exito, saltan las notificaciones
    if resultado.modified_count > 0:
        # Decidir a quién enviar la notificación (al participante que NO pulsó el botón)
        destinatario = favor["idUsuarioOfrece"] if usuario_logueado == favor.get("idUsuarioSolicita") else favor["idUsuarioSolicita"]
        
        if destinatario:
            mensaje_alerta = f"El favor '{favor.get('titulo')}' ha sido marcado como finalizado."
            asunto_correo = "¡Actualización de tu Favor!"
            
            # Guardar la notificación interna para la campana de la App
            notificacion = Notificacion(
                usuarioDestino=destinatario,
                mensaje=mensaje_alerta,
                tipo="info",
                idReferencia=id_favor
            )
            await coleccion_notificaciones.insert_one(notificacion.model_dump())
            
            # Enviar correo en segundo plano
            # FastAPI responderá al usuario de inmediato y mandara el email de forma asíncrona por detrás
            background_tasks.add_task(
                enviar_correo_notificacion,
                usuario_destino=destinatario,
                asunto=asunto_correo,
                contenido_texto=mensaje_alerta
            )
    else:
        # Si modified_count es 0, significa que el favor ya estaba finalizado, cancelado o disponible
        raise HTTPException(
            status_code=400, 
            detail="No se pudo finalizar. El favor debe estar en estado 'aceptado'"
        )
    
    return {"mensaje": "Favor marcado como finalizado con éxito"}

# Eliminar un favor
@router.delete("/{id_favor}/eliminar")
async def eliminar_favor(id_favor: str, usuario_logueado: str = Depends(obtener_usuario_actual)):
    # Buscar el favor
    favor = await coleccion_favores.find_one({"_id": ObjectId(id_favor)})
    
    if not favor:
        raise HTTPException(status_code=404, detail="El favor no existe")
        
    # Solo el dueño puede borrarlo
    if favor["idUsuarioOfrece"] != usuario_logueado:
        raise HTTPException(status_code=403, detail="No tienes permiso para borrar este favor")
        
    # Impedide el borrado si está "aceptado"
    if favor["estado"] == "aceptado":
        raise HTTPException(status_code=400, detail="No puedes borrar un favor que está en curso. Cancélalo primero antes de eliminarlo.")

    # Si el estado es disponible, cancelado o finalizado, se borra
    await coleccion_favores.delete_one({"_id": ObjectId(id_favor)})
    
    return {"mensaje": "Favor eliminado correctamente"}
