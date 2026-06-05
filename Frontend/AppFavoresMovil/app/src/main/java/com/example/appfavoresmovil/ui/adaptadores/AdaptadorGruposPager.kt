package com.example.appfavoresmovil.ui.adaptadores

import androidx.fragment.app.Fragment
import androidx.viewpager2.adapter.FragmentStateAdapter
import com.example.appfavoresmovil.ui.vistas.grupos.FragmentoExplorarGrupos
import com.example.appfavoresmovil.ui.vistas.grupos.FragmentoMisGruposTab

class AdaptadorGruposPager(fragment: Fragment) : FragmentStateAdapter(fragment) {

    override fun getItemCount() = 2

    override fun createFragment(position: Int): Fragment {
        return when (position) {
            0 -> FragmentoMisGruposTab()
            else -> FragmentoExplorarGrupos()
        }
    }
}
