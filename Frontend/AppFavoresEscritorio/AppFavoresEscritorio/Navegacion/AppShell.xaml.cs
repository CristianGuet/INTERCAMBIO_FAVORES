using AppFavoresEscritorio.Modelos.Autenticacion;
using AppFavoresEscritorio.Servicios;
using AppFavoresEscritorio.Singleton;
using AppFavoresEscritorio.Utilidades;
using AppFavoresEscritorio.Vistas.Chats;
using AppFavoresEscritorio.Vistas.Favores;
using AppFavoresEscritorio.Vistas.Grupos;
using AppFavoresEscritorio.Vistas.Notificaciones;
using AppFavoresEscritorio.Vistas.Perfil;
using AppFavoresEscritorio.Vistas.Informes;
using AppFavoresEscritorio.Vistas.Administracion;
 
namespace AppFavoresEscritorio.Navegacion;
 
public partial class AppShell : Shell
{
    private readonly ConfiguracionApp _config = ConfiguracionApp.Instancia;
    private readonly ServicioAutenticacion _servicioAutenticacion;
    private readonly ServicioChat _servicioChat;
    private readonly ServicioFavor _servicioFavor;
    private readonly ServicioGrupo _servicioGrupo;
    private readonly ServicioNotificacion _servicioNotificacion;
    private readonly ServicioPerfil _servicioPerfil;
    private bool _cambiandoTema;
    private bool _estiloHamburguesaAplicado = false;
 
    public AppShell(
        ServicioAutenticacion servicioAutenticacion,
        ServicioChat servicioChat,
        ServicioFavor servicioFavor,
        ServicioGrupo servicioGrupo,
        ServicioNotificacion servicioNotificacion,
        ServicioPerfil servicioPerfil)
    {
        _servicioAutenticacion = servicioAutenticacion;
        _servicioChat = servicioChat;
        _servicioFavor = servicioFavor;
        _servicioGrupo = servicioGrupo;
        _servicioNotificacion = servicioNotificacion;
        _servicioPerfil = servicioPerfil;
        _cambiandoTema = false;
 
        InitializeComponent();
        BindingContext = _config;
 
        Loaded += OnShellCargado;
 
        Routing.RegisterRoute(nameof(PaginaNotificaciones), typeof(PaginaNotificaciones));
        Routing.RegisterRoute(nameof(PaginaPerfil), typeof(PaginaPerfil));
        Routing.RegisterRoute(nameof(PaginaFavoresDisponibles), typeof(PaginaFavoresDisponibles));
        Routing.RegisterRoute(nameof(PaginaBuscarGrupos), typeof(PaginaBuscarGrupos));
        Routing.RegisterRoute(nameof(PaginaMisGrupos), typeof(PaginaMisGrupos));
        Routing.RegisterRoute(nameof(PaginaDetalleChatGrupo), typeof(PaginaDetalleChatGrupo));
        Routing.RegisterRoute(nameof(PaginaInformes), typeof(PaginaInformes));
        Routing.RegisterRoute(nameof(PaginaAdministracion), typeof(PaginaAdministracion));
        Routing.RegisterRoute("detallechatgrupo", typeof(PaginaDetalleChatGrupo));
        Routing.RegisterRoute("chatprivado", typeof(PaginaChatPrivado));
        Routing.RegisterRoute("crearfavor", typeof(PaginaCrearFavor));
        Routing.RegisterRoute("creargrupo", typeof(PaginaCrearGrupo));
        Routing.RegisterRoute("detallegrupo", typeof(PaginaDetalleGrupo));
        Routing.RegisterRoute("PaginaExplorarGrupos", typeof(PaginaExplorarGrupos));
 
        _config.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(ConfiguracionApp.EsTemaOscuro) or nameof(ConfiguracionApp.CodigoIdioma))
            {
                AplicarColoresShell();
                ActualizarBotonesPreferencias();
            }
        };
    }
 
    public void InicializarApariencia()
    {
        _config.CargarPreferencias(notificar: false);
        GestorIdioma.AplicarIdioma(_config.CodigoIdioma);
        GestorTema.AplicarTema(_config.EsTemaOscuro);
        _config.RefrescarEtiquetas();
        AplicarColoresShell();
        ActualizarBotonesPreferencias();
    }
 
    private void OnShellCargado(object? sender, EventArgs e)
    {
        Loaded -= OnShellCargado;
#if WINDOWS
        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(100), AplicarEstiloHamburguesa);
