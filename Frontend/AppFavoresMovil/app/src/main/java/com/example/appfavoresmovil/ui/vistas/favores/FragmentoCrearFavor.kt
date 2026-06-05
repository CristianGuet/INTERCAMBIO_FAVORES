package com.example.appfavoresmovil.ui.vistas.favores

import android.Manifest
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ArrayAdapter
import androidx.activity.result.contract.ActivityResultContracts
import androidx.core.view.isVisible
import androidx.fragment.app.Fragment
import androidx.fragment.app.viewModels
import androidx.lifecycle.lifecycleScope
import com.example.appfavoresmovil.R
import com.example.appfavoresmovil.data.modelos.favores.FavorCreacion
import com.example.appfavoresmovil.databinding.FragmentoCrearFavorBinding
import com.example.appfavoresmovil.ui.vistamodelos.VistaModeloFavor
import com.example.appfavoresmovil.utilidades.ResultadoApi
import com.example.appfavoresmovil.utilidades.AyudanteUbicacion
import com.google.android.material.snackbar.Snackbar
import kotlinx.coroutines.launch

class FragmentoCrearFavor : Fragment() {

    private var _binding: FragmentoCrearFavorBinding? = null
    private val binding get() = _binding!!
    private val viewModel: VistaModeloFavor by viewModels()
    private lateinit var ayudanteUbicacion: AyudanteUbicacion

    private val opcionesCompensacion = listOf("favor", "dinero", "ambos")

    private val permisoUbicacion = registerForActivityResult(
        ActivityResultContracts.RequestPermission()
    ) { concedido ->
        if (concedido) obtenerUbicacion()
        else Snackbar.make(binding.root, R.string.permiso_ubicacion_denegado, Snackbar.LENGTH_LONG).show()
    }

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View {
        _binding = FragmentoCrearFavorBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        ayudanteUbicacion = AyudanteUbicacion(requireContext())

        val spinnerAdapter = ArrayAdapter(
            requireContext(),
            android.R.layout.simple_spinner_dropdown_item,
            listOf(
                getString(R.string.compensacion_favor),
                getString(R.string.compensacion_dinero),
                getString(R.string.compensacion_ambos)
            )
        )
        binding.spinnerCompensacion.adapter = spinnerAdapter

        binding.switchPresencial.setOnCheckedChangeListener { _, checked ->
            binding.layoutUbicacion.isVisible = checked
        }

        binding.spinnerCompensacion.setOnItemSelectedListener(object : android.widget.AdapterView.OnItemSelectedListener {
            override fun onItemSelected(parent: android.widget.AdapterView<*>?, v: View?, pos: Int, id: Long) {
                actualizarCamposCompensacion(pos)
            }
            override fun onNothingSelected(parent: android.widget.AdapterView<*>?) = Unit
        })

        binding.btnUsarUbicacion.setOnClickListener { solicitarUbicacion() }
        binding.btnPublicar.setOnClickListener { publicarFavor() }

        viewLifecycleOwner.lifecycleScope.launch {
            viewModel.crear.observe(viewLifecycleOwner) { resultado ->
                when (resultado) {
                    is ResultadoApi.Cargando -> {
                        binding.progressBar.visibility = View.VISIBLE
                        binding.btnPublicar.isEnabled = false
                    }
                    is ResultadoApi.Exito -> {
                        binding.progressBar.visibility = View.GONE
                        binding.btnPublicar.isEnabled = true
                        Snackbar.make(binding.root, R.string.favor_publicado, Snackbar.LENGTH_SHORT).show()
                        requireActivity().onBackPressedDispatcher.onBackPressed()
                    }
                    is ResultadoApi.Error -> {
                        binding.progressBar.visibility = View.GONE
                        binding.btnPublicar.isEnabled = true
                        Snackbar.make(binding.root, resultado.mensaje, Snackbar.LENGTH_LONG).show()
                    }
                    null -> Unit
                }
            }
        }
    }

    private fun actualizarCamposCompensacion(posicion: Int) {
        binding.layoutCantidad.isVisible = posicion == 1 || posicion == 2
        binding.layoutRecompensa.isVisible = posicion == 0 || posicion == 2
    }

    private fun solicitarUbicacion() {
        when {
            ayudanteUbicacion.tienePermisoUbicacion() -> obtenerUbicacion()
            else -> permisoUbicacion.launch(Manifest.permission.ACCESS_FINE_LOCATION)
        }
    }

    private fun obtenerUbicacion() {
        Snackbar.make(binding.root, R.string.obteniendo_ubicacion, Snackbar.LENGTH_SHORT).show()
        viewLifecycleOwner.lifecycleScope.launch {
            val direccion = ayudanteUbicacion.obtenerDireccionActual()
            if (direccion != null) {
                binding.editUbicacion.setText(direccion)
            } else {
                Snackbar.make(binding.root, R.string.error_ubicacion, Snackbar.LENGTH_LONG).show()
            }
        }
    }

    private fun publicarFavor() {
        val titulo = binding.editTitulo.text?.toString()?.trim().orEmpty()
        val descripcion = binding.editDescripcion.text?.toString()?.trim().orEmpty()
        if (titulo.isBlank() || descripcion.isBlank()) {
            Snackbar.make(binding.root, R.string.campos_obligatorios, Snackbar.LENGTH_SHORT).show()
            return
        }

        val esPresencial = binding.switchPresencial.isChecked
        val tipoComp = opcionesCompensacion[binding.spinnerCompensacion.selectedItemPosition]
        val cantidad = binding.editCantidad.text?.toString()?.toFloatOrNull()
        val recompensa = binding.editRecompensa.text?.toString()?.trim()?.ifBlank { null }

        viewModel.crearFavor(
            FavorCreacion(
                titulo = titulo,
                descripcion = descripcion,
                modalidad = if (esPresencial) "presencial" else "remoto",
                ubicacion = if (esPresencial) binding.editUbicacion.text?.toString()?.trim() else null,
                tipoCompensacion = tipoComp,
                cantidadDinero = if (tipoComp == "dinero" || tipoComp == "ambos") cantidad else null,
                recompensaFavor = if (tipoComp == "favor" || tipoComp == "ambos") recompensa else null
            )
        )
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}
