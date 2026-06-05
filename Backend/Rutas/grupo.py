from fastapi import APIRouter, Depends, Query, status, HTTPException, BackgroundTasks
from Utilidades.formateador import Formateador
from Modelos.grupo import GrupoCreacion, GrupoEdicion, GrupoPublico, MiembroGrupo, PedirInvitacionEntrada
from Configuracion.bbdd import coleccion_grupos, coleccion_usuarios, coleccion_notificaciones
from Utilidades.seguridad import obtener_usuario_actual
from Utilidades.correo import enviar_correo_notificacion

from typing import List, Literal
from bson import ObjectId

router = APIRouter(prefix="/grupos", tags=["Grupos"])


@router.get("/buscar-grupos")
async def buscar_grupos(nombre: str = Query(..., min_length=3)):
    termino = nombre.strip()
    
    # Busca grupos cuyo nombre coincida
    query = {"nombreGrupo": {"$regex": termino, "$options": "i"}}
    grupos = await coleccion_grupos.find(query).to_list(100)
    
    # Formatea la respuesta para no exponer datos privados
    resultado = []
    for g in grupos:
        resultado.append({
            "id": str(g["_id"]),
            "nombreGrupo": g["nombreGrupo"],
            "descripcion": g["descripcion"],
            "fotoGrupo": g.get("fotoGrupo", "group_default.png"),
            "numeroMiembros": len(g.get("miembros", []))
        })
        
    return resultado

@router.post("/crear", response_model=GrupoPublico, status_code=status.HTTP_201_CREATED)
async def crear_grupo(grupo: GrupoCreacion, usuario_logueado: str = Depends(obtener_usuario_actual)):
    # Preparar los datos del grupo
    nuevo_grupo = grupo.model_dump()
    
    # El creador siempre es el "admin"
    # Crea el objeto MiembroGrupo para el creador
    creador = MiembroGrupo(correoUsuario=usuario_logueado, rol="admin")
    
    # Sobreescribe la lista de miembros para asegurar que el creador este ahi con su rol
    nuevo_grupo["miembros"] = [creador.model_dump()]

    nuevo_grupo["fechaCreacion"] = Formateador.obtener_fecha_actual()
    
    # Guarda en la base de datos
    resultado = await coleccion_grupos.insert_one(nuevo_grupo)
    
    # Devolve el grupo recien creado
    grupo_final = await coleccion_grupos.find_one({"_id": resultado.inserted_id})
    
    if grupo_final:
        grupo_final["_id"] = str(grupo_final["_id"])
    
    return grupo_final

@router.get("/mis-grupos", response_model=List[GrupoPublico])
async def obtener_mis_grupos(usuario_logueado: str = Depends(obtener_usuario_actual)):
    # Busca grupos donde el ID del usuario este dentro de la lista de miembros
    # MongoDB usa la notacion "campo.subcampo" para buscar dentro de objetos
    query = {"miembros.correoUsuario": usuario_logueado}
    lista_grupos = await coleccion_grupos.find(query).to_list(50)

    for grupo in lista_grupos:
        grupo["_id"] = str(grupo["_id"])

    return lista_grupos

# unirse a un grupo
@router.post("/{id_grupo}/unirse")
async def unirse_grupo(id_grupo: str, usuario_logueado: str = Depends(obtener_usuario_actual)):
    grupo = await coleccion_grupos.find_one({"_id": ObjectId(id_grupo)})
    
    if not grupo:
        raise HTTPException(status_code=404, detail="Grupo no encontrado")

    # Comprobar si el usuario ya esta en la lista de miembros
    es_miembro = any(miembro["correoUsuario"] == usuario_logueado for miembro in grupo["miembros"])
    if es_miembro:
        raise HTTPException(status_code=400, detail="Ya eres miembro de este grupo")

    # Preparar el objeto del nuevo miembro
    nuevo_miembro = MiembroGrupo(correoUsuario=usuario_logueado, rol="miembro")
    
    # $push empuja al nuevo usuario dentro de la lista "miembros" del grupo
    await coleccion_grupos.update_one(
        {"_id": ObjectId(id_grupo)},
        {"$push": {"miembros": nuevo_miembro.model_dump()}}
    )
    
    return {"mensaje": "Te has unido al grupo correctamente"}

