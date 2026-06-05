package com.example.appfavoresmovil.ui.vistas.chats

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import androidx.fragment.app.viewModels
import androidx.lifecycle.lifecycleScope
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.appfavoresmovil.databinding.FragmentoChatDetalleBinding
import com.example.appfavoresmovil.ui.adaptadores.AdaptadorMensaje
import com.example.appfavoresmovil.ui.vistamodelos.VistaModeloChat
import com.example.appfavoresmovil.utilidades.ResultadoApi
import com.google.android.material.snackbar.Snackbar
import com.google.gson.Gson
import com.google.gson.reflect.TypeToken
import kotlinx.coroutines.launch

class FragmentoChatDetalle : Fragment() {

    private var _binding: FragmentoChatDetalleBinding? = null
    private val binding get() = _binding!!
    private val viewModel: VistaModeloChat by viewModels()
    private lateinit var adapter: AdaptadorMensaje
    private var chatIdActual: String = ""

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View {
        _binding = FragmentoChatDetalleBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        adapter = AdaptadorMensaje()
        binding.recyclerMensajes.layoutManager = LinearLayoutManager(requireContext()).apply {
            stackFromEnd = true
        }
        binding.recyclerMensajes.adapter = adapter

        val participantes = parseParticipantes(requireArguments().getString("participantesJson").orEmpty())
        chatIdActual = requireArguments().getString("chatId").orEmpty()
        viewModel.cargarChat(chatIdActual, participantes.ifEmpty { null })

        binding.btnEnviar.setOnClickListener { enviarMensaje() }

        viewLifecycleOwner.lifecycleScope.launch {
            viewModel.chatDetalle.observe(viewLifecycleOwner) { resultado ->
                when (resultado) {
                    is ResultadoApi.Cargando -> binding.progressBar.visibility = View.VISIBLE
                    is ResultadoApi.Exito -> {
                        binding.progressBar.visibility = View.GONE
                        chatIdActual = resultado.datos.id
                        adapter.submitList(resultado.datos.mensajes) {
                            if (resultado.datos.mensajes.isNotEmpty()) {
                                binding.recyclerMensajes.scrollToPosition(resultado.datos.mensajes.size - 1)
                            }
                        }
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
            viewModel.enviar.observe(viewLifecycleOwner) { resultado ->
                if (resultado is ResultadoApi.Error) {
                    Snackbar.make(binding.root, resultado.mensaje, Snackbar.LENGTH_LONG).show()
                } else if (resultado is ResultadoApi.Exito) {
                    binding.editMensaje.text?.clear()
                }
            }
        }
    }

    private fun enviarMensaje() {
        val contenido = binding.editMensaje.text?.toString()?.trim().orEmpty()
        if (contenido.isBlank() || chatIdActual.isBlank()) return
        viewModel.enviarMensaje(chatIdActual, contenido)
    }

    private fun parseParticipantes(json: String): List<String> {
        if (json.isBlank()) return emptyList()
        val tipo = object : TypeToken<List<String>>() {}.type
        return Gson().fromJson(json, tipo)
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}
