using System.Net.Http.Json;
using System.Text.Json;
using AppFavoresEscritorio.Modelos.Autenticacion;
using AppFavoresEscritorio.Servicios.Base;

namespace AppFavoresEscritorio.Servicios;

public class ServicioPerfil : ServicioApi
{
    private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

    public ServicioPerfil(HttpClient cliente) : base(cliente) { }

    public async Task<UsuarioPublico?> ObtenerMiPerfilAsync()
    {
        var r = await Cliente.GetAsync("perfil/mis-datos");
        await ValidarRespuestaAsync(r);
        return await r.Content.ReadFromJsonAsync<UsuarioPublico>(_json);
    }

    public async Task<UsuarioPublico?> EditarPerfilAsync(string? nombreUsuario, string? descripcion)
    {
        var cuerpo = new Dictionary<string, string?>();
        if (!string.IsNullOrWhiteSpace(nombreUsuario)) cuerpo["nombreUsuario"] = nombreUsuario;
        if (!string.IsNullOrWhiteSpace(descripcion)) cuerpo["descripcion"] = descripcion;

        var r = await Cliente.PutAsJsonAsync("perfil/editar", cuerpo, _json);
        await ValidarRespuestaAsync(r);
        return await r.Content.ReadFromJsonAsync<UsuarioPublico>(_json);
    }
}
