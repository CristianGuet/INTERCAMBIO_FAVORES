package com.example.appfavoresmovil.ui.vistas.grupos

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.appcompat.widget.SearchView
import androidx.fragment.app.Fragment
import androidx.fragment.app.viewModels
import androidx.lifecycle.lifecycleScope
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.appfavoresmovil.R
import com.example.appfavoresmovil.databinding.FragmentoExplorarGruposBinding
import com.example.appfavoresmovil.ui.adaptadores.AdaptadorGrupoBusqueda
import com.example.appfavoresmovil.ui.vistamodelos.VistaModeloGrupo
import com.example.appfavoresmovil.utilidades.ResultadoApi
import com.google.android.material.snackbar.Snackbar
import kotlinx.coroutines.launch

class FragmentoExplorarGrupos : Fragment() {

    private var _binding: FragmentoExplorarGruposBinding? = null
    private val binding get() = _binding!!
    private val viewModel: VistaModeloGrupo by viewModels({ requireParentFragment() })
    private lateinit var adapter: AdaptadorGrupoBusqueda

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View {
        _binding = FragmentoExplorarGruposBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        adapter = AdaptadorGrupoBusqueda { grupo -> viewModel.unirseGrupo(grupo.id) }
        binding.recyclerBusqueda.layoutManager = LinearLayoutManager(requireContext())
        binding.recyclerBusqueda.adapter = adapter

        binding.searchView.setOnQueryTextListener(object : SearchView.OnQueryTextListener {
            override fun onQueryTextSubmit(query: String?): Boolean {
                query?.let { viewModel.buscarGrupos(it.trim()) }
                return true
            }
            override fun onQueryTextChange(newText: String?): Boolean {
                if ((newText?.length ?: 0) >= 3) viewModel.buscarGrupos(newText!!.trim())
                return true
            }
        })

        viewLifecycleOwner.lifecycleScope.launch {
            viewModel.busqueda.observe(viewLifecycleOwner) { resultado ->
                when (resultado) {
                    is ResultadoApi.Cargando -> {
                        binding.progressBar.visibility = View.VISIBLE
                        binding.textVacio.visibility = View.GONE
                    }
                    is ResultadoApi.Exito -> {
                        binding.progressBar.visibility = View.GONE
                        adapter.submitList(resultado.datos)
                        binding.textVacio.visibility =
                            if (resultado.datos.isEmpty()) View.VISIBLE else View.GONE
                    }
                    is ResultadoApi.Error -> {
                        binding.progressBar.visibility = View.GONE
                        Snackbar.make(binding.root, resultado.mensaje, Snackbar.LENGTH_LONG).show()
                    }
                    null -> Unit
                }
            }
        }

        viewLifecycleOwner.lifecycleScope.launch {
            viewModel.unirse.observe(viewLifecycleOwner) { resultado ->
                when (resultado) {
                    is ResultadoApi.Exito -> {
                        Snackbar.make(binding.root, R.string.unido_grupo, Snackbar.LENGTH_SHORT).show()
                        viewModel.cargarMisGrupos()
                    }
                    is ResultadoApi.Error ->
                        Snackbar.make(binding.root, resultado.mensaje, Snackbar.LENGTH_LONG).show()
                    else -> Unit
                }
            }
        }
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}
