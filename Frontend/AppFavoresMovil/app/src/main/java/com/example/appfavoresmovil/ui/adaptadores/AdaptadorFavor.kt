package com.example.appfavoresmovil.ui.adaptadores

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.recyclerview.widget.DiffUtil
import androidx.recyclerview.widget.ListAdapter
import androidx.recyclerview.widget.RecyclerView
import com.example.appfavoresmovil.R
import com.example.appfavoresmovil.data.modelos.favores.FavorPublico
import com.example.appfavoresmovil.databinding.ElementoFavorBinding
import com.example.appfavoresmovil.singleton.SesionApp

class AdaptadorFavor(
    private val onAceptar: (FavorPublico) -> Unit
) : ListAdapter<FavorPublico, AdaptadorFavor.ViewHolder>(DiffCallback()) {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ViewHolder {
        val binding = ElementoFavorBinding.inflate(LayoutInflater.from(parent.context), parent, false)
        return ViewHolder(binding)
    }

    override fun onBindViewHolder(holder: ViewHolder, position: Int) {
        holder.bind(getItem(position))
    }

    inner class ViewHolder(private val binding: ElementoFavorBinding) : RecyclerView.ViewHolder(binding.root) {
        fun bind(favor: FavorPublico) {
            binding.textTitulo.text = favor.titulo
            binding.textDescripcion.text = favor.descripcion
            binding.textOfertante.text = binding.root.context.getString(
                R.string.ofertante, favor.idUsuarioOfrece
            )
            binding.badgeModalidad.text = if (favor.modalidad == "presencial") {
                binding.root.context.getString(R.string.modalidad_presencial)
            } else {
                binding.root.context.getString(R.string.modalidad_remoto)
            }
            binding.textCompensacion.text = favor.tipoCompensacion.replaceFirstChar { it.uppercase() }

            val esPropio = favor.idUsuarioOfrece.equals(SesionApp.correo, ignoreCase = true)
            binding.btnAceptar.visibility = if (esPropio) View.GONE else View.VISIBLE
            binding.btnAceptar.setOnClickListener { onAceptar(favor) }
        }
    }

    class DiffCallback : DiffUtil.ItemCallback<FavorPublico>() {
        override fun areItemsTheSame(old: FavorPublico, new: FavorPublico) = old.id == new.id
        override fun areContentsTheSame(old: FavorPublico, new: FavorPublico) = old == new
    }
}
