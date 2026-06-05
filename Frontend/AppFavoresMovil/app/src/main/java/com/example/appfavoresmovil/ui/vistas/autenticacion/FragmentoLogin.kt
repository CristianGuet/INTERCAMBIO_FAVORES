package com.example.appfavoresmovil.ui.vistas.autenticacion

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import androidx.fragment.app.viewModels
import androidx.lifecycle.lifecycleScope
import androidx.navigation.fragment.findNavController
import com.example.appfavoresmovil.MainActivity
import com.example.appfavoresmovil.R
import com.example.appfavoresmovil.databinding.FragmentoLoginBinding
import com.example.appfavoresmovil.ui.vistamodelos.VistaModeloAutenticacion
import com.example.appfavoresmovil.utilidades.ResultadoApi
import com.google.android.material.snackbar.Snackbar
import kotlinx.coroutines.launch

class FragmentoLogin : Fragment() {

    private var _binding: FragmentoLoginBinding? = null
    private val binding get() = _binding!!
    private val viewModel: VistaModeloAutenticacion by viewModels()

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View {
        _binding = FragmentoLoginBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        binding.btnConectar.setOnClickListener { intentarLogin() }
        binding.textRegistro.setOnClickListener {
            findNavController().navigate(R.id.action_loginFragment_to_registroFragment)
        }

        viewLifecycleOwner.lifecycleScope.launch {
            viewModel.login.observe(viewLifecycleOwner) { resultado ->
                when (resultado) {
                    is ResultadoApi.Cargando -> {
                        binding.progressBar.visibility = View.VISIBLE
                        binding.btnConectar.isEnabled = false
                    }
                    is ResultadoApi.Exito -> {
                        binding.progressBar.visibility = View.GONE
                        binding.btnConectar.isEnabled = true
                        (activity as? MainActivity)?.irAHome()
                    }
                    is ResultadoApi.Error -> {
                        binding.progressBar.visibility = View.GONE
                        binding.btnConectar.isEnabled = true
                        Snackbar.make(binding.root, resultado.mensaje, Snackbar.LENGTH_LONG).show()
                    }
                    null -> Unit
                }
            }
        }
    }

    private fun intentarLogin() {
        val correo = binding.editCorreo.text?.toString()?.trim().orEmpty()
        val contrasena = binding.editContrasena.text?.toString().orEmpty()

        if (correo.isBlank() || contrasena.isBlank()) {
            Snackbar.make(binding.root, R.string.campos_obligatorios, Snackbar.LENGTH_SHORT).show()
            return
        }
        viewModel.login(correo, contrasena)
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}
