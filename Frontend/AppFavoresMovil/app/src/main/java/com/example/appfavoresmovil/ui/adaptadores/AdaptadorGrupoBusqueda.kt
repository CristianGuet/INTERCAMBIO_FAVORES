package com.example.appfavoresmovil.ui.adaptadores

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.DiffUtil
import androidx.recyclerview.widget.ListAdapter
import androidx.recyclerview.widget.RecyclerView
import com.example.appfavoresmovil.R
import com.example.appfavoresmovil.data.modelos.grupos.GrupoBusqueda
import com.example.appfavoresmovil.databinding.ElementoGrupoBusquedaBinding

class AdaptadorGrupoBusqueda(
    private val onUnirse: (GrupoBusqueda) -> Unit
) : ListAdapter<GrupoBusqueda, AdaptadorGrupoBusqueda.ViewHolder>(DiffCallback()) {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ViewHolder {
        val binding = ElementoGrupoBusquedaBinding.inflate(LayoutInflater.from(parent.context), parent, false)
        return ViewHolder(binding)
    }

    override fun onBindViewHolder(holder: ViewHolder, position: Int) {
        holder.bind(getItem(position))
    }

    inner class ViewHolder(private val binding: ElementoGrupoBusquedaBinding) : RecyclerView.ViewHolder(binding.root) {
        fun bind(grupo: GrupoBusqueda) {
            binding.textNombre.text = grupo.nombreGrupo
            binding.textDescripcion.text = grupo.descripcion
            binding.textMiembros.text = binding.root.context.getString(
                R.string.miembros_count, grupo.numeroMiembros
            )
            binding.btnUnirse.setOnClickListener { onUnirse(grupo) }
        }
    }

    class DiffCallback : DiffUtil.ItemCallback<GrupoBusqueda>() {
        override fun areItemsTheSame(old: GrupoBusqueda, new: GrupoBusqueda) = old.id == new.id
        override fun areContentsTheSame(old: GrupoBusqueda, new: GrupoBusqueda) = old == new
    }
}
