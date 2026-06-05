package com.example.appfavoresmovil.ui.vistamodelos

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.appfavoresmovil.data.modelos.favores.FavorCreacion
import com.example.appfavoresmovil.data.modelos.favores.FavorPublico
import com.example.appfavoresmovil.data.repositorios.RepositorioFavor
import com.example.appfavoresmovil.utilidades.ResultadoApi
import kotlinx.coroutines.launch

class VistaModeloFavor : ViewModel() {

    private val repositorio = RepositorioFavor()

    private val _favores = MutableLiveData<ResultadoApi<List<FavorPublico>>>()
    val favores: LiveData<ResultadoApi<List<FavorPublico>>> = _favores

    private val _crear = MutableLiveData<ResultadoApi<FavorPublico>>()
    val crear: LiveData<ResultadoApi<FavorPublico>> = _crear

    private val _aceptar = MutableLiveData<ResultadoApi<Map<String, String>>>()
    val aceptar: LiveData<ResultadoApi<Map<String, String>>> = _aceptar

    fun cargarFavores() {
        _favores.value = ResultadoApi.Cargando
        viewModelScope.launch {
            _favores.value = repositorio.obtenerDisponibles()
        }
    }

    fun crearFavor(favor: FavorCreacion) {
        _crear.value = ResultadoApi.Cargando
        viewModelScope.launch {
            _crear.value = repositorio.crear(favor)
        }
    }

    fun aceptarFavor(id: String) {
        _aceptar.value = ResultadoApi.Cargando
        viewModelScope.launch {
            _aceptar.value = repositorio.aceptar(id)
        }
    }
}
