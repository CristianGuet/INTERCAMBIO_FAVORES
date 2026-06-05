using System.Windows.Input;
using AppFavoresEscritorio.Servicios;
using AppFavoresEscritorio.VistaModelos.Base;
 
namespace AppFavoresEscritorio.VistaModelos.Perfil;
 
public class PerfilVistaModelo : VistaModeloBase
{
    private readonly ServicioPerfil _servicioPerfil;
    private readonly ServicioAutenticacion _servicioAuth;
 
    private string _nombreUsuario = string.Empty;
    private string _correo = string.Empty;
    private string _descripcion = string.Empty;
    private string _fotoPerfil = string.Empty;
    private float? _calificacionMedia;
    private string _rol = string.Empty;
    private bool _modoEdicion;
    private string _nombreEdicion = string.Empty;
    private string _descripcionEdicion = string.Empty;
 
    public PerfilVistaModelo(ServicioPerfil servicioPerfil, ServicioAutenticacion servicioAuth)
    {
        _servicioPerfil = servicioPerfil;
        _servicioAuth = servicioAuth;
 
        ComandoActivarEdicion = new Command(ActivarEdicion);
        ComandoGuardar = new Command(async () => await GuardarAsync());
        ComandoCancelar = new Command(() => ModoEdicion = false);
    }
 
    public string NombreUsuario
    {
        get => _nombreUsuario;
        private set { _nombreUsuario = value; Notificar(); }
    }
 
    public string Correo
    {
        get => _correo;
        private set { _correo = value; Notificar(); }
    }
 
    public string Descripcion
    {
        get => _descripcion;
        private set { _descripcion = value; Notificar(); }
    }
 
    public string FotoPerfil
    {
        get => _fotoPerfil;
        private set { _fotoPerfil = value; Notificar(); }
    }
 
    public float? CalificacionMedia
    {
        get => _calificacionMedia;
        private set { _calificacionMedia = value; Notificar(); Notificar(nameof(TextoCalificacion)); }
    }
 
    public string TextoCalificacion =>
        CalificacionMedia.HasValue
            ? $"Calificacion: {CalificacionMedia:0.0} / 5"
            : "Calificacion: sin valorar";
 
    public string Rol
    {
        get => _rol;
        private set { _rol = value; Notificar(); }
    }
 
    public bool ModoEdicion
    {
        get => _modoEdicion;
        set { _modoEdicion = value; Notificar(); }
    }
 
    public string NombreEdicion
    {
        get => _nombreEdicion;
        set { _nombreEdicion = value; Notificar(); }
    }
 
    public string DescripcionEdicion
    {
        get => _descripcionEdicion;
        set { _descripcionEdicion = value; Notificar(); }
    }
 
    public ICommand ComandoActivarEdicion { get; }
    public ICommand ComandoGuardar { get; }
    public ICommand ComandoCancelar { get; }
 
    private void ActivarEdicion()
    {
        NombreEdicion = NombreUsuario;
        DescripcionEdicion = Descripcion;
        ModoEdicion = true;
    }
 
    public async Task CargarPerfilAsync()
    {
        try
        {
            EstaOcupado = true;
            LimpiarMensajes();
 
            var token = await _servicioAuth.ObtenerTokenAsync();
            if (string.IsNullOrWhiteSpace(token)) return;
 
            _servicioPerfil.AplicarToken(token);
 
            var perfil = await _servicioPerfil.ObtenerMiPerfilAsync();
            if (perfil is null) return;
 
            NombreUsuario = perfil.NombreUsuario;
            Correo = perfil.Correo;
            Descripcion = perfil.Descripcion;
            FotoPerfil = string.IsNullOrWhiteSpace(perfil.FotoPerfil) ? "default.png" : perfil.FotoPerfil;
            CalificacionMedia = perfil.CalificacionMedia;
            Rol = perfil.Rol;
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
 
    private async Task GuardarAsync()
    {
        try
        {
            EstaOcupado = true;
            LimpiarMensajes();
 
            var token = await _servicioAuth.ObtenerTokenAsync();
            if (string.IsNullOrWhiteSpace(token)) return;
            _servicioPerfil.AplicarToken(token);
 
            await _servicioPerfil.EditarPerfilAsync(NombreEdicion, DescripcionEdicion);
            ModoEdicion = false;
            await CargarPerfilAsync();
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