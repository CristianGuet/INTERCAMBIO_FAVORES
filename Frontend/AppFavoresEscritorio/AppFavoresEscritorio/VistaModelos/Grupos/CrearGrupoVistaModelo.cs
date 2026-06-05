using System.Windows.Input;
using AppFavoresEscritorio.Servicios;
using AppFavoresEscritorio.VistaModelos.Base;
 
namespace AppFavoresEscritorio.VistaModelos.Grupos;
 
public class CrearGrupoVistaModelo : VistaModeloBase
{
    private readonly ServicioGrupo _servicioGrupo;
    private readonly ServicioAutenticacion _servicioAuth;
 
    private string _nombre = string.Empty;
    private string _descripcion = string.Empty;
 
    public CrearGrupoVistaModelo(ServicioGrupo servicioGrupo, ServicioAutenticacion servicioAuth)
    {
        _servicioGrupo = servicioGrupo;
        _servicioAuth = servicioAuth;

        ComandoCrear    = new Command(async () => await CrearAsync());
        ComandoCancelar = new Command(async () => await Shell.Current.GoToAsync(".."));
    }
 
    public string Nombre
    {
        get => _nombre;
        set { _nombre = value; Notificar(); }
    }
 
    public string Descripcion
    {
        get => _descripcion;
        set { _descripcion = value; Notificar(); }
    }
 
    public ICommand ComandoCrear    { get; }
    public ICommand ComandoCancelar { get; }
 
    private async Task CrearAsync()
    {
        MensajeError = string.Empty;
 
        if (Nombre.Trim().Length < 3)
        {
            MensajeError = ObtenerTexto("Error_NombreCorto");
            return;
        }
 
        try
        {
            EstaOcupado = true;
            var token = await _servicioAuth.ObtenerTokenAsync();
            if (string.IsNullOrWhiteSpace(token)) return; // FIX: guard sin sesión
            _servicioGrupo.AplicarToken(token);
            await _servicioGrupo.CrearGrupoAsync(Nombre.Trim(), Descripcion.Trim());
            await Shell.Current.DisplayAlertAsync(
                ObtenerTexto("CrearGrupo"),
                "Grupo creado correctamente",
                "Aceptar");
            await Shell.Current.GoToAsync("..");
        }
        catch (HttpRequestException ex) { MensajeError = ex.Message; }
        catch (Exception) { MensajeError = ObtenerTexto("Error_Generico"); }
        finally { EstaOcupado = false; }
    }
}