#endif
    }
 
    public async Task MostrarMenuTrasLogin(UsuarioResumen perfil)
    {
        SesionApp.Instancia.EstablecerUsuario(perfil);
        SesionApp.Instancia.EstablecerRol(perfil.Rol ?? "Usuario");
 
        var token = await _servicioAutenticacion.ObtenerTokenAsync();
        AplicarTokenEnServicios(token);
        FlyoutBehavior = FlyoutBehavior.Flyout;
        MostrarFlyoutsMenu();
 
        tabBarAuth.IsVisible = false;
        Shell.SetTabBarIsVisible(this, false);
 
        btnCerrarSesion.IsVisible = true;
        AplicarColoresShell();
        await GoToAsync("//PaginaInicio");
    }
 
    public async Task CerrarSesionAsync()
    {
        try
        {
            await Task.Delay(200);
            await _servicioAutenticacion.CerrarSesionAsync();

            SesionApp.Instancia.Limpiar();
            AplicarTokenEnServicios(null);

            btnCerrarSesion.IsVisible = false;
            tabBarAuth.IsVisible = true;
            Shell.SetTabBarIsVisible(this, true);

            await GoToAsync("//PaginaLogin");

            FlyoutBehavior = FlyoutBehavior.Disabled;
            OcultarFlyoutsMenu();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CerrarSesion] Error: {ex.GetType().Name} - {ex.Message}");
            System.Diagnostics.Debug.WriteLine(ex.StackTrace);

            try
            {
                SesionApp.Instancia.Limpiar();
                btnCerrarSesion.IsVisible = false;
                tabBarAuth.IsVisible = true;
                Shell.SetTabBarIsVisible(this, true);
                await GoToAsync("//PaginaLogin");
                FlyoutBehavior = FlyoutBehavior.Disabled;
                OcultarFlyoutsMenu();
            }
            catch (Exception ex2)
            {
                System.Diagnostics.Debug.WriteLine($"[CerrarSesion Fallback] {ex2.Message}");
            }
        }
    }
 
    private async void OnCerrarSesionClicked(object? sender, EventArgs e)
    {
        try
        {
            await CerrarSesionAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[OnCerrarSesion] {ex.Message}");
        }
    }
 
    private void AplicarTokenEnServicios(string? token)
    {
        _servicioAutenticacion.AplicarToken(token);
        _servicioChat.AplicarToken(token);
        _servicioFavor.AplicarToken(token);
        _servicioGrupo.AplicarToken(token);
        _servicioNotificacion.AplicarToken(token);
        _servicioPerfil.AplicarToken(token);
    }
 
    private void MostrarFlyoutsMenu()
    {
        flyoutInicio.IsVisible = true;
        flyoutNotificaciones.IsVisible = true;
        flyoutPerfil.IsVisible = true;
        flyoutFavores.IsVisible = true;
        flyoutMisGrupos.IsVisible = true;
        flyoutExplorarGrupos.IsVisible = true;
        flyoutInformes.IsVisible = true;
        if (SesionApp.Instancia.EsAdmin) flyoutAdmin.IsVisible = true;
    }
 
    private void OcultarFlyoutsMenu()
    {
        flyoutInicio.IsVisible = false;
        flyoutNotificaciones.IsVisible = false;
        flyoutPerfil.IsVisible = false;
        flyoutFavores.IsVisible = false;
        flyoutMisGrupos.IsVisible = false;
        flyoutExplorarGrupos.IsVisible = false;
        flyoutInformes.IsVisible = false;
        flyoutAdmin.IsVisible = false;
    }
 
    public void AplicarColoresShell()
    {
        if (!MainThread.IsMainThread)
        {
            MainThread.BeginInvokeOnMainThread(AplicarColoresShell);
            return;
        }
 
        var fondo = GestorRecursos.ObtenerColor("ColorFondo", Colors.White);
        var texto = GestorRecursos.ObtenerColor("ColorTexto", Colors.Black);
        var textoSecundario = GestorRecursos.ObtenerColor("ColorTextoSecundario", Colors.Gray);
        var primario = GestorRecursos.ObtenerColor("ColorPrimario", Colors.Purple);
        var superficie = GestorRecursos.ObtenerColor("ColorSuperficie", Colors.White);
 
        this.BackgroundColor = superficie;
        this.FlyoutBackgroundColor = superficie;
        Shell.SetBackgroundColor(this, fondo);
        Shell.SetForegroundColor(this, texto);
        Shell.SetTitleColor(this, texto);
        Shell.SetUnselectedColor(this, texto);
        Shell.SetDisabledColor(this, textoSecundario);
        Shell.SetTabBarBackgroundColor(this, superficie);
        Shell.SetTabBarTitleColor(this, textoSecundario);
        Shell.SetTabBarUnselectedColor(this, textoSecundario);
        Shell.SetTabBarForegroundColor(this, primario);
 
        barraPreferencias.BackgroundColor = superficie;
 
        ActualizarBotonesPreferencias();
    }
 
    private void ActualizarBotonesPreferencias()
    {
        var primario = GestorRecursos.ObtenerColor("ColorPrimario", Colors.Purple);
        btnIdioma.TextColor = primario;
        btnIdioma.BorderColor = primario;
        btnTema.TextColor = primario;
        btnTema.BorderColor = primario;
        btnCerrarSesion.TextColor = primario;
        btnCerrarSesion.BorderColor = primario;
 
        btnIdioma.Text = _config.EtiquetaIdiomaActual;
        btnTema.Text = _config.EtiquetaTemaActual;
    }
 
    private void OnIdiomaClicked(object? sender, EventArgs e) => _config.AlternarIdioma();
 
    private void OnTemaClicked(object? sender, EventArgs e)
    {
        if (_cambiandoTema) return;
        _cambiandoTema = true;
        try { _config.AlternarTema(); }
        finally { _cambiandoTema = false; }
    }
 
