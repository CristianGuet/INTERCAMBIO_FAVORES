using AppFavoresEscritorio.Modelos.Autenticacion;
using AppFavoresEscritorio.Servicios;
using AppFavoresEscritorio.Singleton;
using AppFavoresEscritorio.VistaModelos.Base;
using System.Windows.Input;

namespace AppFavoresEscritorio.VistaModelos.Autenticacion;

public class RegistroVistaModelo : VistaModeloBase
{
    private readonly ServicioAutenticacion _servicio;
    private readonly ConfiguracionApp _config;

    private string _nombreUsuario = string.Empty;
    private string _correo = string.Empty;
    private string _contrasenia = string.Empty;
    private string _confirmarContrasenia = string.Empty;

    public RegistroVistaModelo(ServicioAutenticacion servicio)
    {
        _servicio = servicio;
        _config = ConfiguracionApp.Instancia;
        Titulo = ObtenerTexto("Registro_Titulo");

        ComandoRegistrar = new Command(async () => await EjecutarRegistroAsync(), () => !EstaOcupado);
        ComandoVolverLogin = new Command(async () =>
            await Shell.Current.GoToAsync("//PaginaLogin"));
        ComandoCambiarTema = new Command(_config.AlternarTema);

        _config.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(ConfiguracionApp.EsTemaOscuro)
                or nameof(ConfiguracionApp.LogoImagen)
                or nameof(ConfiguracionApp.EtiquetaTemaActual))
            {
                Notificar(nameof(EsTemaOscuro));
                Notificar(nameof(LogoImagen));
                Notificar(nameof(IconoTema));
            }
        };
    }

    public ICommand ComandoRegistrar { get; }
    public ICommand ComandoVolverLogin { get; }
    public ICommand ComandoCambiarTema { get; }

    public string NombreUsuario
    {
        get => _nombreUsuario;
        set { _nombreUsuario = value; Notificar(); }
    }

    public string Correo
    {
        get => _correo;
        set { _correo = value; Notificar(); }
    }

    public string Contrasenia
    {
        get => _contrasenia;
        set { _contrasenia = value; Notificar(); }
    }

    public string ConfirmarContrasenia
    {
        get => _confirmarContrasenia;
        set { _confirmarContrasenia = value; Notificar(); }
    }

    public bool EsTemaOscuro => _config.EsTemaOscuro;
    public string IconoTema => _config.EtiquetaTemaActual;
    public string LogoImagen => _config.LogoImagen;

    private async Task EjecutarRegistroAsync()
    {
        LimpiarMensajes();

        if (!ValidarCampos()) return;

        try
        {
            EstaOcupado = true;
            (ComandoRegistrar as Command)?.ChangeCanExecute();

            var usuario = new UsuarioCreacion
            {
                NombreUsuario = NombreUsuario.Trim(),
                Correo = Correo.Trim(),
                Contrasenia = Contrasenia
            };

            await _servicio.RegistrarAsync(usuario);

            await Shell.Current.DisplayAlertAsync(
                ObtenerTexto("Exito_Registro"),
                ObtenerTexto("Exito_RegistroDetalle"),
                "OK");

            await Shell.Current.GoToAsync("//PaginaLogin");
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
            (ComandoRegistrar as Command)?.ChangeCanExecute();
        }
    }

    private bool ValidarCampos()
    {
        if (string.IsNullOrWhiteSpace(NombreUsuario)
            || string.IsNullOrWhiteSpace(Correo)
            || string.IsNullOrWhiteSpace(Contrasenia)
            || string.IsNullOrWhiteSpace(ConfirmarContrasenia))
        {
            MensajeError = ObtenerTexto("Error_CamposVacios");
            return false;
        }

        if (NombreUsuario.Trim().Length < 3)
        {
            MensajeError = ObtenerTexto("Error_NombreCorto");
            return false;
        }

        if (!Correo.Contains('@', StringComparison.Ordinal))
        {
            MensajeError = ObtenerTexto("Error_CorreoInvalido");
            return false;
        }

        if (Contrasenia.Length < 6)
        {
            MensajeError = ObtenerTexto("Error_ContraseniaCorta");
            return false;
        }

        if (Contrasenia != ConfirmarContrasenia)
        {
            MensajeError = ObtenerTexto("Error_ContraseniasNoCoinciden");
            return false;
        }

        return true;
    }
}
