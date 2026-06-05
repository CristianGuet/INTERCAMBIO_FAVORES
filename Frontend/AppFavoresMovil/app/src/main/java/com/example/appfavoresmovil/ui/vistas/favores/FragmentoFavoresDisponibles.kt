package com.example.appfavoresmovil.ui.vistas.favores

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import androidx.fragment.app.viewModels
import androidx.lifecycle.lifecycleScope
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.appfavoresmovil.R
import com.example.appfavoresmovil.databinding.FragmentoFavoresDisponiblesBinding
import com.example.appfavoresmovil.ui.adaptadores.AdaptadorFavor
import com.example.appfavoresmovil.ui.vistamodelos.VistaModeloFavor
import com.example.appfavoresmovil.utilidades.ResultadoApi
import com.google.android.material.snackbar.Snackbar
import kotlinx.coroutines.launch

class FragmentoFavoresDisponibles : Fragment() {

    private var _binding: FragmentoFavoresDisponiblesBinding? = null
    private val binding get() = _binding!!
    private val viewModel: VistaModeloFavor by viewModels()
    private lateinit var adapter: AdaptadorFavor

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View {
        _binding = FragmentoFavoresDisponiblesBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        adapter = AdaptadorFavor { favor -> viewModel.aceptarFavor(favor.id) }
        binding.recyclerFavores.layoutManager = LinearLayoutManager(requireContext())
        binding.recyclerFavores.adapter = adapter

        binding.swipeRefresh.setOnRefreshListener { viewModel.cargarFavores() }
        binding.fabCrear.setOnClickListener {
            findNavController().navigate(R.id.action_favoresDisponiblesFragment_to_crearFavorFragment)
        }

        viewLifecycleOwner.lifecycleScope.launch {
            viewModel.favores.observe(viewLifecycleOwner) { resultado ->
                binding.swipeRefresh.isRefreshing = false
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
            viewModel.aceptar.observe(viewLifecycleOwner) { resultado ->
                when (resultado) {
                    is ResultadoApi.Exito -> {
                        Snackbar.make(binding.root, R.string.favor_aceptado, Snackbar.LENGTH_SHORT).show()
                        viewModel.cargarFavores()
                    }
                    is ResultadoApi.Error ->
                        Snackbar.make(binding.root, resultado.mensaje, Snackbar.LENGTH_LONG).show()
                    else -> Unit
                }
            }
        }

        viewModel.cargarFavores()
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}