@router.post("/{id_grupo}/salir")
async def salir_de_grupo(id_grupo: str, usuario_logueado: str = Depends(obtener_usuario_actual)):
    # Verificar si el grupo existe
    grupo = await coleccion_grupos.find_one({"_id": ObjectId(id_grupo)})
    if not grupo:
        raise HTTPException(status_code=404, detail="Grupo no encontrado")

    # Verificar si el usuario esta en el grupo
    miembro_actual = next((m for m in grupo["miembros"] if m["correoUsuario"] == usuario_logueado), None)
    if not miembro_actual:
        raise HTTPException(status_code=400, detail="No perteneces a este grupo")
    
    # Es el último miembro?
    if len(grupo["miembros"]) == 1:
        # Si solo queda un usuario,se borra el grupo
        await coleccion_grupos.delete_one({"_id": ObjectId(id_grupo)})
        return {"mensaje": "Has salido del grupo y se ha eliminado el grupo correctemente"}

    # Si es admin el unico admin, no puede dejar el grupo sin ningun admin
    if miembro_actual["rol"] == "admin":
        administradores = [m for m in grupo["miembros"] if m["rol"] == "admin"]
        # Si es el unico admin pero aun hay más miembros se le obliga a nombrar a otro admin antes de salir
        if len(administradores) == 1 and len(grupo["miembros"]) > 1:
            raise HTTPException(
                status_code=400, 
                detail="Eres el único administrador. Debes nombrar a otro administrador antes de salir."
            )

    # Sacar al usuario de la lista de miembros
    await coleccion_grupos.update_one(
        {"_id": ObjectId(id_grupo)},
        {"$pull": {"miembros": {"correoUsuario": usuario_logueado}}}
    )

    return {"mensaje": "Has salido del grupo correctamente"}

# unirse a un grupo por invitacion de correo
@router.post("/{id_grupo}/invitar-por-correo")
async def invitar_usuario_email(id_grupo: str, correo_invitado: str,background_tasks: BackgroundTasks, usuario_logueado: str = Depends(obtener_usuario_actual)):
    # Limpiar el correo de espacios y mayusculas
    correo_destinatario = correo_invitado.lower().strip()

    # Verificar que el usuario invitado existe en la base de datos
    usuario_destino = await coleccion_usuarios.find_one({"correo": correo_destinatario})
    if not usuario_destino:
        raise HTTPException(status_code=404, detail="No existe ningún usuario con ese correo")

    # Verificar que el grupo existe
    grupo = await coleccion_grupos.find_one({"_id": ObjectId(id_grupo)})
    if not grupo:
        raise HTTPException(status_code=404, detail="Grupo no encontrado")

    # Comprobar si ya es miembro para no duplicarlo
    es_miembro = any(m["correoUsuario"] == correo_destinatario for m in grupo["miembros"])
    if es_miembro:
        raise HTTPException(status_code=400, detail="El usuario ya esta en este grupo")

    # Añadir al nuevo miembro usando su correo como idUsuario
    nuevo_miembro = MiembroGrupo(correoUsuario=correo_destinatario, rol="miembro")
    
    await coleccion_grupos.update_one(
        {"_id": ObjectId(id_grupo)},
        {"$push": {"miembros": nuevo_miembro.model_dump()}}
    )

    nueva_notificacion = {
        "idUsuario": correo_destinatario,
        "mensaje": f"Has sido añadido al grupo '{grupo['nombreGrupo']}' por {usuario_logueado}",
        "tipo": "invitacion_grupo",
        "idReferencia": id_grupo,
        "leida": False,
        "fecha": Formateador.obtener_fecha_actual()
    }

    await coleccion_notificaciones.insert_one(nueva_notificacion)

    background_tasks.add_task(
        enviar_correo_notificacion,
        usuario_destino=correo_destinatario,
        asunto="¡Te han añadido a un grupo!",
        contenido_texto=nueva_notificacion
    )

    return {"mensaje": f"Usuario {correo_destinatario} añadido correctemente"}

