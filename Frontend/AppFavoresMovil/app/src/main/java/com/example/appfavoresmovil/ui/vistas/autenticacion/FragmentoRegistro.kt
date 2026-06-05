package com.example.appfavoresmovil.ui.vistas.autenticacion

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import androidx.fragment.app.viewModels
import androidx.lifecycle.lifecycleScope
import androidx.navigation.fragment.findNavController
import com.example.appfavoresmovil.R
import com.example.appfavoresmovil.data.modelos.autenticacion.UsuarioCreacion
import com.example.appfavoresmovil.databinding.FragmentoRegistroBinding
import com.example.appfavoresmovil.ui.vistamodelos.VistaModeloAutenticacion
import com.example.appfavoresmovil.utilidades.ResultadoApi
import com.google.android.material.snackbar.Snackbar
import kotlinx.coroutines.launch

class FragmentoRegistro : Fragment() {

    private var _binding: FragmentoRegistroBinding? = null
    private val binding get() = _binding!!
    private val viewModel: VistaModeloAutenticacion by viewModels()

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View {
        _binding = FragmentoRegistroBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        binding.btnRegistrarme.setOnClickListener { intentarRegistro() }
        binding.textLogin.setOnClickListener {
            findNavController().navigateUp()
        }

        viewLifecycleOwner.lifecycleScope.launch {
            viewModel.registro.observe(viewLifecycleOwner) { resultado ->
                when (resultado) {
                    is ResultadoApi.Cargando -> {
                        binding.progressBar.visibility = View.VISIBLE
                        binding.btnRegistrarme.isEnabled = false
                    }
                    is ResultadoApi.Exito -> {
                        binding.progressBar.visibility = View.GONE
                        binding.btnRegistrarme.isEnabled = true
                        Snackbar.make(binding.root, R.string.registro_exitoso, Snackbar.LENGTH_LONG).show()
                        findNavController().navigateUp()
                    }
                    is ResultadoApi.Error -> {
                        binding.progressBar.visibility = View.GONE
                        binding.btnRegistrarme.isEnabled = true
                        Snackbar.make(binding.root, resultado.mensaje, Snackbar.LENGTH_LONG).show()
                    }
                    null -> Unit
                }
            }
        }
    }

    private fun intentarRegistro() {
        val nombre = binding.editNombre.text?.toString()?.trim().orEmpty()
        val correo = binding.editCorreo.text?.toString()?.trim().orEmpty()
        val contrasena = binding.editContrasena.text?.toString().orEmpty()
        val repetir = binding.editContrasenaRepetir.text?.toString().orEmpty()

        when {
            nombre.isBlank() || correo.isBlank() || contrasena.isBlank() || repetir.isBlank() ->
                Snackbar.make(binding.root, R.string.campos_obligatorios, Snackbar.LENGTH_SHORT).show()
            !correo.contains("@") ->
                Snackbar.make(binding.root, R.string.correo_invalido, Snackbar.LENGTH_SHORT).show()
            contrasena.length < 6 ->
                Snackbar.make(binding.root, R.string.contrasena_corta, Snackbar.LENGTH_SHORT).show()
            contrasena != repetir ->
                Snackbar.make(binding.root, R.string.contrasenas_no_coinciden, Snackbar.LENGTH_SHORT).show()
            else -> viewModel.registro(UsuarioCreacion(correo, contrasena, nombre))
        }
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}
