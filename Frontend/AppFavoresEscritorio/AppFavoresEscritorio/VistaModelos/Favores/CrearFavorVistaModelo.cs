using System.Windows.Input;
using AppFavoresEscritorio.Modelos.Favores;
using AppFavoresEscritorio.Servicios;
using AppFavoresEscritorio.VistaModelos.Base;

namespace AppFavoresEscritorio.VistaModelos.Favores;

public class CrearFavorVistaModelo : VistaModeloBase
{
    private readonly ServicioFavor _servicioFavor;
    private readonly ServicioAutenticacion _servicioAuth;

    private string _titulo = string.Empty;
    private string _descripcion = string.Empty;
    private bool _esPresencial;
    private string? _ubicacion;
    private int _indexCompensacion; // 0=favor, 1=dinero, 2=ambos
    private string _cantidadDineroTexto = string.Empty;
    private string? _recompensaFavor;

    public CrearFavorVistaModelo(ServicioFavor servicioFavor, ServicioAutenticacion servicioAuth)
    {
        _servicioFavor = servicioFavor;
        _servicioAuth = servicioAuth;

        ComandoCrear   = new Command(async () => await PublicarAsync());
        ComandoCancelar = new Command(async () => await Shell.Current.GoToAsync(".."));
    }

    public string Titulo
    {
        get => _titulo;
        set { _titulo = value; Notificar(); }
    }

    public string Descripcion
    {
        get => _descripcion;
        set { _descripcion = value; Notificar(); }
    }

    public bool EsPresencial
    {
        get => _esPresencial;
        set
        {
            _esPresencial = value;
            Notificar();
            Notificar(nameof(MostrarUbicacion));
        }
    }

    public bool MostrarUbicacion => _esPresencial;

    public string? Ubicacion
    {
        get => _ubicacion;
        set { _ubicacion = value; Notificar(); }
    }

    public int IndexCompensacion
    {
        get => _indexCompensacion;
        set
        {
            _indexCompensacion = value;
            Notificar();
            Notificar(nameof(MostrarCamposDinero));
            Notificar(nameof(MostrarCamposFavor));
        }
    }

    // 0=favor → false, 1=dinero → true, 2=ambos → true
    public bool MostrarCamposDinero => _indexCompensacion == 1 || _indexCompensacion == 2;

    // 0=favor → true, 1=dinero → false, 2=ambos → true
    public bool MostrarCamposFavor => _indexCompensacion == 0 || _indexCompensacion == 2;

    public string CantidadDineroTexto
    {
        get => _cantidadDineroTexto;
        set { _cantidadDineroTexto = value; Notificar(); }
    }

    public string? RecompensaFavor
    {
        get => _recompensaFavor;
        set { _recompensaFavor = value; Notificar(); }
    }

    public ICommand ComandoCrear    { get; }
    public ICommand ComandoCancelar { get; }

    private string TipoCompensacion => _indexCompensacion switch
    {
        1 => "dinero",
        2 => "ambos",
        _ => "favor"
    };

    private async Task PublicarAsync()
    {
        LimpiarMensajes();

        if (string.IsNullOrWhiteSpace(Titulo) || Titulo.Trim().Length < 5)
        {
            MensajeError = "El título debe tener al menos 5 caracteres.";
            return;
        }
        if (string.IsNullOrWhiteSpace(Descripcion) || Descripcion.Trim().Length < 10)
        {
            MensajeError = "La descripción debe tener al menos 10 caracteres.";
            return;
        }
        if (EsPresencial && string.IsNullOrWhiteSpace(Ubicacion))
        {
            MensajeError = "Debes indicar la ubicación para un favor presencial.";
            return;
        }

        float? cantidadDinero = null;
        if (MostrarCamposDinero)
        {
            if (!float.TryParse(CantidadDineroTexto, out var val) || val <= 0)
            {
                MensajeError = "Introduce una cantidad de dinero válida mayor a 0.";
                return;
            }
            cantidadDinero = val;
        }

        try
        {
            EstaOcupado = true;

            var token = await _servicioAuth.ObtenerTokenAsync();
            _servicioFavor.AplicarToken(token);

            var favor = new FavorCreacion
            {
                Titulo            = Titulo.Trim(),
                Descripcion       = Descripcion.Trim(),
                Modalidad         = EsPresencial ? "presencial" : "remoto",
                Ubicacion         = EsPresencial ? Ubicacion?.Trim() : null,
                TipoCompensacion  = TipoCompensacion,
                CantidadDinero    = cantidadDinero,
                RecompensaFavor   = MostrarCamposFavor ? RecompensaFavor?.Trim() : null
            };

            await _servicioFavor.CrearFavorAsync(favor);

            await Shell.Current.DisplayAlert(
                "Favor publicado",
                "Tu favor se ha publicado correctamente.",
                "Aceptar");

            await Shell.Current.GoToAsync("..");
        }
        catch (HttpRequestException ex)
        {
            MensajeError = string.IsNullOrWhiteSpace(ex.Message)
                ? ObtenerTexto("Error_Generico")
                : ex.Message;
        }
        catch (Exception)
        {
            MensajeError = ObtenerTexto("Error_Generico");
        }
        finally
        {
            EstaOcupado = false;
        }
    }
}
