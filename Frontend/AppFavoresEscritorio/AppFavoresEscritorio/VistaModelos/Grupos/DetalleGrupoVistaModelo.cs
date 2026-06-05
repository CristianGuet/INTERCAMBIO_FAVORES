using System.Collections.ObjectModel;
using System.Windows.Input;
using AppFavoresEscritorio.Modelos.Grupos;
using AppFavoresEscritorio.Servicios;
using AppFavoresEscritorio.Singleton;
using AppFavoresEscritorio.VistaModelos.Base;
 
namespace AppFavoresEscritorio.VistaModelos.Grupos;
 
public class DetalleGrupoVistaModelo : VistaModeloBase, IQueryAttributable
{
    private readonly ServicioGrupo _servicioGrupo;
    private readonly ServicioAutenticacion _servicioAuth;
 
    private string _grupoId    = string.Empty;
    private GrupoPublico? _grupo;
    private string _nombreGrupo = string.Empty;
    private string _descripcion  = string.Empty;
    private bool   _esAdmin;
 
    public DetalleGrupoVistaModelo(ServicioGrupo servicioGrupo, ServicioAutenticacion servicioAuth)
    {
        _servicioGrupo = servicioGrupo;
        _servicioAuth  = servicioAuth;
 
        Miembros = new ObservableCollection<MiembroGrupo>();
 
        ComandoSalir       = new Command(async () => await SalirGrupoAsync());
        ComandoExpulsar    = new Command<MiembroGrupo>(async m => await ExpulsarAsync(m));
        ComandoHacerAdmin  = new Command<MiembroGrupo>(async m => await CambiarRolAsync(m, "admin"));
        ComandoQuitarAdmin = new Command<MiembroGrupo>(async m => await CambiarRolAsync(m, "miembro"));
        ComandoAbrirChat   = new Command(async () => await AbrirChatGrupoAsync());
    }
 
    public ObservableCollection<MiembroGrupo> Miembros { get; }
 
    public string NombreGrupo
    {
        get => _nombreGrupo;
        set { _nombreGrupo = value; Notificar(); }
    }
 
    public string Descripcion
    {
        get => _descripcion;
        set { _descripcion = value; Notificar(); }
    }
 
    public bool EsAdmin
    {
        get => _esAdmin;
        set { _esAdmin = value; Notificar(); }
    }
 
    public string MiCorreo => SesionApp.Instancia.Usuario?.Correo ?? string.Empty;
 
    public ICommand ComandoSalir       { get; }
    public ICommand ComandoExpulsar    { get; }
    public ICommand ComandoHacerAdmin  { get; }
    public ICommand ComandoQuitarAdmin { get; }
    public ICommand ComandoAbrirChat   { get; }
 
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("grupoId", out var val))
            _grupoId = val?.ToString() ?? string.Empty;
    }
 
    public async Task CargarAsync()
    {
        if (string.IsNullOrEmpty(_grupoId)) return;
 
        try
        {
            EstaOcupado = true;
            LimpiarMensajes();
 
            var token = await _servicioAuth.ObtenerTokenAsync();
            if (string.IsNullOrWhiteSpace(token)) return;
 
            _servicioGrupo.AplicarToken(token);
 
            var grupos = await _servicioGrupo.ObtenerMisGruposAsync();
            _grupo = grupos.FirstOrDefault(g => g.Id == _grupoId);
 
            if (_grupo is null)
            {
                MensajeError = ObtenerTexto("Error_GrupoNoEncontrado");
                return;
            }
 
            NombreGrupo = _grupo.NombreGrupo;
            Descripcion  = _grupo.Descripcion;
            EsAdmin      = _grupo.EsAdminCorreo(MiCorreo);
 
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Miembros.Clear();
                foreach (var m in _grupo.Miembros)
                    Miembros.Add(m);
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
 
    private async Task SalirGrupoAsync()
    {
        if (_grupo is null) return;
 
        var confirmar = await Shell.Current.DisplayAlert(
            "Salir del grupo",
            $"¿Seguro que quieres salir de '{_grupo.NombreGrupo}'?",
            "Sí", "No");
 
        if (!confirmar) return;
 
        try
        {
            EstaOcupado = true;
            LimpiarMensajes();
 
            var token = await _servicioAuth.ObtenerTokenAsync();
            if (string.IsNullOrWhiteSpace(token)) return;
            _servicioGrupo.AplicarToken(token);
 
            await _servicioGrupo.SalirGrupoAsync(_grupoId);
 
            await Shell.Current.DisplayAlert("Grupo", "Has salido del grupo.", "Aceptar");
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
 
    private async Task ExpulsarAsync(MiembroGrupo? miembro)
    {
        if (miembro is null || _grupo is null) return;
 
        var confirmar = await Shell.Current.DisplayAlert(
            "Expulsar miembro",
            $"¿Expulsar a {miembro.CorreoUsuario}?",
            "Sí", "No");
 
        if (!confirmar) return;
 
        try
        {
            EstaOcupado = true;
            LimpiarMensajes();
 
            var token = await _servicioAuth.ObtenerTokenAsync();
            if (string.IsNullOrWhiteSpace(token)) return;
            _servicioGrupo.AplicarToken(token);
 
            await _servicioGrupo.ExpulsarMiembroAsync(_grupoId, miembro.CorreoUsuario);
            await CargarAsync();
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
 
    private async Task CambiarRolAsync(MiembroGrupo? miembro, string nuevoRol)
    {
        if (miembro is null || _grupo is null) return;
 
        if (nuevoRol == "miembro")
        {
            var ok = await Shell.Current.DisplayAlert(
                "Cambiar rol",
                $"¿Quitar permisos de admin a {miembro.CorreoUsuario}?",
                "Sí", "No");
            if (!ok) return;
        }
 
        try
        {
            EstaOcupado = true;
            LimpiarMensajes();
 
            var token = await _servicioAuth.ObtenerTokenAsync();
            if (string.IsNullOrWhiteSpace(token)) return;
            _servicioGrupo.AplicarToken(token);
 
            await _servicioGrupo.CambiarRolAsync(_grupoId, miembro.CorreoUsuario, nuevoRol);
            await CargarAsync();
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
 
    private async Task AbrirChatGrupoAsync()
    {
        if (_grupo is null) return;
        await Shell.Current.GoToAsync(
            $"detallechatgrupo?grupoId={Uri.EscapeDataString(_grupoId)}");
    }
}