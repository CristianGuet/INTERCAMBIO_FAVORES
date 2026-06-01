from fastapi import APIRouter, BackgroundTasks, Depends, HTTPException
from Modelos.chat import Mensaje, ChatPublico, MensajeEntrada, ChatCreacionRequest
from Utilidades.correo import enviar_correo_notificacion
from Configuracion.bbdd import coleccion_chats
from Utilidades.seguridad import obtener_usuario_actual
from Utilidades.formateador import Formateador
from typing import List
from bson import ObjectId

router = APIRouter(prefix="/chat", tags=["Chats"])

@router.post("/obtener-o-crear_chat", response_model=ChatPublico)
async def obtener_o_crear_chat(datos: ChatCreacionRequest, usuario_logueado: str = Depends(obtener_usuario_actual)):
    # Forzar minúsculas inmediatamente al usuario del Token JWT 
    usuario_normalizado = usuario_logueado.lower().strip()

    # Convertir también a minúsculas todos los correos enviados desde el frontend
    participantes = [p.lower().strip() for p in datos.participantes]

    # Asegurar que el usuario que ejecuta la accion este dentro del chat
    if usuario_normalizado not in participantes:
        participantes.append(usuario_normalizado)

    # Limpiar duplicados locales y ordenar alfabeticamente de forma limpia
    participantes_ordenados = sorted(list(set(participantes)))

    # Determinar si la peticion es para un chat privado
    es_chat_privado = len(participantes_ordenados) == 2
    tipo_chat = "privado" if es_chat_privado else "grupo"

    # si el chat es privado aplica tu logica de id unico determinista
    if es_chat_privado:
        id_chat_privado = "_".join(participantes_ordenados)
        
        # Comprobar si ya existe usando el _id personalizado de texto
        chat_existente = await coleccion_chats.find_one({"_id": id_chat_privado})
        if chat_existente:
            chat_existente["_id"] = str(chat_existente["_id"])
            return chat_existente

        # Si no existe, lo creamos forzando ese _id unico
        nuevo_chat = {
            "_id": id_chat_privado,
            "tipo": "privado",
            "participantes": participantes_ordenados,
            "mensajes": [],
            "ultimaActividad": Formateador.obtener_fecha_actual()
        }
        await coleccion_chats.insert_one(nuevo_chat)
        return nuevo_chat

    # Si es un chat grupol se busca si ya existe un grupo con exactamente esos mismos participantes
    else:
        chat_existente = await coleccion_chats.find_one({
            "tipo": "grupo",
            "participantes": participantes_ordenados  # Al estar guardados siempre ordenados, coincide exactamente
        })
        if chat_existente:
            chat_existente["_id"] = str(chat_existente["_id"])
            return chat_existente

        # Si no existe, deja que MongoDB genere un ObjectId automatico
        nuevo_chat = {
            "tipo": "grupo",
            "participantes": participantes_ordenados,
            "mensajes": [],
            "ultimaActividad": Formateador.obtener_fecha_actual()
        }
        
        resultado = await coleccion_chats.insert_one(nuevo_chat)
        nuevo_chat["_id"] = str(resultado.inserted_id)
        return nuevo_chat

@router.post("/{id_chat}/enviar", response_model=Mensaje)
async def enviar_mensaje(id_chat: str, entrada: MensajeEntrada,background_tasks: BackgroundTasks, usuario_logueado: str = Depends(obtener_usuario_actual)):
    usuario_normalizado = usuario_logueado.lower().strip()

    # Si es un ObjectId válido (chat grupal) lo convierte, si no, usa el String (chat privado)
    id_filtro = ObjectId(id_chat) if ObjectId.is_valid(id_chat) else id_chat

    # El usuario solo puede enviar si pertenece a este chat
    chat = await coleccion_chats.find_one({"_id": id_filtro, "participantes": usuario_normalizado})
    if not chat:
        raise HTTPException(status_code=403, detail="No tienes permiso para interactuar en este chat o no existe")

    nuevo_mensaje = Mensaje(
        emisor=usuario_normalizado,
        contenido=entrada.contenido
    )

    await coleccion_chats.update_one(
        {"_id": id_filtro},
        {
            "$push": {"mensajes": nuevo_mensaje.model_dump()},
            "$set": {"ultimaActividad": Formateador.obtener_fecha_actual()}
        }
    )

    for participante in chat["participantes"]:
        if participante != usuario_normalizado:
            background_tasks.add_task(
                enviar_correo_notificacion,
                usuario_destino=participante,
                asunto="Nuevo mensaje en FavorApp",
                contenido_texto=f"Has recibido un nuevo mensaje de {usuario_normalizado}:\n\n\"{entrada.contenido}\""
            )

    return nuevo_mensaje

@router.put("/{id_chat}/leer-chats")
async def marcar_como_leido(id_chat: str, usuario_logueado: str = Depends(obtener_usuario_actual)):
    usuario_normalizado = usuario_logueado.lower().strip()

    # Soporta tanto ObjectId de grupos como el String de privados
    id_filtro = ObjectId(id_chat) if ObjectId.is_valid(id_chat) else id_chat

    chat = await coleccion_chats.find_one({"_id": id_filtro, "participantes": usuario_normalizado})
    if not chat:
        raise HTTPException(status_code=403, detail="No tienes acceso a este chat")

    await coleccion_chats.update_one(
        {"_id": id_filtro},
        {"$set": {"mensajes.$[elem].leido": True}},
        array_filters=[{"elem.emisor": {"$ne": usuario_normalizado}}]
    )
    
    return {"mensaje": "Se han marcado los mensajes como leídos"}