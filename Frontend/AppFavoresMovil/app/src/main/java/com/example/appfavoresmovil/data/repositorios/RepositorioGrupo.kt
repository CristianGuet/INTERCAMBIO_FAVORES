package com.example.appfavoresmovil.data.repositorios

import com.example.appfavoresmovil.data.modelos.grupos.GrupoBusqueda
import com.example.appfavoresmovil.data.modelos.grupos.GrupoPublico
import com.example.appfavoresmovil.data.servicios.ClienteRetrofit
import com.example.appfavoresmovil.utilidades.ResultadoApi
import retrofit2.HttpException
import java.io.IOException

class RepositorioGrupo {

    private val api = ClienteRetrofit.api

    suspend fun obtenerMisGrupos(): ResultadoApi<List<GrupoPublico>> = ejecutar {
        api.getMisGrupos()
    }

    suspend fun buscar(nombre: String): ResultadoApi<List<GrupoBusqueda>> = ejecutar {
        api.buscarGrupos(nombre)
    }

    suspend fun crear(nombre: String, descripcion: String?): ResultadoApi<GrupoPublico> = ejecutar {
        api.crearGrupo(
            mapOf(
                "nombreGrupo" to nombre,
                "descripcion" to descripcion
            )
        )
    }

    suspend fun unirse(id: String): ResultadoApi<Map<String, String>> = ejecutar {
        api.unirseGrupo(id)
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
