package com.example.appfavoresmovil.data.modelos.grupos

import com.google.gson.annotations.SerializedName

data class GrupoBusqueda(
    @SerializedName("id") val id: String,
    @SerializedName("nombreGrupo") val nombreGrupo: String,
    @SerializedName("descripcion") val descripcion: String,
    @SerializedName("fotoGrupo") val fotoGrupo: String,
    @SerializedName("numeroMiembros") val numeroMiembros: Int
)
