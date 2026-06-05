using AppFavoresEscritorio.Navegacion;
using AppFavoresEscritorio.Servicios;
using AppFavoresEscritorio.VistaModelos.Autenticacion;
using AppFavoresEscritorio.VistaModelos.Chats;
using AppFavoresEscritorio.VistaModelos.Favores;
using AppFavoresEscritorio.VistaModelos.Grupos;
using AppFavoresEscritorio.VistaModelos.Inicio;
using AppFavoresEscritorio.VistaModelos.Notificaciones;
using AppFavoresEscritorio.VistaModelos.Perfil;
using AppFavoresEscritorio.Vistas.Autenticacion;
using AppFavoresEscritorio.Vistas.Chats;
using AppFavoresEscritorio.Vistas.Favores;
using AppFavoresEscritorio.Vistas.Grupos;
using AppFavoresEscritorio.Vistas.Inicio;
using AppFavoresEscritorio.Vistas.Notificaciones;
using AppFavoresEscritorio.Vistas.Perfil;
using AppFavoresEscritorio.Vistas.Informes;
using AppFavoresEscritorio.Vistas.Administracion;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Syncfusion.Maui.Toolkit.Hosting;

namespace AppFavoresEscritorio;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureSyncfusionToolkit() 
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf",   "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf",  "OpenSansSemibold");
            });

        // Servicios HTTP
        builder.Services.AddSingleton(_ => new ServicioAutenticacion(new HttpClient()));
        builder.Services.AddSingleton(_ => new ServicioChat(new HttpClient()));
        builder.Services.AddSingleton(_ => new ServicioFavor(new HttpClient()));
        builder.Services.AddSingleton(_ => new ServicioGrupo(new HttpClient()));
        builder.Services.AddSingleton(_ => new ServicioNotificacion(new HttpClient()));
        builder.Services.AddSingleton(_ => new ServicioPerfil(new HttpClient()));
        builder.Services.AddSingleton(_ => new ServicioAdmin(new HttpClient()));

        // Shell 
        builder.Services.AddSingleton<AppShell>();

        // VistaModelos 
        builder.Services.AddTransient<LoginVistaModelo>();
        builder.Services.AddTransient<RegistroVistaModelo>();
        builder.Services.AddTransient<InicioVistaModelo>();
        builder.Services.AddTransient<NotificacionesVistaModelo>();
        builder.Services.AddTransient<PerfilVistaModelo>();
        builder.Services.AddTransient<FavoresDisponiblesVistaModelo>();
        builder.Services.AddTransient<CrearFavorVistaModelo>();
        builder.Services.AddTransient<BuscarGruposVistaModelo>();
        builder.Services.AddTransient<MisGruposVistaModelo>();
        builder.Services.AddTransient<CrearGrupoVistaModelo>();
        builder.Services.AddTransient<DetalleGrupoVistaModelo>();
        builder.Services.AddTransient<DetalleChatGrupoVistaModelo>();
        builder.Services.AddTransient<ChatPrivadoVistaModelo>();

        // Vistas 
        builder.Services.AddTransient<PaginaLogin>();
        builder.Services.AddTransient<PaginaRegistro>();
        builder.Services.AddTransient<PaginaInicio>();
        builder.Services.AddTransient<PaginaNotificaciones>();
        builder.Services.AddTransient<PaginaPerfil>();
        builder.Services.AddTransient<PaginaFavoresDisponibles>();
        builder.Services.AddTransient<PaginaCrearFavor>();
        builder.Services.AddTransient<PaginaBuscarGrupos>();
        builder.Services.AddTransient<PaginaMisGrupos>();
        builder.Services.AddTransient<PaginaCrearGrupo>();
        builder.Services.AddTransient<PaginaDetalleGrupo>();
        builder.Services.AddTransient<PaginaDetalleChatGrupo>();
        builder.Services.AddTransient<PaginaChatPrivado>();
        builder.Services.AddTransient<ExplorarGruposVistaModelo>();
        builder.Services.AddTransient<PaginaExplorarGrupos>();
        builder.Services.AddTransient<PaginaInformes>();
        builder.Services.AddTransient<PaginaAdministracion>();
        

#if DEBUG
        builder.Logging.AddDebug();
#endif
// ── Quitar bordes y fondos nativos de WinUI 3 en Windows (Evita cilindro dentro de rectángulo) ──
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoBordersWindows", (handler, view) =>
        {
#if WINDOWS
            handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
            var pincelTransparente = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));
            handler.PlatformView.Background = pincelTransparente;
            
            handler.PlatformView.Resources["TextControlBackground"] = pincelTransparente;
            handler.PlatformView.Resources["TextControlBackgroundPointerOver"] = pincelTransparente;
            handler.PlatformView.Resources["TextControlBackgroundFocused"] = pincelTransparente;
            handler.PlatformView.Resources["TextControlBorderBrushFocused"] = pincelTransparente;
#endif
        });

        Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("NoBordersWindows", (handler, view) =>
        {
#if WINDOWS
            handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
            var pincelTransparente = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));
            handler.PlatformView.Background = pincelTransparente;
            
            handler.PlatformView.Resources["TextControlBackground"] = pincelTransparente;
            handler.PlatformView.Resources["TextControlBackgroundPointerOver"] = pincelTransparente;
            handler.PlatformView.Resources["TextControlBackgroundFocused"] = pincelTransparente;
            handler.PlatformView.Resources["TextControlBorderBrushFocused"] = pincelTransparente;
#endif
        });

        builder.ConfigureLifecycleEvents(events =>
{
#if WINDOWS
    events.AddWindows(windows =>
    {
        windows.OnWindowCreated(window =>
        {
            window.SystemBackdrop = null;
        });
    });
#endif
});
        return builder.Build();
    }
}