package com.example.appfavoresmovil.ui.vistas.grupos

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import androidx.fragment.app.viewModels
import androidx.lifecycle.lifecycleScope
import com.example.appfavoresmovil.R
import com.example.appfavoresmovil.databinding.FragmentoCrearGrupoBinding
import com.example.appfavoresmovil.ui.vistamodelos.VistaModeloGrupo
import com.example.appfavoresmovil.utilidades.ResultadoApi
import com.google.android.material.snackbar.Snackbar
import kotlinx.coroutines.launch

class FragmentoCrearGrupo : Fragment() {

    private var _binding: FragmentoCrearGrupoBinding? = null
    private val binding get() = _binding!!
    private val viewModel: VistaModeloGrupo by viewModels()

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View {
        _binding = FragmentoCrearGrupoBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        binding.btnCrear.setOnClickListener { crearGrupo() }

        viewLifecycleOwner.lifecycleScope.launch {
            viewModel.crear.observe(viewLifecycleOwner) { resultado ->
                when (resultado) {
                    is ResultadoApi.Cargando -> {
                        binding.progressBar.visibility = View.VISIBLE
                        binding.btnCrear.isEnabled = false
                    }
                    is ResultadoApi.Exito -> {
                        binding.progressBar.visibility = View.GONE
                        binding.btnCrear.isEnabled = true
                        Snackbar.make(binding.root, R.string.grupo_creado, Snackbar.LENGTH_SHORT).show()
                        requireActivity().onBackPressedDispatcher.onBackPressed()
                    }
                    is ResultadoApi.Error -> {
                        binding.progressBar.visibility = View.GONE
                        binding.btnCrear.isEnabled = true
                        Snackbar.make(binding.root, resultado.mensaje, Snackbar.LENGTH_LONG).show()
                    }
                    null -> Unit
                }
            }
        }
    }

    private fun crearGrupo() {
        val nombre = binding.editNombre.text?.toString()?.trim().orEmpty()
        val descripcion = binding.editDescripcion.text?.toString()?.trim()?.ifBlank { null }
        if (nombre.length < 3) {
            Snackbar.make(binding.root, R.string.nombre_grupo_corto, Snackbar.LENGTH_SHORT).show()
            return
        }
        viewModel.crearGrupo(nombre, descripcion)
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}