@router.post("/{id_grupo}/pedir-invitacion")
async def pedir_invitacion_grupo(id_grupo: str, entrada: PedirInvitacionEntrada, background_tasks: BackgroundTasks, usuario_logueado: str = Depends(obtener_usuario_actual)):
    grupo = await coleccion_grupos.find_one({"_id": ObjectId(id_grupo)})
    if not grupo:
        raise HTTPException(status_code=404, detail="Grupo no encontrado")

    # Validar si ya es miembro
    es_miembro = any(m["correoUsuario"] == usuario_logueado for m in grupo.get("miembros", []))
    if es_miembro:
        raise HTTPException(status_code=400, detail="Ya eres miembro de este grupo")

    # Validar si ya solicito unirse antes
    if any(s["correoUsuario"] == usuario_logueado for s in grupo.get("solicitudes", [])):
        raise HTTPException(status_code=400, detail="Ya has enviado una solicitud a este grupo")
    
    nueva_solicitud = {
        "correoUsuario": usuario_logueado,
        "comentario": entrada.comentario,
        "fechaSolicitud": Formateador.obtener_fecha_actual()
    }

    # Guardar el correo en la lista de solicitudes pendientes
    await coleccion_grupos.update_one(
        {"_id": ObjectId(id_grupo)},
        {"$push": {"solicitudes": nueva_solicitud}}
    )

    admins = [m["correoUsuario"] for m in grupo.get("miembros", []) if m["rol"] == "admin"]
    for admin in admins:
        background_tasks.add_task(
            enviar_correo_notificacion,
            usuario_destino=admin,
            asunto="Nueva solicitud de acceso a tu grupo",
            contenido_texto=f"El usuario {usuario_logueado} quiere unirse a tu grupo '{grupo['nombreGrupo']}'. Entra a FavorApp para aceptar o rechazar la solicitud."
        )

    return {"mensaje": "Solicitud enviada correctamente."}

@router.post("/{id_grupo}/aceptar-solicitud")
async def aceptar_solicitud_grupo(id_grupo: str, correo_aspirante: str, background_tasks: BackgroundTasks, usuario_logueado: str = Depends(obtener_usuario_actual)):
    correo_aspirante = correo_aspirante.lower().strip()

    # Solo el admin puede gestionar solicitudes
    grupo = await coleccion_grupos.find_one({
        "_id": ObjectId(id_grupo),
        "miembros": {"$elemMatch": {"correoUsuario": usuario_logueado, "rol": "admin"}}
    })
    
    if not grupo:
        raise HTTPException(status_code=403, detail="No tienes permisos de administrador para este grupo o no existe")

    # Verificar si la solicitud existe dentro de la lista
    solicitud_existe = any(s["correoUsuario"] == correo_aspirante for s in grupo.get("solicitudes", []))
    if not solicitud_existe:
        raise HTTPException(status_code=404, detail="La solicitud no existe o ya fue procesada")

    nuevo_miembro = MiembroGrupo(correoUsuario=correo_aspirante, rol="miembro")

    # Añadir a miembros y eliminar la solicitud
    await coleccion_grupos.update_one(
        {"_id": ObjectId(id_grupo)},
        {
            "$push": {"miembros": nuevo_miembro.model_dump()},
            "$pull": {"solicitudes": {"correoUsuario": correo_aspirante}}
        }
    )

    # Notificar al usuario aceptado
    mensaje_notificacion = f"Has sido aceptado en el grupo '{grupo['nombreGrupo']}'"
    await coleccion_notificaciones.insert_one({
        "usuarioDestino": correo_aspirante,
        "mensaje": mensaje_notificacion,
        "tipo": "solicitud_aceptada",
        "idReferencia": id_grupo,
        "leida": False,
        "fecha": Formateador.obtener_fecha_actual()
    })

    background_tasks.add_task(
        enviar_correo_notificacion,
        usuario_destino=correo_aspirante,
        asunto="Solicitud Aceptada",
        contenido_texto=mensaje_notificacion
    )   

    return {"mensaje": f"Usuario {correo_aspirante} ha sido aceptado."}

