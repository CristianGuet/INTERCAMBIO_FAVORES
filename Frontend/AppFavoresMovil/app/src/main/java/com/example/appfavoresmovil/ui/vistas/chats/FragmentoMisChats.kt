package com.example.appfavoresmovil.ui.vistas.chats

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
import com.example.appfavoresmovil.data.modelos.chats.ChatPublico
import com.example.appfavoresmovil.data.modelos.grupos.GrupoPublico
import com.example.appfavoresmovil.databinding.FragmentoMisChatsBinding
import com.example.appfavoresmovil.ui.adaptadores.AdaptadorChat
import com.example.appfavoresmovil.ui.vistamodelos.VistaModeloChat
import com.example.appfavoresmovil.ui.vistamodelos.VistaModeloGrupo
import com.example.appfavoresmovil.utilidades.ResultadoApi
import com.google.android.material.snackbar.Snackbar
import kotlinx.coroutines.launch

class FragmentoMisChats : Fragment() {

    private var _binding: FragmentoMisChatsBinding? = null
    private val binding get() = _binding!!
    private val viewModelChat: VistaModeloChat by viewModels()
    private val viewModelGrupo: VistaModeloGrupo by viewModels({ requireActivity() })
    private lateinit var adapter: AdaptadorChat
    private var chatsOriginales: List<ChatPublico> = emptyList()
    private var grupos: List<GrupoPublico> = emptyList()

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View {
        _binding = FragmentoMisChatsBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        adapter = AdaptadorChat { chat ->
            findNavController().navigate(
                R.id.action_misChatsFragment_to_chatDetalleFragment,
                bundleOf("chatId" to chat.id, "participantesJson" to "")
            )
        }
        binding.recyclerChats.layoutManager = LinearLayoutManager(requireContext())
        binding.recyclerChats.adapter = adapter

        // Observar chats
        viewLifecycleOwner.lifecycleScope.launch {
            viewModelChat.chats.observe(viewLifecycleOwner) { resultado ->
                when (resultado) {
                    is ResultadoApi.Cargando -> {
                        binding.progressBar.visibility = View.VISIBLE
                        binding.textVacio.visibility = View.GONE
                    }
                    is ResultadoApi.Exito -> {
                        chatsOriginales = resultado.datos
                        combinarYMostrar()
                    }
                    is ResultadoApi.Error -> {
                        binding.progressBar.visibility = View.GONE
                        Snackbar.make(binding.root, resultado.mensaje, Snackbar.LENGTH_LONG).show()
                    }
                    null -> Unit
                }
            }
        }

        // Observar grupos
        viewLifecycleOwner.lifecycleScope.launch {
            viewModelGrupo.misGrupos.observe(viewLifecycleOwner) { resultado ->
                when (resultado) {
                    is ResultadoApi.Exito -> {
                        grupos = resultado.datos
                        combinarYMostrar()
                    }
                    else -> Unit
                }
            }
        }

        viewModelChat.cargarChats()
        viewModelGrupo.cargarMisGrupos()
    }

    private fun combinarYMostrar() {
        if (chatsOriginales.isEmpty() && grupos.isEmpty()) return
        chatsOriginales.forEach { chat ->
            if (chat.tipo == "grupo") {
                val grupo = grupos.find { grupo ->
                    val correosGrupo = grupo.miembros.map { it.correoUsuario }.sorted()
                    val correosChat = chat.participantes.sorted()
                    correosGrupo == correosChat
                }
                chat.nombreGrupo = grupo?.nombreGrupo ?: "Chat del grupo"
            }
        }
        adapter.submitList(chatsOriginales)
        binding.progressBar.visibility = View.GONE
        binding.textVacio.visibility = if (chatsOriginales.isEmpty()) View.VISIBLE else View.GONE
    }

    override fun onResume() {
        super.onResume()
        viewModelChat.cargarChats()
        viewModelGrupo.cargarMisGrupos()
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}