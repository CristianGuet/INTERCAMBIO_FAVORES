package com.example.appfavoresmovil.data.servicios

import android.os.Build
import com.example.appfavoresmovil.singleton.SesionApp
import okhttp3.Interceptor
import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import java.util.concurrent.TimeUnit

object ClienteRetrofit {

    private fun esEmulador(): Boolean {
        return (Build.FINGERPRINT.startsWith("generic")
                || Build.FINGERPRINT.startsWith("unknown")
                || Build.MODEL.contains("google_sdk")
                || Build.MODEL.contains("Android SDK built for x86")
                || Build.MANUFACTURER.contains("Genymotion")
                || (Build.BRAND.startsWith("generic") && Build.DEVICE.startsWith("generic"))
                || Build.PRODUCT == "google_sdk")
    }

    val BASE_URL = if (esEmulador()) {
        "http://10.0.2.2:8000/"
    } else {
        "http://192.168.1.142:8000/"
    }
    private val authInterceptor = Interceptor { chain ->
        val peticion = chain.request().newBuilder().apply {
            if (SesionApp.token.isNotBlank()) {
                addHeader("Authorization", "Bearer ${SesionApp.token}")
            }
        }.build()
        chain.proceed(peticion)
    }

    private val loggingInterceptor = HttpLoggingInterceptor().apply {
        level = HttpLoggingInterceptor.Level.BODY
    }

    private val okHttpClient = OkHttpClient.Builder()
        .addInterceptor(authInterceptor)
        .addInterceptor(loggingInterceptor)
        .connectTimeout(30, TimeUnit.SECONDS)
        .readTimeout(30, TimeUnit.SECONDS)
        .build()

    val api: ServicioApi by lazy {
        Retrofit.Builder()
            .baseUrl(BASE_URL)
            .client(okHttpClient)
            .addConverterFactory(GsonConverterFactory.create())
            .build()
            .create(ServicioApi::class.java)
    }
}
