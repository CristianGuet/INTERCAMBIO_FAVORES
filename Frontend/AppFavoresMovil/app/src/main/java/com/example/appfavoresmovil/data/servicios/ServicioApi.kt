package com.example.appfavoresmovil.data.servicios

import com.example.appfavoresmovil.data.modelos.autenticacion.RespuestaLogin
import com.example.appfavoresmovil.data.modelos.autenticacion.UsuarioCreacion
import com.example.appfavoresmovil.data.modelos.autenticacion.UsuarioPublico
import com.example.appfavoresmovil.data.modelos.chats.ChatPublico
import com.example.appfavoresmovil.data.modelos.chats.MensajePublico
import com.example.appfavoresmovil.data.modelos.favores.FavorCreacion
import com.example.appfavoresmovil.data.modelos.favores.FavorPublico
import com.example.appfavoresmovil.data.modelos.grupos.GrupoBusqueda
import com.example.appfavoresmovil.data.modelos.grupos.GrupoPublico
import retrofit2.http.Body
import retrofit2.http.Field
import retrofit2.http.FormUrlEncoded
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.PUT
import retrofit2.http.Path
import retrofit2.http.Query

interface ServicioApi {

    data class EnviarMensajeRequest(val contenido: String)
    @FormUrlEncoded
    @POST("usuarios/login")
    suspend fun login(
        @Field("username") username: String,
        @Field("password") password: String
    ): RespuestaLogin

    @POST("usuarios/registro")
    suspend fun registro(@Body usuario: UsuarioCreacion): UsuarioPublico

    @GET("favores/lista")
    suspend fun getFavoresDisponibles(
        @Query("estado") estado: String = "disponible"
    ): List<FavorPublico>

    @GET("favores/mis-publicados")
    suspend fun getMisFavores(): List<FavorPublico>

    @POST("favores/crear")
    suspend fun crearFavor(@Body favor: FavorCreacion): FavorPublico

    @PUT("favores/{id}/aceptar")
    suspend fun aceptarFavor(@Path("id") id: String): Map<String, String>

    @GET("grupos/mis-grupos")
    suspend fun getMisGrupos(): List<GrupoPublico>

    @GET("grupos/buscar-grupos")
    suspend fun buscarGrupos(@Query("nombre") nombre: String): List<GrupoBusqueda>

    @POST("grupos/crear")
    suspend fun crearGrupo(@Body body: Map<String, String?>): GrupoPublico

    @POST("grupos/{id}/unirse")
    suspend fun unirseGrupo(@Path("id") id: String): Map<String, String>

    @GET("chat/mis-chats")
    suspend fun getMisChats(): List<ChatPublico>

    @POST("chat/obtener-o-crear_chat")
    suspend fun obtenerOCrearChat(
        @Body body: Map<String, List<String>>
    ): ChatPublico

    @POST("chat/{id}/enviar")
    suspend fun enviarMensaje(
        @Path("id") id: String,
        @Body request: EnviarMensajeRequest
    ): MensajePublico

    @PUT("chat/{id}/leer-chats")
    suspend fun marcarLeido(@Path("id") id: String): Map<String, String>
}
