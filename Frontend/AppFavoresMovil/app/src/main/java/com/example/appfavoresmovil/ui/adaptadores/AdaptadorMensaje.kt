package com.example.appfavoresmovil.ui.adaptadores

import android.view.Gravity
import android.view.LayoutInflater
import android.view.ViewGroup
import android.widget.FrameLayout
import androidx.recyclerview.widget.DiffUtil
import androidx.recyclerview.widget.ListAdapter
import androidx.recyclerview.widget.RecyclerView
import com.example.appfavoresmovil.data.modelos.chats.MensajePublico
import com.example.appfavoresmovil.databinding.ElementoMensajeBinding
import com.example.appfavoresmovil.singleton.SesionApp

class AdaptadorMensaje : ListAdapter<MensajePublico, AdaptadorMensaje.ViewHolder>(DiffCallback()) {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ViewHolder {
        val binding = ElementoMensajeBinding.inflate(LayoutInflater.from(parent.context), parent, false)
        return ViewHolder(binding)
    }

    override fun onBindViewHolder(holder: ViewHolder, position: Int) {
        holder.bind(getItem(position))
    }

    inner class ViewHolder(private val binding: ElementoMensajeBinding) : RecyclerView.ViewHolder(binding.root) {
        fun bind(mensaje: MensajePublico) {
            val esPropio = mensaje.emisor.equals(SesionApp.correo, ignoreCase = true)
            binding.textContenido.text = mensaje.contenido
            binding.textFecha.text = mensaje.fechaEnvio

            val params = binding.bubbleContainer.layoutParams as FrameLayout.LayoutParams
            params.gravity = if (esPropio) Gravity.END else Gravity.START
            binding.bubbleContainer.layoutParams = params

            binding.bubbleContainer.setBackgroundResource(
                if (esPropio) com.example.appfavoresmovil.R.drawable.bubble_own
                else com.example.appfavoresmovil.R.drawable.bubble_other
            )
            binding.textContenido.setTextColor(
                binding.root.context.getColor(
                    if (esPropio) com.example.appfavoresmovil.R.color.colorOnPrimary
                    else com.example.appfavoresmovil.R.color.colorOnSurface
                )
            )
        }
    }

    class DiffCallback : DiffUtil.ItemCallback<MensajePublico>() {
        override fun areItemsTheSame(old: MensajePublico, new: MensajePublico) =
            old.emisor == new.emisor && old.fechaEnvio == new.fechaEnvio && old.contenido == new.contenido

        override fun areContentsTheSame(old: MensajePublico, new: MensajePublico) = old == new
    }
}
