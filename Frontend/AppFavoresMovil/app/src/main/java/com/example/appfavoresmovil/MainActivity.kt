package com.example.appfavoresmovil

import android.content.Intent
import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.app.AppCompatDelegate
import com.example.appfavoresmovil.databinding.ActivityMainBinding
import com.example.appfavoresmovil.utilidades.PreferenciasSesion

class MainActivity : AppCompatActivity() {

    private lateinit var binding: ActivityMainBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_FOLLOW_SYSTEM)
        super.onCreate(savedInstanceState)

        PreferenciasSesion.cargarSesion(this)

        if (com.example.appfavoresmovil.singleton.SesionApp.estaLogueado) {
            irAHome()
            return
        }

        binding = ActivityMainBinding.inflate(layoutInflater)
        setContentView(binding.root)
    }

    fun irAHome() {
        startActivity(
            Intent(this, ActividadInicio::class.java).apply {
                flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
            }
        )
        finish()
    }
}
