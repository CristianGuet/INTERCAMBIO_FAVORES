# Estructura del Proyecto Favsy

## Backend (FastAPI)
"""
Backend/
├── Cargas/ # Imágenes predefinidas
│ ├── claro_amistad.png
│ ├── default.png
│ ├── group_default.png
│ └── oscuro_amistad.png
├── Configuracion/
│ ├── ajustes.py
│ └── bbdd.py
├── Modelos/
│ ├── chat.py
│ ├── favor.py
│ ├── grupo.py
│ ├── modeloBase.py
│ ├── notificacion.py
│ └── usuario.py
├── Rutas/
│ ├── admin.py
│ ├── chat.py
│ ├── favor.py
│ ├── grupo.py
│ ├── notificacion.py
│ ├── perfil.py
│ └── usuario.py
├── Utilidades/
│ ├── correo.py
│ ├── formateador.py
│ └── seguridad.py
├── main.py
├── requirements.txt
└── test_API.py
"""

## Frontend – AppFavoresEscritorio (MAUI / .NET)
"""
AppFavoresEscritorio/
├── AppFavoresEscritorio/
│ ├── Controles/
│ │ └── EntryLimpio.cs
│ ├── Modelos/
│ │ ├── Autenticacion/
│ │ │ ├── RespuestaLogin.cs
│ │ │ ├── Usuario.cs
│ │ │ └── UsuarioPublico.cs
│ │ ├── Chats/
│ │ │ └── ChatPublico.cs
│ │ ├── Favores/
│ │ │ ├── FavorCreacion.cs
│ │ │ └── FavorPublico.cs
│ │ ├── Grupos/
│ │ │ └── GrupoPublico.cs
│ │ └── Notificaciones/
│ │ └── Notificacion.cs
│ ├── Navegacion/
│ │ ├── AppShell.xaml
│ │ └── AppShell.xaml.cs
│ ├── Platforms/
│ │ ├── Android/
│ │ │ ├── Resources/values/colors.xml
│ │ │ ├── AndroidManifest.xml
│ │ │ ├── MainActivity.cs
│ │ │ └── MainApplication.cs
│ │ ├── iOS/
│ │ │ ├── Resources/PrivacyInfo.xcprivacy
│ │ │ ├── AppDelegate.cs
│ │ │ ├── Info.plist
│ │ │ └── Program.cs
│ │ ├── MacCatalyst/
│ │ │ ├── AppDelegate.cs
│ │ │ ├── Entitlements.plist
│ │ │ ├── Info.plist
│ │ │ └── Program.cs
│ │ └── Windows/
│ │ ├── app.manifest
│ │ ├── App.xaml
│ │ ├── App.xaml.cs
│ │ └── Package.appxmanifest
│ ├── Properties/
│ │ └── launchSettings.json
│ ├── Resources/
│ │ ├── AppIcon/
│ │ │ ├── appicon.svg
│ │ │ ├── appiconfg.svg
│ │ │ └── icono.ico
│ │ ├── Fonts/
│ │ │ ├── OpenSans-Regular.ttf
│ │ │ └── OpenSans-Semibold.ttf
│ │ ├── Idiomas/
│ │ │ ├── StringsEn.xaml
│ │ │ ├── StringsEn.xaml.cs
│ │ │ ├── StringsEs.xaml
│ │ │ └── StringsEs.xaml.cs
│ │ ├── Images/
│ │ │ ├── claro_amistad.png
│ │ │ ├── default.png
│ │ │ ├── group_default.png
│ │ │ └── oscuro_amistad.png
│ │ ├── Raw/
│ │ │ └── AboutAssets.txt
│ │ ├── Splash/
│ │ │ └── splash.svg
│ │ └── Styles/
│ │ ├── TemaClaro.xaml
│ │ ├── TemaClaro.xaml.cs
│ │ ├── TemaOscuro.xaml
│ │ └── TemaOscuro.xaml.cs
│ ├── Servicios/
│ │ ├── Base/
│ │ │ └── ServicioApi.cs
│ │ ├── ServicioAdmin.cs
│ │ ├── ServicioAutenticacion.cs
│ │ ├── ServicioChat.cs
│ │ ├── ServicioFavor.cs
│ │ ├── ServicioGrupo.cs
│ │ ├── ServicioNotificacion.cs
│ │ └── ServicioPerfil.cs
│ ├── Singleton/
│ │ ├── ConfiguracionApp.cs
│ │ └── SesionApp.cs
│ ├── Utilidades/
│ │ ├── ConvertidorEstadoFavor.cs
│ │ ├── ConvertidorTituloChat.cs
│ │ ├── ConvertidorUrlImagen.cs
│ │ ├── ConvertidosEsCero.cs
│ │ ├── GestorIdioma.cs
│ │ ├── GestorRecursos.cs
│ │ ├── GestorTema.cs
│ │ └── InvertirBoolConverter.cs
│ ├── VistaModelos/
│ │ ├── Administracion/
│ │ │ └── AdministracionVistaModelo.cs
│ │ ├── Autenticacion/
│ │ │ ├── LoginVistaModelo.cs
│ │ │ └── RegistroVistaModelo.cs
│ │ ├── Base/
│ │ │ └── VistaModeloBase.cs
│ │ ├── Chats/
│ │ │ └── ChatPrivadoVistaModelo.cs
│ │ ├── Favores/
│ │ │ ├── CrearFavorVistaModelo.cs
│ │ │ └── FavoresDisponiblesVistaModelo.cs
│ │ ├── Grupos/
│ │ │ ├── BuscarGruposVistaModelo.cs
│ │ │ ├── CrearGrupoVistaModelo.cs
│ │ │ ├── DetalleChatGrupoVistaModelo.cs
│ │ │ ├── DetalleGrupoVistaModelo.cs
│ │ │ ├── ExplorarGruposVistaModelo.cs.cs
│ │ │ └── MisGruposVistaModelo.cs
│ │ ├── Inicio/
│ │ │ └── InicioVistaModelo.cs
│ │ ├── Notificaciones/
│ │ │ └── NotificacionesVistaModelo.cs
│ │ └── Perfil/
│ │ └── PerfilVistaModelo.cs
│ ├── Vistas/
│ │ ├── Administracion/
│ │ │ ├── PaginaAdministracion.xaml
│ │ │ └── PaginaAdministracion.xaml.cs
│ │ ├── Autenticacion/
│ │ │ ├── PaginaLogin.xaml
│ │ │ ├── PaginaLogin.xaml.cs
│ │ │ ├── PaginaRegistro.xaml
│ │ │ └── PaginaRegistro.xaml.cs
│ │ ├── Chats/
│ │ │ ├── PaginaChatPrivado.xaml
│ │ │ └── PaginaChatPrivado.xaml.cs
│ │ ├── Favores/
│ │ │ ├── PaginaCrearFavor.xaml
│ │ │ ├── PaginaCrearFavor.xaml.cs
│ │ │ ├── PaginaFavoresDisponibles.xaml
│ │ │ └── PaginaFavoresDisponibles.xaml.cs
│ │ ├── Grupos/
│ │ │ ├── PaginaBuscarGrupos.xaml
│ │ │ ├── PaginaBuscarGrupos.xaml.cs
│ │ │ ├── PaginaCrearGrupo.xaml
│ │ │ ├── PaginaCrearGrupo.xaml.cs
│ │ │ ├── PaginaDetalleChatGrupo.xaml
│ │ │ ├── PaginaDetalleChatGrupo.xaml.cs
│ │ │ ├── PaginaDetalleGrupo.xaml
│ │ │ ├── PaginaDetalleGrupo.xaml.cs
│ │ │ ├── PaginaExplorarGrupos.xaml
│ │ │ ├── PaginaExplorarGrupos.xaml.cs
│ │ │ ├── PaginaMisGrupos.xaml
│ │ │ └── PaginaMisGrupos.xaml.cs
│ │ ├── Informes/
│ │ │ ├── PaginaInformes.xaml
│ │ │ └── PaginaInformes.xaml.cs
│ │ ├── Inicio/
│ │ │ ├── PaginaInicio.xaml
│ │ │ └── PaginaInicio.xaml.cs
│ │ ├── Notificaciones/
│ │ │ ├── PaginaNotificaciones.xaml
│ │ │ └── PaginaNotificaciones.xaml.cs
│ │ └── Perfil/
│ │ ├── PaginaPerfil.xaml
│ │ └── PaginaPerfil.xaml.cs
│ ├── App.xaml
│ ├── App.xaml.cs
│ ├── AppFavoresEscritorio.csproj
│ └── MauiProgram.cs
└── AppFavoresEscritorio.slnx
"""

