package com.example.appfavoresmovil.ui.vistas.grupos

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import androidx.navigation.fragment.findNavController
import com.example.appfavoresmovil.R
import com.example.appfavoresmovil.databinding.FragmentoMisGruposBinding
import com.example.appfavoresmovil.ui.adaptadores.AdaptadorGruposPager
import com.google.android.material.tabs.TabLayoutMediator

class FragmentoMisGrupos : Fragment() {

    private var _binding: FragmentoMisGruposBinding? = null
    private val binding get() = _binding!!

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View {
        _binding = FragmentoMisGruposBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        binding.viewPager.adapter = AdaptadorGruposPager(this)
        TabLayoutMediator(binding.tabLayout, binding.viewPager) { tab, pos ->
            tab.text = if (pos == 0) getString(R.string.mis_grupos) else getString(R.string.explorar)
        }.attach()

        binding.fabCrear.setOnClickListener {
            findNavController().navigate(R.id.action_misGruposFragment_to_crearGrupoFragment)
        }
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}
