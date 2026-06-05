package com.example.appfavoresmovil.data.repositorios

import com.example.appfavoresmovil.data.modelos.chats.ChatPublico
import com.example.appfavoresmovil.data.modelos.chats.MensajePublico
import com.example.appfavoresmovil.data.servicios.ClienteRetrofit
import com.example.appfavoresmovil.data.servicios.ServicioApi
import com.example.appfavoresmovil.utilidades.ResultadoApi
import retrofit2.HttpException
import java.io.IOException

class RepositorioChat {

    private val api = ClienteRetrofit.api

    suspend fun obtenerMisChats(): ResultadoApi<List<ChatPublico>> = ejecutar {
        api.getMisChats()
    }

    suspend fun obtenerOCrear(participantes: List<String>): ResultadoApi<ChatPublico> = ejecutar {
        api.obtenerOCrearChat(mapOf("participantes" to participantes))
    }

    suspend fun obtenerPorId(id: String): ResultadoApi<ChatPublico> = ejecutar {
        val chats = api.getMisChats()
        chats.find { it.id == id }
            ?: throw IllegalStateException("Chat no encontrado")
    }

    suspend fun enviarMensaje(idChat: String, contenido: String): ResultadoApi<MensajePublico> = ejecutar {
        api.enviarMensaje(idChat, ServicioApi.EnviarMensajeRequest(contenido))
    }

    suspend fun marcarLeido(idChat: String): ResultadoApi<Map<String, String>> = ejecutar {
        api.marcarLeido(idChat)
    }

    private suspend fun <T> ejecutar(bloque: suspend () -> T): ResultadoApi<T> {
        return try {
            ResultadoApi.Exito(bloque())
        } catch (e: HttpException) {
            val mensaje = try {
                val errorBody = e.response()?.errorBody()?.string()
                val json = org.json.JSONObject(errorBody ?: "")
                json.optString("detail", e.message ?: "Error ${e.code()}")
            } catch (_: Exception) {
                e.message ?: "Error ${e.code()}"
            }
            ResultadoApi.Error(mensaje)
        } catch (e: IOException) {
            ResultadoApi.Error("Sin conexión con el servidor")
        } catch (e: Exception) {
            ResultadoApi.Error(e.message ?: "Error desconocido")
        }
    }
}