## Frontend – AppFavoresMovil (Android / Kotlin)
"""
AppFavoresMovil/
├── app/
│ ├── src/
│ │ ├── androidTest/java/com/example/appfavoresmovil/
│ │ │ └── ExampleInstrumentedTest.kt
│ │ ├── main/
│ │ │ ├── java/com/example/appfavoresmovil/
│ │ │ │ ├── data/
│ │ │ │ │ ├── modelos/
│ │ │ │ │ │ ├── autenticacion/
│ │ │ │ │ │ │ ├── RespuestaLogin.kt
│ │ │ │ │ │ │ ├── UsuarioCreacion.kt
│ │ │ │ │ │ │ └── UsuarioPublico.kt
│ │ │ │ │ │ ├── chats/
│ │ │ │ │ │ │ ├── ChatPublico.kt
│ │ │ │ │ │ │ └── MensajePublico.kt
│ │ │ │ │ │ ├── favores/
│ │ │ │ │ │ │ ├── FavorCreacion.kt
│ │ │ │ │ │ │ └── FavorPublico.kt
│ │ │ │ │ │ ├── grupos/
│ │ │ │ │ │ │ ├── GrupoBusqueda.kt
│ │ │ │ │ │ │ └── GrupoPublico.kt
│ │ │ │ │ │ └── notificaciones/
│ │ │ │ │ │ └── Notificacion.kt
│ │ │ │ │ ├── repositorios/
│ │ │ │ │ │ ├── RepositorioAutenticacion.kt
│ │ │ │ │ │ ├── RepositorioChat.kt
│ │ │ │ │ │ ├── RepositorioFavor.kt
│ │ │ │ │ │ └── RepositorioGrupo.kt
│ │ │ │ │ └── servicios/
│ │ │ │ │ ├── ClienteRetrofit.kt
│ │ │ │ │ └── ServicioApi.kt
│ │ │ │ ├── singleton/
│ │ │ │ │ └── SesionApp.kt
│ │ │ │ ├── ui/
│ │ │ │ │ ├── adaptadores/
│ │ │ │ │ │ ├── AdaptadorChat.kt
│ │ │ │ │ │ ├── AdaptadorFavor.kt
│ │ │ │ │ │ ├── AdaptadorGrupo.kt
│ │ │ │ │ │ ├── AdaptadorGrupoBusqueda.kt
│ │ │ │ │ │ ├── AdaptadorGruposPager.kt
│ │ │ │ │ │ └── AdaptadorMensaje.kt
│ │ │ │ │ ├── vistamodelos/
│ │ │ │ │ │ ├── VistaModeloAutenticacion.kt
│ │ │ │ │ │ ├── VistaModeloChat.kt
│ │ │ │ │ │ ├── VistaModeloFavor.kt
│ │ │ │ │ │ └── VistaModeloGrupo.kt
│ │ │ │ │ └── vistas/
│ │ │ │ │ ├── autenticacion/
│ │ │ │ │ │ ├── FragmentoLogin.kt
│ │ │ │ │ │ └── FragmentoRegistro.kt
│ │ │ │ │ ├── chats/
│ │ │ │ │ │ ├── FragmentoChatDetalle.kt
│ │ │ │ │ │ └── FragmentoMisChats.kt
│ │ │ │ │ ├── favores/
│ │ │ │ │ │ ├── FragmentoCrearFavor.kt
│ │ │ │ │ │ └── FragmentoFavoresDisponibles.kt
│ │ │ │ │ ├── grupos/
│ │ │ │ │ │ ├── FragmentoCrearGrupo.kt
│ │ │ │ │ │ ├── FragmentoDetalleGrupo.kt
│ │ │ │ │ │ ├── FragmentoExplorarGrupos.kt
│ │ │ │ │ │ ├── FragmentoMisGrupos.kt
│ │ │ │ │ │ └── FragmentoMisGruposTab.kt
│ │ │ │ │ └── perfil/
│ │ │ │ │ └── FragmentoPerfil.kt
│ │ │ │ ├── utilidades/
│ │ │ │ │ ├── AyudanteUbicacion.kt
│ │ │ │ │ ├── PreferenciasSesion.kt
│ │ │ │ │ └── ResultadoApi.kt
│ │ │ │ ├── ActividadInicio.kt
│ │ │ │ └── MainActivity.kt
│ │ │ ├── keepRules/
│ │ │ │ └── rules.keep
│ │ │ ├── res/
│ │ │ │ ├── drawable/
│ │ │ │ │ ├── badge_background.xml
│ │ │ │ │ ├── bubble_other.xml
│ │ │ │ │ ├── bubble_own.xml
│ │ │ │ │ ├── ic_launcher_background.xml
│ │ │ │ │ └── ic_launcher_foreground.xml
│ │ │ │ ├── layout/
│ │ │ │ │ ├── actividad_inicio.xml
│ │ │ │ │ ├── activity_main.xml
│ │ │ │ │ ├── elemento_chat.xml
│ │ │ │ │ ├── elemento_favor.xml
│ │ │ │ │ ├── elemento_grupo.xml
│ │ │ │ │ ├── elemento_grupo_busqueda.xml
│ │ │ │ │ ├── elemento_mensaje.xml
│ │ │ │ │ ├── fragmento_chat_detalle.xml
│ │ │ │ │ ├── fragmento_crear_favor.xml
│ │ │ │ │ ├── fragmento_crear_grupo.xml
│ │ │ │ │ ├── fragmento_detalle_grupo.xml
│ │ │ │ │ ├── fragmento_explorar_grupos.xml
│ │ │ │ │ ├── fragmento_favores_disponibles.xml
│ │ │ │ │ ├── fragmento_login.xml
│ │ │ │ │ ├── fragmento_mis_chats.xml
│ │ │ │ │ ├── fragmento_mis_grupos.xml
│ │ │ │ │ ├── fragmento_mis_grupos_tab.xml
│ │ │ │ │ ├── fragmento_perfil.xml
│ │ │ │ │ └── fragmento_registro.xml
│ │ │ │ ├── menu/
│ │ │ │ │ └── menu_bottom_nav.xml
│ │ │ │ ├── mipmap-anydpi-v26/
│ │ │ │ │ ├── ic_launcher.xml
│ │ │ │ │ └── ic_launcher_round.xml
│ │ │ │ ├── navigation/
│ │ │ │ │ ├── nav_graph.xml
│ │ │ │ │ └── nav_graph_auth.xml
│ │ │ │ ├── values/
│ │ │ │ │ ├── colors.xml
│ │ │ │ │ ├── dimens.xml
│ │ │ │ │ ├── strings.xml
│ │ │ │ │ └── themes.xml
│ │ │ │ ├── values-night/
│ │ │ │ │ ├── colors.xml
│ │ │ │ │ └── themes.xml
│ │ │ │ ├── values-v23/
│ │ │ │ │ └── themes.xml
│ │ │ │ └── xml/
│ │ │ │ ├── backup_rules.xml
│ │ │ │ ├── data_extraction_rules.xml
│ │ │ │ └── network_security_config.xml
│ │ │ └── AndroidManifest.xml
│ │ └── test/java/com/example/appfavoresmovil/
│ │ └── ExampleUnitTest.kt
│ ├── build.gradle.kts
│ └── proguard-rules.pro (opcional)
├── gradle/
│ ├── wrapper/
│ │ ├── gradle-wrapper.jar
│ │ └── gradle-wrapper.properties
│ └── libs.versions.toml
├── build.gradle.kts
├── gradle.properties
├── gradlew
├── gradlew.bat
├── local.properties
└── settings.gradle.kts
"""

## Archivos raíz del repositorio
"""
├── LICENSE
└── README.md
"""