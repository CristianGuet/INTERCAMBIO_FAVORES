package com.example.appfavoresmovil.data.modelos.favores

import com.google.gson.annotations.SerializedName

data class FavorPublico(
    @SerializedName("_id") val id: String,
    @SerializedName("titulo") val titulo: String,
    @SerializedName("descripcion") val descripcion: String,
    @SerializedName("modalidad") val modalidad: String,
    @SerializedName("tipoCompensacion") val tipoCompensacion: String,
    @SerializedName("cantidadDinero") val cantidadDinero: Float?,
    @SerializedName("recompensaFavor") val recompensaFavor: String?,
    @SerializedName("ubicacion") val ubicacion: String?,
    @SerializedName("estado") val estado: String,
    @SerializedName("fechaCreacion") val fechaCreacion: String,
    @SerializedName("idUsuarioOfrece") val idUsuarioOfrece: String,
    @SerializedName("idUsuarioSolicita") val idUsuarioSolicita: String?
)
