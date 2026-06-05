package com.example.appfavoresmovil.data.modelos.chats

import com.google.gson.annotations.SerializedName

data class ChatPublico(
    @SerializedName("_id") val id: String,
    @SerializedName("tipo") val tipo: String,
    @SerializedName("participantes") val participantes: List<String>,
    @SerializedName("mensajes") val mensajes: List<MensajePublico>,
    @SerializedName("ultimaActividad") val ultimaActividad: String,
    var nombreGrupo: String = ""
)
