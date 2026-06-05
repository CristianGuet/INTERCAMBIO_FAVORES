using System.Net.Http.Json;
using System.Text.Json;
using AppFavoresEscritorio.Modelos.Autenticacion;
using AppFavoresEscritorio.Modelos.Favores;
using AppFavoresEscritorio.Modelos.Grupos;
using AppFavoresEscritorio.Servicios.Base;

namespace AppFavoresEscritorio.Servicios;

public class ServicioAdmin : ServicioApi
{
    private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

    public ServicioAdmin(HttpClient cliente) : base(cliente) { }

    // Usuarios
    public async Task<List<UsuarioPublico>> ObtenerTodosUsuariosAsync()
    {
        var r = await Cliente.GetAsync("admin/usuarios");
        await ValidarRespuestaAsync(r);
        return await r.Content.ReadFromJsonAsync<List<UsuarioPublico>>(_json) ?? [];
    }

    public async Task EliminarUsuarioAsync(string correo)
    {
        var r = await Cliente.DeleteAsync($"admin/usuarios/{Uri.EscapeDataString(correo)}");
        await ValidarRespuestaAsync(r);
    }

    public async Task CambiarRolUsuarioAsync(string correo, string nuevoRol)
    {
        var contenido = new { nuevo_rol = nuevoRol };
        var r = await Cliente.PatchAsJsonAsync($"admin/usuarios/{Uri.EscapeDataString(correo)}/rol", contenido, _json);
        await ValidarRespuestaAsync(r);
    }

    // Favores
    public async Task<List<FavorPublico>> ObtenerTodosFavoresAsync()
    {
        var r = await Cliente.GetAsync("admin/favores");
        await ValidarRespuestaAsync(r);
        return await r.Content.ReadFromJsonAsync<List<FavorPublico>>(_json) ?? [];
    }

    public async Task EliminarFavorAsync(string idFavor)
    {
        var r = await Cliente.DeleteAsync($"admin/favores/{idFavor}");
        await ValidarRespuestaAsync(r);
    }

    // Grupos
    public async Task<List<GrupoPublico>> ObtenerTodosGruposAsync()
    {
        var r = await Cliente.GetAsync("admin/grupos");
        await ValidarRespuestaAsync(r);
        return await r.Content.ReadFromJsonAsync<List<GrupoPublico>>(_json) ?? [];
    }

    public async Task EliminarGrupoAsync(string idGrupo)
    {
        var r = await Cliente.DeleteAsync($"admin/grupos/{idGrupo}");
        await ValidarRespuestaAsync(r);
    }
}