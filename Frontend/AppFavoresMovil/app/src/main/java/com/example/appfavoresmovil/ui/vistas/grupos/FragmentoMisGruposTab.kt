package com.example.appfavoresmovil.ui.vistas.grupos

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.core.os.bundleOf
import androidx.fragment.app.Fragment
import androidx.fragment.app.viewModels
import androidx.lifecycle.lifecycleScope
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.appfavoresmovil.R
import com.example.appfavoresmovil.databinding.FragmentoMisGruposTabBinding
import com.example.appfavoresmovil.ui.adaptadores.AdaptadorGrupo
import com.example.appfavoresmovil.ui.vistamodelos.VistaModeloGrupo
import com.example.appfavoresmovil.utilidades.ResultadoApi
import com.google.android.material.snackbar.Snackbar
import com.google.gson.Gson
import kotlinx.coroutines.launch

class FragmentoMisGruposTab : Fragment() {

    private var _binding: FragmentoMisGruposTabBinding? = null
    private val binding get() = _binding!!
    private val viewModel: VistaModeloGrupo by viewModels({ requireParentFragment() })
    private lateinit var adapter: AdaptadorGrupo

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View {
        _binding = FragmentoMisGruposTabBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        adapter = AdaptadorGrupo { grupo ->
            findNavController().navigate(
                R.id.action_misGruposFragment_to_detalleGrupoFragment,
                bundleOf(
                    "grupoId" to grupo.id,
                    "nombreGrupo" to grupo.nombreGrupo,
                    "descripcion" to grupo.descripcion,
                    "miembrosJson" to Gson().toJson(grupo.miembros)
                )
            )
        }
        binding.recyclerGrupos.layoutManager = LinearLayoutManager(requireContext())
        binding.recyclerGrupos.adapter = adapter

        viewLifecycleOwner.lifecycleScope.launch {
            viewModel.misGrupos.observe(viewLifecycleOwner) { resultado ->
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

        viewModel.cargarMisGrupos()
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}
