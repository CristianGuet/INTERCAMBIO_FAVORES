package com.example.appfavoresmovil.data.modelos.notificaciones

import com.google.gson.annotations.SerializedName

data class Notificacion(
    @SerializedName("_id") val id: String,
    @SerializedName("tipo") val tipo: String,
    @SerializedName("mensaje") val mensaje: String,
    @SerializedName("leida") val leida: Boolean,
    @SerializedName("fechaCreacion") val fechaCreacion: String
)
