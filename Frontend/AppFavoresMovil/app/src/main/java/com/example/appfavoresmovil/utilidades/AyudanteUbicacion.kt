package com.example.appfavoresmovil.utilidades

import android.Manifest
import android.content.Context
import android.content.pm.PackageManager
import android.location.Geocoder
import android.location.Location
import androidx.core.content.ContextCompat
import com.google.android.gms.location.LocationServices
import kotlinx.coroutines.suspendCancellableCoroutine
import java.util.Locale
import kotlin.coroutines.resume

class AyudanteUbicacion(private val context: Context) {

    private val fusedClient = LocationServices.getFusedLocationProviderClient(context)

    fun tienePermisoUbicacion(): Boolean {
        return ContextCompat.checkSelfPermission(
            context,
            Manifest.permission.ACCESS_FINE_LOCATION
        ) == PackageManager.PERMISSION_GRANTED
    }

    suspend fun obtenerDireccionActual(): String? {
        if (!tienePermisoUbicacion()) return null

        val location: Location? = suspendCancellableCoroutine { cont ->
            try {
                fusedClient.lastLocation
                    .addOnSuccessListener { location ->
                        cont.resume(location)
                    }
                    .addOnFailureListener {
                        cont.resume(null)
                    }
            } catch (e: SecurityException) {
                cont.resume(null)
            }
        }

        // Usar ?. para acceso seguro a propiedades de location (puede ser null)
        return location?.let { geocodificarInversa(it.latitude, it.longitude) }
    }

    @Suppress("DEPRECATION")
    private fun geocodificarInversa(latitud: Double, longitud: Double): String? {
        if (!Geocoder.isPresent()) return null

        return try {
            val geocoder = Geocoder(context, Locale.getDefault())
            val direccion = geocoder.getFromLocation(latitud, longitud, 1)?.firstOrNull()
            formatearDireccion(direccion)
        } catch (_: Exception) {
            null
        }
    }

    private fun formatearDireccion(direccion: android.location.Address?): String? {
        if (direccion == null) return null
        val partes = mutableListOf<String>()
        val calle = listOfNotNull(direccion.thoroughfare, direccion.subThoroughfare)
            .joinToString(" ").trim()
        if (calle.isNotBlank()) partes.add(calle)
        direccion.locality?.let { partes.add(it) }
        direccion.countryCode?.let { partes.add(it) }
        return partes.filter { it.isNotBlank() }.joinToString(", ").ifBlank { null }
    }
}