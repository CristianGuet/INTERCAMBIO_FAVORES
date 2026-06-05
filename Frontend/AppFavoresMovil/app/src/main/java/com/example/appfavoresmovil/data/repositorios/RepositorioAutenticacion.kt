package com.example.appfavoresmovil.data.repositorios

import android.content.Context
import com.example.appfavoresmovil.data.modelos.autenticacion.RespuestaLogin
import com.example.appfavoresmovil.data.modelos.autenticacion.UsuarioCreacion
import com.example.appfavoresmovil.data.modelos.autenticacion.UsuarioPublico
import com.example.appfavoresmovil.data.servicios.ClienteRetrofit
import com.example.appfavoresmovil.utilidades.PreferenciasSesion
import com.example.appfavoresmovil.utilidades.ResultadoApi
import retrofit2.HttpException
import java.io.IOException

class RepositorioAutenticacion(private val context: Context) {

    private val api = ClienteRetrofit.api

    suspend fun login(correo: String, contrasena: String): ResultadoApi<RespuestaLogin> {
        return ejecutar {
            val respuesta = api.login(username = correo, password = contrasena)
            val usuario = respuesta.usuario
            if (usuario != null) {
                PreferenciasSesion.guardarSesion(
                    context,
                    respuesta.accessToken,
                    usuario.correo,
                    usuario.nombre,
                    usuario.rol
                )
            } else {
                PreferenciasSesion.guardarSesion(context, respuesta.accessToken, "", "", "Usuario")
            }
            respuesta
        }
    }

    suspend fun registro(usuario: UsuarioCreacion): ResultadoApi<UsuarioPublico> {
        return ejecutar { api.registro(usuario) }
    }

    fun cerrarSesion() {
        PreferenciasSesion.limpiarSesion(context)
    }

    private suspend fun <T> ejecutar(bloque: suspend () -> T): ResultadoApi<T> {
        return try {
            ResultadoApi.Exito(bloque())
        } catch (e: HttpException) {
            val codigo = e.code()
            val mensaje = when (codigo) {
                401 -> "Usuario o contraseña incorrectos. Por favor, inténtalo de nuevo."
                else -> e.message ?: "Error $codigo"
            }
            ResultadoApi.Error(mensaje)
        } catch (e: IOException) {
            ResultadoApi.Error("Sin conexión con el servidor")
        } catch (e: Exception) {
            ResultadoApi.Error(e.message ?: "Error desconocido")
        }
    }
}
