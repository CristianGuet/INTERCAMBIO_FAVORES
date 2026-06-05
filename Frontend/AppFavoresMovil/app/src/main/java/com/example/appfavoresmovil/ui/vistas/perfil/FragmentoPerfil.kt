package com.example.appfavoresmovil.ui.vistas.perfil

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import androidx.fragment.app.viewModels
import com.example.appfavoresmovil.ActividadInicio
import com.example.appfavoresmovil.R
import com.example.appfavoresmovil.databinding.FragmentoPerfilBinding
import com.example.appfavoresmovil.singleton.SesionApp
import com.example.appfavoresmovil.ui.vistamodelos.VistaModeloAutenticacion

class FragmentoPerfil : Fragment() {

    private var _binding: FragmentoPerfilBinding? = null
    private val binding get() = _binding!!
    private val authViewModel: VistaModeloAutenticacion by viewModels()

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View {
        _binding = FragmentoPerfilBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        binding.textNombre.text = SesionApp.nombre.ifBlank { SesionApp.correo }
        binding.textCorreo.text = SesionApp.correo
        binding.textRol.text = getString(R.string.rol) + ": " + SesionApp.rol

        binding.btnCerrarSesion.setOnClickListener {
            authViewModel.cerrarSesion()
            (activity as? ActividadInicio)?.cerrarSesionYVolverALogin()
        }
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}
