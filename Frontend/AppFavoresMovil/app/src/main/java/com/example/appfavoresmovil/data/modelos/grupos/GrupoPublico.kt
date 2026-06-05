package com.example.appfavoresmovil.data.modelos.grupos

import com.google.gson.annotations.SerializedName

data class GrupoPublico(
    @SerializedName("_id") val id: String,
    @SerializedName("nombreGrupo") val nombreGrupo: String,
    @SerializedName("descripcion") val descripcion: String,
    @SerializedName("fotoGrupo") val fotoGrupo: String,
    @SerializedName("miembros") val miembros: List<MiembroGrupo>,
    @SerializedName("solicitudes") val solicitudes: List<SolicitudGrupo>,
    @SerializedName("fechaCreacion") val fechaCreacion: String
)

data class MiembroGrupo(
    @SerializedName("correoUsuario") val correoUsuario: String,
    @SerializedName("rol") val rol: String
)

data class SolicitudGrupo(
    @SerializedName("correoUsuario") val correoUsuario: String,
    @SerializedName("comentario") val comentario: String?,
    @SerializedName("fechaSolicitud") val fechaSolicitud: String
)
