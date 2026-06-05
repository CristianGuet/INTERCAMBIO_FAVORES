package com.example.appfavoresmovil

import android.content.Intent
import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity
import androidx.navigation.fragment.NavHostFragment
import androidx.navigation.ui.setupWithNavController
import com.example.appfavoresmovil.databinding.ActividadInicioBinding

class ActividadInicio : AppCompatActivity() {

    private lateinit var binding: ActividadInicioBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActividadInicioBinding.inflate(layoutInflater)
        setContentView(binding.root)

        val navHost = supportFragmentManager
            .findFragmentById(R.id.nav_host_home) as NavHostFragment
        val navController = navHost.navController

        binding.bottomNavigation.setupWithNavController(navController)
    }

    fun cerrarSesionYVolverALogin() {
        startActivity(
            Intent(this, MainActivity::class.java).apply {
                flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
            }
        )
        finish()
    }
}