@router.post("/{id_grupo}/rechazar-solicitud")
async def rechazar_solicitud_grupo(id_grupo: str, correo_aspirante: str, background_tasks: BackgroundTasks, usuario_logueado: str = Depends(obtener_usuario_actual)):
    correo_aspirante = correo_aspirante.lower().strip()

    # Comprobar si quien rechaza es admin
    grupo = await coleccion_grupos.find_one({
        "_id": ObjectId(id_grupo),
        "miembros": {"$elemMatch": {"correoUsuario": usuario_logueado, "rol": "admin"}}
    })
    
    if not grupo:
        raise HTTPException(status_code=403, detail="No tienes permisos de administrador para rechazar miembros")

    # Verificar si la solicitud existe
    solicitud_existe = any(s["correoUsuario"] == correo_aspirante for s in grupo.get("solicitudes", []))
    if not solicitud_existe:
        raise HTTPException(status_code=404, detail="La solicitud no existe o ya fue procesada")

    # Eliminar la solicitud de la base de datos tras rechazarla
    await coleccion_grupos.update_one(
        {"_id": ObjectId(id_grupo)},
        {"$pull": {"solicitudes": {"correoUsuario": correo_aspirante}}}
    )

    mensaje_notificacion = f"Tu solicitud para unirte al grupo '{grupo['nombreGrupo']}' ha sido rechazada"
    
    await coleccion_notificaciones.insert_one({
        "usuarioDestino": correo_aspirante,
        "mensaje": mensaje_notificacion ,
        "tipo": "solicitud_rechazada",
        "idReferencia": id_grupo,
        "leida": False,
        "fecha": Formateador.obtener_fecha_actual()
    })

    background_tasks.add_task(
        enviar_correo_notificacion,
        usuario_destino=correo_aspirante,
        asunto="Solicitud Rechazada",
        contenido_texto=mensaje_notificacion
    )

    return {"mensaje": f"Solicitud de {correo_aspirante} fue rechazada"}

@router.get("/buscar-usuario")
async def buscar_usuario(busqueda: str = Query(..., min_length=3)):
    termino = busqueda.lower().strip()
    
    query = {
        "$or": [
            {"nombreUsuario": {"$regex": termino, "$options": "i"}},
            {"correo": {"$regex": termino, "$options": "i"}}
        ]
    }
    usuarios = await coleccion_usuarios.find(query).to_list(100)
    
    # Devolve solo lo necesario para que el admin lo reconozca
    return [
        {
            "nombre": u["nombreUsuario"], 
            "correo": u["correo"], 
            "foto": u.get("fotoPerfil", "default.png")
        } for u in usuarios
    ]

@router.post("/{id_grupo}/expulsar")
async def expulsar_miembro(id_grupo: str, usuario_a_expulsar: str, background_tasks: BackgroundTasks, usuario_logueado: str = Depends(obtener_usuario_actual)):
    # Verificar que el que ejecuta es admin del grupo
    grupo = await coleccion_grupos.find_one({
        "_id": ObjectId(id_grupo),
        "miembros": {"$elemMatch": {"correoUsuario": usuario_logueado, "rol": "admin"}} # Se tiene que cumplir que sea admin y que sea el usurio_logeado
    })
    
    if not grupo:
        raise HTTPException(status_code=403, detail="Solo los administradores pueden expulsar miembros")

    # No puedes expulsarte a ti mismo
    if usuario_a_expulsar == usuario_logueado:
        raise HTTPException(status_code=400, detail="No puedes expulsarte a ti mismo. Debes salir del grupo")

    # Eliminar al miembro
    resultado = await coleccion_grupos.update_one(
        {"_id": ObjectId(id_grupo)},
        {"$pull": {"miembros": {"correoUsuario": usuario_a_expulsar}}}
    )

    if resultado.modified_count == 0:
        raise HTTPException(status_code=404, detail="El usuario no pertenece a este grupo")

    mensaje_notificacion = f"Has sido expulsado del grupo '{grupo['nombreGrupo']}'"
    await coleccion_notificaciones.insert_one({
        "usuarioDestino": usuario_a_expulsar,
        "mensaje": mensaje_notificacion,
        "tipo": "expulsion_grupo",
        "fecha": Formateador.obtener_fecha_actual(),
        "leida": False
    })

    background_tasks.add_task(
        enviar_correo_notificacion,
        usuario_destino=usuario_a_expulsar,
        asunto="Te han eliminado de un grupo",
        contenido_texto=mensaje_notificacion
    )

    return {"mensaje": f"Usuario {usuario_a_expulsar} expulsado correctamente"}

