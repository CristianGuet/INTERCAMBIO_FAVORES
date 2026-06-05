package com.example.appfavoresmovil.ui.adaptadores

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.recyclerview.widget.DiffUtil
import androidx.recyclerview.widget.ListAdapter
import androidx.recyclerview.widget.RecyclerView
import com.example.appfavoresmovil.R
import com.example.appfavoresmovil.data.modelos.grupos.GrupoPublico
import com.example.appfavoresmovil.databinding.ElementoGrupoBinding
import com.example.appfavoresmovil.singleton.SesionApp

class AdaptadorGrupo(
    private val onClick: (GrupoPublico) -> Unit
) : ListAdapter<GrupoPublico, AdaptadorGrupo.ViewHolder>(DiffCallback()) {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ViewHolder {
        val binding = ElementoGrupoBinding.inflate(LayoutInflater.from(parent.context), parent, false)
        return ViewHolder(binding)
    }

    override fun onBindViewHolder(holder: ViewHolder, position: Int) {
        holder.bind(getItem(position))
    }

    inner class ViewHolder(private val binding: ElementoGrupoBinding) : RecyclerView.ViewHolder(binding.root) {
        fun bind(grupo: GrupoPublico) {
            binding.textNombre.text = grupo.nombreGrupo
            binding.textDescripcion.text = grupo.descripcion
            binding.textMiembros.text = binding.root.context.getString(
                R.string.miembros_count, grupo.miembros.size
            )
            val esAdmin = grupo.miembros.any {
                it.correoUsuario.equals(SesionApp.correo, ignoreCase = true) && it.rol == "admin"
            }
            binding.badgeAdmin.visibility = if (esAdmin) View.VISIBLE else View.GONE
            binding.root.setOnClickListener { onClick(grupo) }
        }
    }

    class DiffCallback : DiffUtil.ItemCallback<GrupoPublico>() {
        override fun areItemsTheSame(old: GrupoPublico, new: GrupoPublico) = old.id == new.id
        override fun areContentsTheSame(old: GrupoPublico, new: GrupoPublico) = old == new
    }
}
