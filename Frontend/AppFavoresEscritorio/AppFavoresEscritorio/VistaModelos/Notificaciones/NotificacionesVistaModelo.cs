using System.Collections.ObjectModel;
using System.Windows.Input;
using AppFavoresEscritorio.Modelos.Notificaciones;
using AppFavoresEscritorio.Servicios;
using AppFavoresEscritorio.VistaModelos.Base;
 
namespace AppFavoresEscritorio.VistaModelos.Notificaciones;
 
public class NotificacionesVistaModelo : VistaModeloBase
{
    private readonly ServicioNotificacion _servicio;
 
    public NotificacionesVistaModelo(ServicioNotificacion servicio)
    {
        _servicio = servicio;
        Notificaciones = new ObservableCollection<Modelos.Notificaciones.Notificacion>();
 
        ComandoMarcarTodasLeidas = new Command(async () => await MarcarTodasLeidasAsync());
        ComandoRefrescar = new Command(async () => await CargarAsync());
    }
 
    public ObservableCollection<Modelos.Notificaciones.Notificacion> Notificaciones { get; }
 
    public int NoLeidas => Notificaciones.Count(n => !n.Leida);
 
    public bool HayNoLeidas => NoLeidas > 0;
 
    public ICommand ComandoMarcarTodasLeidas { get; }
    public ICommand ComandoRefrescar { get; }
 
    public async Task CargarAsync()
    {
        try
        {
            EstaOcupado = true;
            LimpiarMensajes();
 
            var token = await SecureStorage.Default.GetAsync("token_acceso");
            if (string.IsNullOrWhiteSpace(token)) return;
 
            _servicio.AplicarToken(token);
 
            var lista = await _servicio.ObtenerNotificacionesAsync();
 
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Notificaciones.Clear();
                foreach (var n in lista)
                    Notificaciones.Add(n);
                Notificar(nameof(NoLeidas));
                Notificar(nameof(HayNoLeidas));
            });
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
 
    private async Task MarcarTodasLeidasAsync()
    {
        try
        {
            EstaOcupado = true;
            var token = await SecureStorage.Default.GetAsync("token_acceso");
            if (string.IsNullOrWhiteSpace(token)) return;
            _servicio.AplicarToken(token);
            await _servicio.MarcarTodasLeidasAsync();
            await CargarAsync();
        }
        catch (HttpRequestException ex)
        {
            MensajeError = string.IsNullOrWhiteSpace(ex.Message)
                ? ObtenerTexto("Error_Generico")
                : ex.Message;
        }
        finally
        {
            EstaOcupado = false;
        }
    }
}