package com.example.appfavoresmovil.ui.adaptadores

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.DiffUtil
import androidx.recyclerview.widget.ListAdapter
import androidx.recyclerview.widget.RecyclerView
import com.example.appfavoresmovil.R
import com.example.appfavoresmovil.data.modelos.chats.ChatPublico
import com.example.appfavoresmovil.databinding.ElementoChatBinding
import com.example.appfavoresmovil.singleton.SesionApp

class AdaptadorChat(
    private val onClick: (ChatPublico) -> Unit
) : ListAdapter<ChatPublico, AdaptadorChat.ViewHolder>(DiffCallback()) {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ViewHolder {
        val binding = ElementoChatBinding.inflate(LayoutInflater.from(parent.context), parent, false)
        return ViewHolder(binding)
    }

    override fun onBindViewHolder(holder: ViewHolder, position: Int) {
        holder.bind(getItem(position))
    }

    inner class ViewHolder(private val binding: ElementoChatBinding) : RecyclerView.ViewHolder(binding.root) {
        fun bind(chat: ChatPublico) {
            binding.textNombre.text = when (chat.tipo) {
                "grupo" -> chat.nombreGrupo.ifEmpty { binding.root.context.getString(R.string.chat_grupo) }
                else -> chat.participantes.firstOrNull { !it.equals(SesionApp.correo, ignoreCase = true) }
                    ?: chat.participantes.firstOrNull().orEmpty()
            }
            val ultimo = chat.mensajes.lastOrNull()
            binding.textUltimoMensaje.text = ultimo?.contenido ?: ""
            binding.textFecha.text = ultimo?.fechaEnvio ?: chat.ultimaActividad
            binding.root.setOnClickListener { onClick(chat) }
        }
    }

    class DiffCallback : DiffUtil.ItemCallback<ChatPublico>() {
        override fun areItemsTheSame(old: ChatPublico, new: ChatPublico) = old.id == new.id
        override fun areContentsTheSame(old: ChatPublico, new: ChatPublico) = old == new
    }
}
