using System.Collections.ObjectModel;
using System.Windows.Input;
using AppFavoresEscritorio.Modelos.Favores;
using AppFavoresEscritorio.Servicios;
using AppFavoresEscritorio.Singleton;
using AppFavoresEscritorio.VistaModelos.Base;
 
namespace AppFavoresEscritorio.VistaModelos.Favores;
 
public class FavoresDisponiblesVistaModelo : VistaModeloBase
{
    private readonly ServicioFavor _servicioFavor;
    private readonly ServicioAutenticacion _servicioAuth;
    private bool _isRefreshing;
 
    public FavoresDisponiblesVistaModelo(ServicioFavor servicioFavor, ServicioAutenticacion servicioAuth)
    {
        _servicioFavor = servicioFavor;
        _servicioAuth = servicioAuth;
        Favores = new ObservableCollection<FavorPublico>();
 
        ComandoAceptarFavor = new Command<FavorPublico>(async f => await AceptarFavorAsync(f));
        ComandoRefrescar = new Command(async () => await CargarFavoresAsync());
    }
 
    public ObservableCollection<FavorPublico> Favores { get; }
 
    public string MiCorreo => SesionApp.Instancia.Usuario?.Correo ?? string.Empty;
 
    public bool IsRefreshing
    {
        get => _isRefreshing;
        set { _isRefreshing = value; Notificar(); }
    }
 
    public ICommand ComandoAceptarFavor { get; }
    public ICommand ComandoRefrescar { get; }
 
    public async Task CargarFavoresAsync()
    {
        try
        {
            IsRefreshing = true;
            LimpiarMensajes();
 
            var token = await _servicioAuth.ObtenerTokenAsync();
            if (string.IsNullOrWhiteSpace(token)) return;
 
            _servicioFavor.AplicarToken(token);
 
            var lista = await _servicioFavor.ObtenerFavoresDisponiblesAsync();
 
            foreach (var f in lista)
                f.PuedeAceptar = !string.Equals(f.IdUsuarioOfrece, MiCorreo, StringComparison.OrdinalIgnoreCase);
 
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Favores.Clear();
                foreach (var f in lista)
                    Favores.Add(f);
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
            IsRefreshing = false;
        }
    }
 
    private async Task AceptarFavorAsync(FavorPublico? favor)
    {
        if (favor is null) return;
 
        if (string.Equals(favor.IdUsuarioOfrece, MiCorreo, StringComparison.OrdinalIgnoreCase))
        {
            MensajeError = "No puedes aceptar un favor que publicaste tu mismo.";
            return;
        }
 
        try
        {
            EstaOcupado = true;
            LimpiarMensajes();
 
            var token = await _servicioAuth.ObtenerTokenAsync();
            if (string.IsNullOrWhiteSpace(token)) return;
 
            _servicioFavor.AplicarToken(token);
 
            await _servicioFavor.AceptarFavorAsync(favor.Id);
            await CargarFavoresAsync();
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