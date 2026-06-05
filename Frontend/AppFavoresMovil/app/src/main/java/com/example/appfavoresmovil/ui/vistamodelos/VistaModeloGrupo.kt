package com.example.appfavoresmovil.ui.vistamodelos

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.appfavoresmovil.data.modelos.grupos.GrupoBusqueda
import com.example.appfavoresmovil.data.modelos.grupos.GrupoPublico
import com.example.appfavoresmovil.data.repositorios.RepositorioGrupo
import com.example.appfavoresmovil.utilidades.ResultadoApi
import kotlinx.coroutines.launch

class VistaModeloGrupo : ViewModel() {

    private val repositorio = RepositorioGrupo()

    private val _misGrupos = MutableLiveData<ResultadoApi<List<GrupoPublico>>>()
    val misGrupos: LiveData<ResultadoApi<List<GrupoPublico>>> = _misGrupos

    private val _busqueda = MutableLiveData<ResultadoApi<List<GrupoBusqueda>>>()
    val busqueda: LiveData<ResultadoApi<List<GrupoBusqueda>>> = _busqueda

    private val _crear = MutableLiveData<ResultadoApi<GrupoPublico>>()
    val crear: LiveData<ResultadoApi<GrupoPublico>> = _crear

    private val _unirse = MutableLiveData<ResultadoApi<Map<String, String>>>()
    val unirse: LiveData<ResultadoApi<Map<String, String>>> = _unirse

    fun cargarMisGrupos() {
        _misGrupos.value = ResultadoApi.Cargando
        viewModelScope.launch {
            _misGrupos.value = repositorio.obtenerMisGrupos()
        }
    }

    fun buscarGrupos(nombre: String) {
        if (nombre.length < 3) {
            _busqueda.value = ResultadoApi.Exito(emptyList())
            return
        }
        _busqueda.value = ResultadoApi.Cargando
        viewModelScope.launch {
            _busqueda.value = repositorio.buscar(nombre)
        }
    }

    fun crearGrupo(nombre: String, descripcion: String?) {
        _crear.value = ResultadoApi.Cargando
        viewModelScope.launch {
            _crear.value = repositorio.crear(nombre, descripcion)
        }
    }

    fun unirseGrupo(id: String) {
        _unirse.value = ResultadoApi.Cargando
        viewModelScope.launch {
            _unirse.value = repositorio.unirse(id)
        }
    }
}
