package com.example.appfavoresmovil.data.modelos.chats

import com.google.gson.annotations.SerializedName

data class MensajePublico(
    @SerializedName("emisor") val emisor: String,
    @SerializedName("contenido") val contenido: String,
    @SerializedName("fechaEnvio") val fechaEnvio: String,
    @SerializedName("leido") val leido: Boolean
)
