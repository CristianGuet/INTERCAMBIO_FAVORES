package com.example.appfavoresmovil.singleton

object SesionApp {
    var token: String = ""
    var correo: String = ""
    var nombre: String = ""
    var rol: String = "Usuario"

    val estaLogueado get() = token.isNotBlank()

    fun limpiar() {
        token = ""
        correo = ""
        nombre = ""
        rol = "Usuario"
    }
}