@router.patch("/{id_grupo}/cambiar-rol")
async def cambiar_rol_miembro(id_grupo: str, correo_miembro: str, background_tasks: BackgroundTasks, nuevo_rol: Literal["admin", "miembro"],
                              usuario_logueado: str = Depends(obtener_usuario_actual)):
    # Verificar que el usuario logueado es admin
    grupo = await coleccion_grupos.find_one({
        "_id": ObjectId(id_grupo),
        "miembros": {"$elemMatch": {"correoUsuario": usuario_logueado, "rol": "admin"}}
    })
    
    if not grupo:
        raise HTTPException(status_code=403, detail="No tienes permisos de administrador")

    # Actualizar el rol en el array
    resultado = await coleccion_grupos.update_one(
        {"_id": ObjectId(id_grupo), "miembros.correoUsuario": correo_miembro},
        {"$set": {"miembros.$.rol": nuevo_rol}}
    )

    if resultado.modified_count == 0:
        raise HTTPException(status_code=404, detail="Usuario no encontrado en el grupo")

    mensaje_notificacion = f"Tu rol en el grupo '{grupo['nombreGrupo']}' ha sido cambiado por {usuario_logueado}: ahora eres {nuevo_rol}"
    await coleccion_notificaciones.insert_one({
        "usuarioDestino": correo_miembro,
        "mensaje": mensaje_notificacion,
        "tipo": "cambio_rol",
        "fecha": Formateador.obtener_fecha_actual(),
        "leida": False
    })

    background_tasks.add_task(
        enviar_correo_notificacion,
        usuario_destino=correo_miembro,
        asunto="Han cambiado tu rol en el grupo",
        contenido_texto=mensaje_notificacion
    )

    return {"mensaje": f"Rol de {correo_miembro} actualizado a {nuevo_rol}"}

@router.patch("/{id_grupo}/editar")
async def editar_grupo(id_grupo: str, datos_actualizar : GrupoEdicion, background_tasks: BackgroundTasks, usuario_logueado: str = Depends(obtener_usuario_actual)):

    # Verificar que el usuario es admin
    grupo_previo = await coleccion_grupos.find_one({
        "_id": ObjectId(id_grupo),
        "miembros": {"$elemMatch": {"correoUsuario": usuario_logueado, "rol": "admin"}}
    })
    
    if not grupo_previo:
        raise HTTPException(status_code=403, detail="No tienes permiso para editar este grupo")

    # Procesar cambios 
    campos_limpios = datos_actualizar.model_dump(exclude_unset=True, exclude_none=True)
    campos = {k: v for k, v in campos_limpios.items() if v.strip() != "" and v != "string"}

    if not campos:
        raise HTTPException(status_code=400, detail="No hay campos válidos para actualizar")

    await coleccion_grupos.update_one({"_id": ObjectId(id_grupo)}, {"$set": campos})

    # Notificar a los miembros del grupo
    if "nombreGrupo" in campos:
        nuevo_nombre = campos["nombreGrupo"]
        nombre_antiguo = grupo_previo["nombreGrupo"]
        
        notificaciones = []
        for miembro in grupo_previo["miembros"]:
            id_miembro = miembro["correoUsuario"]
            if id_miembro != usuario_logueado:
                mensaje_noti = f"El administrador ha cambiado el nombre del grupo '{nombre_antiguo}' a '{nuevo_nombre}'"
                notificaciones.append({
                    "usuarioDestino": id_miembro,
                    "mensaje": mensaje_noti,
                    "tipo": "cambio_nombre_grupo",
                    "idReferencia": id_grupo,
                    "leida": False,
                    "fecha": Formateador.obtener_fecha_actual()
                })

                background_tasks.add_task(
                    enviar_correo_notificacion,
                    usuario_destino=id_miembro,
                    asunto="Un grupo ha cambiado de nombre",
                    contenido_texto=mensaje_noti
                )
        
        if notificaciones:
            await coleccion_notificaciones.insert_many(notificaciones)

    return {"mensaje": "Grupo actualizado y miembros notificados"}
