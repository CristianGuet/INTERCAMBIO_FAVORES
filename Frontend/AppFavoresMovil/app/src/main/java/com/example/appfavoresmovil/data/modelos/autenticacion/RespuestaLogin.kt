package com.example.appfavoresmovil.data.modelos.autenticacion

import com.google.gson.annotations.SerializedName

data class RespuestaLogin(
    @SerializedName("access_token") val accessToken: String,
    @SerializedName("token_type") val tokenType: String,
    @SerializedName("usuario") val usuario: UsuarioResumen?
)

data class UsuarioResumen(
    @SerializedName("nombre") val nombre: String,
    @SerializedName("correo") val correo: String,
    @SerializedName("id") val id: String,
    @SerializedName("rol") val rol: String
)
