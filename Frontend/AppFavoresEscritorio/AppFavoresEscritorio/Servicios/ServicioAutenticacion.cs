using System.Net.Http.Json;
using System.Text.Json;
using AppFavoresEscritorio.Modelos.Autenticacion;
using AppFavoresEscritorio.Servicios.Base;
using AppFavoresEscritorio.Singleton;

namespace AppFavoresEscritorio.Servicios;

public class ServicioAutenticacion : ServicioApi
{
    private const string ClaveToken = "token_acceso";

    private static readonly JsonSerializerOptions JsonOpciones = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ServicioAutenticacion(HttpClient cliente) : base(cliente) { }

    public async Task<RespuestaLogin> LoginAsync(string correo, string contrasenia)
    {
        var contenido = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["username"] = correo.Trim().ToLowerInvariant(),
            ["password"] = contrasenia
        });

        var respuesta = await Cliente.PostAsync("usuarios/login", contenido);
        await ValidarRespuestaAsync(respuesta);

        var datos = await respuesta.Content.ReadFromJsonAsync<RespuestaLogin>(JsonOpciones)
            ?? throw new InvalidOperationException("Respuesta de login vacía.");

        await GuardarSesionAsync(datos);
        return datos;
    }

    public async Task<UsuarioPublico> RegistrarAsync(UsuarioCreacion usuario)
    {
        usuario.Correo = usuario.Correo.Trim().ToLowerInvariant();
        var respuesta = await Cliente.PostAsJsonAsync("usuarios/registro", usuario, JsonOpciones);
        await ValidarRespuestaAsync(respuesta);

        return await respuesta.Content.ReadFromJsonAsync<UsuarioPublico>(JsonOpciones)
            ?? throw new InvalidOperationException("Respuesta de registro vacía.");
    }

    public async Task GuardarSesionAsync(RespuestaLogin datos)
    {
        await SecureStorage.Default.SetAsync(ClaveToken, datos.AccessToken);
        AplicarToken(datos.AccessToken);
        SesionApp.Instancia.EstablecerSesion(datos);
    }

    public async Task<string?> ObtenerTokenAsync() =>
        await SecureStorage.Default.GetAsync(ClaveToken);

    public async Task CargarSesionGuardadaAsync()
    {
        var token = await ObtenerTokenAsync();
        if (string.IsNullOrWhiteSpace(token)) return;

        AplicarToken(token);
        SesionApp.Instancia.EstablecerSesion(new RespuestaLogin { AccessToken = token });
    }

    public async Task CerrarSesionAsync()
    {
        try
        {
            SecureStorage.Default.RemoveAll();
        }
        catch { /* En Windows a veces falla con DPAPI; ignorar */ }
        LimpiarToken();
        await Task.CompletedTask;
    }

    private void LimpiarToken() => AplicarToken(null);
}
