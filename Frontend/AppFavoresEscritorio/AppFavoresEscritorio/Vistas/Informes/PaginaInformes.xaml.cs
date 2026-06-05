using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AppFavoresEscritorio.Servicios;
using AppFavoresEscritorio.Singleton;
using Syncfusion.Maui.Toolkit.Charts;

namespace AppFavoresEscritorio.Vistas.Informes;

public partial class PaginaInformes : ContentPage, INotifyPropertyChanged
{
    private readonly ServicioFavor _servicioFavor;
    private readonly ServicioAutenticacion _servicioAuth;
    private bool _isRefreshing;
    private bool _isCargando;
    private string _mensajeError = string.Empty;
    private bool _hayDatos;

    public event PropertyChangedEventHandler? PropertyChanged;

    public PaginaInformes(ServicioFavor servicioFavor, ServicioAutenticacion servicioAuth)
    {
        InitializeComponent();
        _servicioFavor = servicioFavor;
        _servicioAuth = servicioAuth;
        RefrescarCommand = new Command(async () => await CargarDatosAsync());
        BindingContext = this;
    }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set { _isRefreshing = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRefreshing))); }
    }

    public bool IsCargando
    {
        get => _isCargando;
        set { _isCargando = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCargando))); }
    }

    public string MensajeError
    {
        get => _mensajeError;
        set { _mensajeError = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MensajeError))); PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HayError))); }
    }

    public bool HayError => !string.IsNullOrWhiteSpace(MensajeError);

    public bool HayDatos
    {
        get => _hayDatos;
        set { _hayDatos = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HayDatos))); PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SinDatos))); }
    }

    public bool SinDatos => !HayDatos && !IsCargando && !HayError;

    public ICommand RefrescarCommand { get; }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarDatosAsync();
    }

    private async Task CargarDatosAsync()
    {
        if (!SesionApp.Instancia.EstaLogueado)
        {
            MensajeError = "Debes iniciar sesión para ver los informes.";
            return;
        }

        try
        {
            IsRefreshing = true;
            IsCargando = true;
            MensajeError = string.Empty;

            var token = await _servicioAuth.ObtenerTokenAsync();
            _servicioFavor.AplicarToken(token);

            var disponibles = await _servicioFavor.ObtenerFavoresPorEstadoAsync("disponible");
            var aceptados = await _servicioFavor.ObtenerFavoresPorEstadoAsync("aceptado");
            var finalizados = await _servicioFavor.ObtenerFavoresPorEstadoAsync("finalizado");
            var cancelados = await _servicioFavor.ObtenerFavoresPorEstadoAsync("cancelado");

            lblDisponibles.Text = disponibles.Count.ToString();
            lblAceptados.Text = aceptados.Count.ToString();
            lblFinalizados.Text = finalizados.Count.ToString();
            lblCancelados.Text = cancelados.Count.ToString();

            var datos = new List<FavorEstadoDto>();
            if (disponibles.Count > 0)
                datos.Add(new FavorEstadoDto { Estado = ObtenerTexto("EstadoDisponible"), Cantidad = disponibles.Count });
            if (aceptados.Count > 0)
                datos.Add(new FavorEstadoDto { Estado = ObtenerTexto("EstadoAceptado"), Cantidad = aceptados.Count });
            if (finalizados.Count > 0)
                datos.Add(new FavorEstadoDto { Estado = ObtenerTexto("EstadoFinalizado"), Cantidad = finalizados.Count });
            if (cancelados.Count > 0)
                datos.Add(new FavorEstadoDto { Estado = ObtenerTexto("EstadoCancelado"), Cantidad = cancelados.Count });

            HayDatos = datos.Count > 0;

            if (HayDatos)
            {
                serieFavores.ItemsSource = datos;
                AplicarColoresGrafico();   // Asigna colores personalizados
                graficoFavores.IsVisible = true;
            }
            else
            {
                graficoFavores.IsVisible = false;
            }
        }
        catch (HttpRequestException ex)
        {
            MensajeError = string.IsNullOrWhiteSpace(ex.Message) ? "Error de conexión con el servidor." : ex.Message;
            HayDatos = false;
        }
        catch (Exception ex)
        {
            MensajeError = $"Error inesperado: {ex.Message}";
            HayDatos = false;
        }
        finally
        {
            IsCargando = false;
            IsRefreshing = false;
        }
    }

    // Asigna los colores personalizados a la serie
    private void AplicarColoresGrafico()
    {
        // PaletteBrushes es una colección de Brush que asigna colores en orden
        // a las porciones del gráfico (según el orden en ItemsSource)
        serieFavores.PaletteBrushes = new List<Brush>
        {
            new SolidColorBrush(Color.FromArgb("#10B981")), // Disponible → verde
            new SolidColorBrush(Color.FromArgb("#F59E0B")), // Aceptado → naranja
            new SolidColorBrush(Color.FromArgb("#6366F1")), // Finalizado → morado
            new SolidColorBrush(Color.FromArgb("#EF4444"))  // Cancelado → rojo
        };
    }

    private string ObtenerTexto(string clave) =>
        Application.Current?.Resources[clave]?.ToString() ?? clave;

    private class FavorEstadoDto
    {
        public string Estado { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }
}