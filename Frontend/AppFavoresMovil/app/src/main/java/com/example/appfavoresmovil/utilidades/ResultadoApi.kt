package com.example.appfavoresmovil.utilidades

sealed class ResultadoApi<out T> {
    data class Exito<T>(val datos: T) : ResultadoApi<T>()
    data class Error(val mensaje: String) : ResultadoApi<Nothing>()
    object Cargando : ResultadoApi<Nothing>()
}
