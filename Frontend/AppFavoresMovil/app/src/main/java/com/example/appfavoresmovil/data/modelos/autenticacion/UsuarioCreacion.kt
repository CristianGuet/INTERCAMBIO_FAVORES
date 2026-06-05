package com.example.appfavoresmovil.data.modelos.autenticacion

import com.google.gson.annotations.SerializedName

data class UsuarioCreacion(
    @SerializedName("correo") val correo: String,
    @SerializedName("contrasenia") val contrasenia: String,
    @SerializedName("nombreUsuario") val nombreUsuario: String
)