#if WINDOWS
    private void AplicarEstiloHamburguesa()
    {
        if (_estiloHamburguesaAplicado) return;
        _estiloHamburguesaAplicado = true;
 
        var ventana = Microsoft.Maui.Controls.Application.Current?
            .Windows.FirstOrDefault()?.Handler?.PlatformView
            as Microsoft.UI.Xaml.Window;
        if (ventana is null) return;
 
        void ActualizarColoresBarraTitulo()
        {
            try
            {
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(ventana);
                var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
                var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
                if (appWindow?.TitleBar != null && Microsoft.UI.Windowing.AppWindowTitleBar.IsCustomizationSupported())
                {
                    var colorFondo = GestorRecursos.ObtenerColor("ColorSuperficie", Colors.White);
                    var colorTexto = GestorRecursos.ObtenerColor("ColorTexto", Colors.Black);
                    var winFondo = ToWinColor(colorFondo);
                    var winTexto = ToWinColor(colorTexto);
                    appWindow.TitleBar.BackgroundColor = winFondo;
                    appWindow.TitleBar.ForegroundColor = winTexto;
                    appWindow.TitleBar.ButtonBackgroundColor = winFondo;
                    appWindow.TitleBar.ButtonForegroundColor = winTexto;
                    appWindow.TitleBar.InactiveBackgroundColor = winFondo;
                    appWindow.TitleBar.ButtonInactiveBackgroundColor = winFondo;
                    appWindow.TitleBar.ButtonInactiveForegroundColor = winTexto;
                }
            }
            catch { }
        }
 
        if (ventana.Content is not Microsoft.UI.Xaml.FrameworkElement raiz) return;
        var navView = BuscarDescendienteWin<Microsoft.UI.Xaml.Controls.NavigationView>(raiz);
        if (navView is null) return;
 
        navView.IsSettingsVisible = false;
 
        void AplicarColoresNavView()
        {
            var colorTexto = GestorRecursos.ObtenerColor("ColorTexto", Colors.Black);
            var colorFondo = GestorRecursos.ObtenerColor("ColorSuperficie", Colors.White);
            var brushTexto = new Microsoft.UI.Xaml.Media.SolidColorBrush(ToWinColor(colorTexto));
            var brushFondo = new Microsoft.UI.Xaml.Media.SolidColorBrush(ToWinColor(colorFondo));
            var brushTransparente = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));
 
            navView.Background = brushFondo;
 
            SetNavResource(navView, "NavigationViewItemForeground", brushTexto);
            SetNavResource(navView, "NavigationViewItemForegroundPointerOver", brushTexto);
            SetNavResource(navView, "NavigationViewItemForegroundPressed", brushTexto);
            SetNavResource(navView, "NavigationViewItemForegroundSelected", brushTexto);
            SetNavResource(navView, "NavigationViewItemForegroundDisabled", brushTexto);
 
            SetNavResource(navView, "NavigationViewItemBackground", brushTransparente);
            SetNavResource(navView, "NavigationViewItemBackgroundPointerOver", brushTransparente);
            SetNavResource(navView, "NavigationViewItemBackgroundPressed", brushTransparente);
            SetNavResource(navView, "NavigationViewItemBackgroundSelected", brushTransparente);
            SetNavResource(navView, "NavigationViewItemBackgroundSelectedPointerOver", brushTransparente);
 
            SetNavResource(navView, "NavigationViewItemBorderBrush", brushTransparente);
            SetNavResource(navView, "NavigationViewItemBorderBrushPointerOver", brushTransparente);
            SetNavResource(navView, "NavigationViewItemBorderBrushPressed", brushTransparente);
            SetNavResource(navView, "NavigationViewItemBorderBrushSelected", brushTransparente);
            SetNavResource(navView, "NavigationViewItemBorderBrushSelectedPointerOver", brushTransparente);
            SetNavResource(navView, "NavigationViewItemBorderThickness", new Microsoft.UI.Xaml.Thickness(0));
            SetNavResource(navView, "NavigationViewItemSeparatorForeground", brushTransparente);
            SetNavResource(navView, "NavigationViewItemSeparatorBackground", brushTransparente);
 
            SetNavResource(navView, "NavigationViewContentBackground", brushFondo);
            SetNavResource(navView, "NavigationViewExpandedPaneBackground", brushFondo);
            SetNavResource(navView, "NavigationViewDefaultPaneBackground", brushFondo);
 
            SetNavResource(navView, "NavigationViewSelectionIndicatorForeground", brushTransparente);
 
            var toggleBtn = BuscarPorNombreWin<Microsoft.UI.Xaml.Controls.Button>(navView, "TogglePaneButton");
            if (toggleBtn is not null)
            {
                var colorPrimario = GestorRecursos.ObtenerColor("ColorPrimario", Colors.Purple);
                var brushPrimario = new Microsoft.UI.Xaml.Media.SolidColorBrush(ToWinColor(colorPrimario));
                toggleBtn.Foreground = brushPrimario;
                toggleBtn.Background = brushFondo;
                toggleBtn.Opacity = 1;
                toggleBtn.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            }
 
            var moreBtn = BuscarPorNombreWin<Microsoft.UI.Xaml.Controls.Button>(navView, "MoreButton");
            if (moreBtn is not null)
                moreBtn.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }
 
        ActualizarColoresBarraTitulo();
        AplicarColoresNavView();
 
        _config.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ConfiguracionApp.EsTemaOscuro))
            {
                ActualizarColoresBarraTitulo();
                AplicarColoresNavView();
            }
        };
    }
 
    private static void SetNavResource(Microsoft.UI.Xaml.Controls.NavigationView navView, string clave, object valor)
    {
        try { navView.Resources[clave] = valor; } catch { }
    }
 
    private static Windows.UI.Color ToWinColor(Microsoft.Maui.Graphics.Color c) =>
        Windows.UI.Color.FromArgb((byte)(c.Alpha * 255), (byte)(c.Red * 255), (byte)(c.Green * 255), (byte)(c.Blue * 255));
 
    private static T? BuscarDescendienteWin<T>(Microsoft.UI.Xaml.DependencyObject padre) where T : Microsoft.UI.Xaml.DependencyObject
    {
        int n = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(padre);
        for (int i = 0; i < n; i++)
        {
            var hijo = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChild(padre, i);
            if (hijo is T t) return t;
            var r = BuscarDescendienteWin<T>(hijo);
            if (r is not null) return r;
        }
        return null;
    }
 
    private static T? BuscarPorNombreWin<T>(Microsoft.UI.Xaml.DependencyObject padre, string nombre) where T : Microsoft.UI.Xaml.FrameworkElement
    {
        int n = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(padre);
        for (int i = 0; i < n; i++)
        {
            var hijo = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChild(padre, i);
            if (hijo is T el && el.Name == nombre) return el;
            var r = BuscarPorNombreWin<T>(hijo, nombre);
            if (r is not null) return r;
        }
        return null;
    }
#endif
}