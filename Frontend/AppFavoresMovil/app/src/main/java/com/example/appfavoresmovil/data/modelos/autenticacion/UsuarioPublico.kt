package com.example.appfavoresmovil.data.modelos.autenticacion

import com.google.gson.annotations.SerializedName

data class UsuarioPublico(
    @SerializedName("_id") val id: String,
    @SerializedName("correo") val correo: String,
    @SerializedName("nombreUsuario") val nombreUsuario: String,
    @SerializedName("descripcion") val descripcion: String,
    @SerializedName("fotoPerfil") val fotoPerfil: String,
    @SerializedName("calificacionMedia") val calificacionMedia: Float?,
    @SerializedName("rol") val rol: String
)
