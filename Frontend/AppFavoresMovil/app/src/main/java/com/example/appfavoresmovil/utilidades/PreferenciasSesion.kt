package com.example.appfavoresmovil.utilidades

import android.content.Context
import androidx.security.crypto.EncryptedSharedPreferences
import androidx.security.crypto.MasterKey
import com.example.appfavoresmovil.singleton.SesionApp

object PreferenciasSesion {

    private const val ARCHIVO = "sesion_favsy"
    private const val CLAVE_TOKEN = "token_acceso"
    private const val CLAVE_CORREO = "correo"
    private const val CLAVE_NOMBRE = "nombre"
    private const val CLAVE_ROL = "rol"

    private fun prefs(context: Context) = EncryptedSharedPreferences.create(
        context,
        ARCHIVO,
        MasterKey.Builder(context).setKeyScheme(MasterKey.KeyScheme.AES256_GCM).build(),
        EncryptedSharedPreferences.PrefKeyEncryptionScheme.AES256_SIV,
        EncryptedSharedPreferences.PrefValueEncryptionScheme.AES256_GCM
    )

    fun guardarSesion(context: Context, token: String, correo: String, nombre: String, rol: String) {
        prefs(context).edit()
            .putString(CLAVE_TOKEN, token)
            .putString(CLAVE_CORREO, correo)
            .putString(CLAVE_NOMBRE, nombre)
            .putString(CLAVE_ROL, rol)
            .apply()
        SesionApp.token = token
        SesionApp.correo = correo
        SesionApp.nombre = nombre
        SesionApp.rol = rol
    }

    fun cargarSesion(context: Context): Boolean {
        val p = prefs(context)
        val token = p.getString(CLAVE_TOKEN, "") ?: ""
        if (token.isBlank()) return false
        SesionApp.token = token
        SesionApp.correo = p.getString(CLAVE_CORREO, "") ?: ""
        SesionApp.nombre = p.getString(CLAVE_NOMBRE, "") ?: ""
        SesionApp.rol = p.getString(CLAVE_ROL, "Usuario") ?: "Usuario"
        return true
    }

    fun limpiarSesion(context: Context) {
        prefs(context).edit().clear().apply()
        SesionApp.limpiar()
    }
}
