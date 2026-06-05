package com.example.appfavoresmovil.ui.vistamodelos

import android.app.Application
import androidx.lifecycle.AndroidViewModel
import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.viewModelScope
import com.example.appfavoresmovil.data.modelos.autenticacion.RespuestaLogin
import com.example.appfavoresmovil.data.modelos.autenticacion.UsuarioCreacion
import com.example.appfavoresmovil.data.modelos.autenticacion.UsuarioPublico
import com.example.appfavoresmovil.data.repositorios.RepositorioAutenticacion
import com.example.appfavoresmovil.utilidades.ResultadoApi
import kotlinx.coroutines.launch

class VistaModeloAutenticacion(application: Application) : AndroidViewModel(application) {

    private val repositorio = RepositorioAutenticacion(application.applicationContext)

    private val _login = MutableLiveData<ResultadoApi<RespuestaLogin>>()
    val login: LiveData<ResultadoApi<RespuestaLogin>> = _login

    private val _registro = MutableLiveData<ResultadoApi<UsuarioPublico>>()
    val registro: LiveData<ResultadoApi<UsuarioPublico>> = _registro

    fun login(correo: String, contrasena: String) {
        _login.value = ResultadoApi.Cargando
        viewModelScope.launch {
            _login.value = repositorio.login(correo, contrasena)
        }
    }

    fun registro(usuario: UsuarioCreacion) {
        _registro.value = ResultadoApi.Cargando
        viewModelScope.launch {
            _registro.value = repositorio.registro(usuario)
        }
    }

    fun cerrarSesion() {
        repositorio.cerrarSesion()
    }
}
