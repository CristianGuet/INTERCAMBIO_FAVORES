package com.example.appfavoresmovil.ui.vistas.grupos

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.core.os.bundleOf
import androidx.fragment.app.Fragment
import androidx.navigation.fragment.findNavController
import com.example.appfavoresmovil.R
import com.example.appfavoresmovil.data.modelos.grupos.MiembroGrupo
import com.example.appfavoresmovil.databinding.FragmentoDetalleGrupoBinding
import com.google.gson.Gson
import com.google.gson.reflect.TypeToken

class FragmentoDetalleGrupo : Fragment() {

    private var _binding: FragmentoDetalleGrupoBinding? = null
    private val binding get() = _binding!!

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View {
        _binding = FragmentoDetalleGrupoBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        binding.textNombre.text = requireArguments().getString("nombreGrupo").orEmpty()
        binding.textDescripcion.text = requireArguments().getString("descripcion").orEmpty()

        val miembrosJson = requireArguments().getString("miembrosJson").orEmpty()
        val tipo = object : TypeToken<List<MiembroGrupo>>() {}.type
        val miembros: List<MiembroGrupo> = Gson().fromJson(miembrosJson, tipo)
        binding.textMiembros.text = miembros.joinToString("\n") { miembro ->
            val rol = if (miembro.rol == "admin") " (${getString(R.string.admin)})" else ""
            "${miembro.correoUsuario}$rol"
        }

        val correos = miembros.map { it.correoUsuario }
        binding.btnAbrirChat.setOnClickListener {
            findNavController().navigate(
                R.id.action_detalleGrupoFragment_to_chatDetalleFragment,
                bundleOf(
                    "chatId" to "",
                    "participantesJson" to Gson().toJson(correos)
                )
            )
        }
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}
