using AppFavoresEscritorio.Navegacion;
using AppFavoresEscritorio.Servicios;
using AppFavoresEscritorio.Singleton;
using AppFavoresEscritorio.VistaModelos.Base;
using System.Windows.Input;

namespace AppFavoresEscritorio.VistaModelos.Autenticacion;

public class LoginVistaModelo : VistaModeloBase
{
    private readonly ServicioAutenticacion _servicio;
    private readonly ConfiguracionApp _config;

    private string _correo = string.Empty;
    private string _contrasenia = string.Empty;

    public LoginVistaModelo(ServicioAutenticacion servicio)
    {
        _servicio = servicio;
        _config = ConfiguracionApp.Instancia;
        Titulo = ObtenerTexto("App_Nombre");

        ComandoLogin = new Command(async () => await EjecutarLoginAsync(), () => !EstaOcupado);
        ComandoIrRegistro = new Command(async () =>
            await Shell.Current.GoToAsync("//PaginaRegistro"));
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

    public ICommand ComandoLogin { get; }
    public ICommand ComandoIrRegistro { get; }
    public ICommand ComandoCambiarTema { get; }

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

    public bool EsTemaOscuro => _config.EsTemaOscuro;
    public string IconoTema => _config.EtiquetaTemaActual;
    public string LogoImagen => _config.LogoImagen;

    private async Task EjecutarLoginAsync()
    {
        LimpiarMensajes();

        if (string.IsNullOrWhiteSpace(Correo) || string.IsNullOrWhiteSpace(Contrasenia))
        {
            MensajeError = ObtenerTexto("Error_CamposVacios");
            return;
        }

        try
        {
            EstaOcupado = true;
            (ComandoLogin as Command)?.ChangeCanExecute();

            var respuesta = await _servicio.LoginAsync(Correo, Contrasenia);
            if (respuesta.Usuario is null)
            {
                MensajeError = ObtenerTexto("Error_Generico");
                return;
            }

            if (Shell.Current is AppShell shell)
                await shell.MostrarMenuTrasLogin(respuesta.Usuario);
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
            (ComandoLogin as Command)?.ChangeCanExecute();
        }
    }
}
