package com.example.appfavoresmovil.data.repositorios

import com.example.appfavoresmovil.data.modelos.favores.FavorCreacion
import com.example.appfavoresmovil.data.modelos.favores.FavorPublico
import com.example.appfavoresmovil.data.servicios.ClienteRetrofit
import com.example.appfavoresmovil.utilidades.ResultadoApi
import retrofit2.HttpException
import java.io.IOException

class RepositorioFavor {

    private val api = ClienteRetrofit.api

    suspend fun obtenerDisponibles(): ResultadoApi<List<FavorPublico>> = ejecutar {
        api.getFavoresDisponibles()
    }

    suspend fun crear(favor: FavorCreacion): ResultadoApi<FavorPublico> = ejecutar {
        api.crearFavor(favor)
    }

    suspend fun aceptar(id: String): ResultadoApi<Map<String, String>> = ejecutar {
        api.aceptarFavor(id)
    }

    private suspend fun <T> ejecutar(bloque: suspend () -> T): ResultadoApi<T> {
        return try {
            ResultadoApi.Exito(bloque())
        } catch (e: HttpException) {
            ResultadoApi.Error(e.message ?: "Error ${e.code()}")
        } catch (e: IOException) {
            ResultadoApi.Error("Sin conexión con el servidor")
        } catch (e: Exception) {
            ResultadoApi.Error(e.message ?: "Error desconocido")
        }
    }
}
