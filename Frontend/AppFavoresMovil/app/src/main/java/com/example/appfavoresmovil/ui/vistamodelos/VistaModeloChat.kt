package com.example.appfavoresmovil.ui.vistamodelos

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.appfavoresmovil.data.modelos.chats.ChatPublico
import com.example.appfavoresmovil.data.modelos.chats.MensajePublico
import com.example.appfavoresmovil.data.repositorios.RepositorioChat
import com.example.appfavoresmovil.utilidades.ResultadoApi
import kotlinx.coroutines.launch

class VistaModeloChat : ViewModel() {

    private val repositorio = RepositorioChat()

    private val _chats = MutableLiveData<ResultadoApi<List<ChatPublico>>>()
    val chats: LiveData<ResultadoApi<List<ChatPublico>>> = _chats

    private val _chatDetalle = MutableLiveData<ResultadoApi<ChatPublico>>()
    val chatDetalle: LiveData<ResultadoApi<ChatPublico>> = _chatDetalle

    private val _enviar = MutableLiveData<ResultadoApi<MensajePublico>>()
    val enviar: LiveData<ResultadoApi<MensajePublico>> = _enviar

    fun cargarChats() {
        _chats.value = ResultadoApi.Cargando
        viewModelScope.launch {
            _chats.value = repositorio.obtenerMisChats()
        }
    }

    fun cargarChat(idChat: String, participantes: List<String>?) {
        _chatDetalle.value = ResultadoApi.Cargando
        viewModelScope.launch {
            val resultado = if (participantes != null && participantes.isNotEmpty()) {
                repositorio.obtenerOCrear(participantes)
            } else {
                repositorio.obtenerPorId(idChat)
            }
            if (resultado is ResultadoApi.Exito) {
                repositorio.marcarLeido(resultado.datos.id)
            }
            _chatDetalle.value = resultado
        }
    }

    fun enviarMensaje(idChat: String, contenido: String) {
        _enviar.value = ResultadoApi.Cargando
        viewModelScope.launch {
            val resultado = repositorio.enviarMensaje(idChat, contenido)
            _enviar.value = resultado
            if (resultado is ResultadoApi.Exito) {
                cargarChat(idChat, null)
            }
        }
    }
}
