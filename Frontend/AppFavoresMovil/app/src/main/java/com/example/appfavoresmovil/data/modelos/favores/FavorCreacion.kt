package com.example.appfavoresmovil.data.modelos.favores

import com.google.gson.annotations.SerializedName

data class FavorCreacion(
    @SerializedName("titulo") val titulo: String,
    @SerializedName("descripcion") val descripcion: String,
    @SerializedName("modalidad") val modalidad: String,
    @SerializedName("ubicacion") val ubicacion: String?,
    @SerializedName("tipoCompensacion") val tipoCompensacion: String,
    @SerializedName("cantidadDinero") val cantidadDinero: Float?,
    @SerializedName("recompensaFavor") val recompensaFavor: String?
)
