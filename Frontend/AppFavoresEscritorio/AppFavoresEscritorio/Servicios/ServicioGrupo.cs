using System.Net.Http.Json;
using System.Text.Json;
using AppFavoresEscritorio.Modelos.Grupos;
using AppFavoresEscritorio.Servicios.Base;

namespace AppFavoresEscritorio.Servicios;

public class ServicioGrupo : ServicioApi
{
    private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

    public ServicioGrupo(HttpClient cliente) : base(cliente) { }

    public async Task<List<GrupoPublico>> ObtenerMisGruposAsync()
    {
        var r = await Cliente.GetAsync("grupos/mis-grupos");
        await ValidarRespuestaAsync(r);
        return await r.Content.ReadFromJsonAsync<List<GrupoPublico>>(_json) ?? [];
    }

    public async Task<List<GrupoBusqueda>> BuscarGruposAsync(string nombre)
    {
        var r = await Cliente.GetAsync($"grupos/buscar-grupos?nombre={Uri.EscapeDataString(nombre)}");
        await ValidarRespuestaAsync(r);
        return await r.Content.ReadFromJsonAsync<List<GrupoBusqueda>>(_json) ?? [];
    }

    public async Task<GrupoPublico?> CrearGrupoAsync(string nombreGrupo, string? descripcion = null)
    {
        var cuerpo = new { nombreGrupo, descripcion };
        var r = await Cliente.PostAsJsonAsync("grupos/crear", cuerpo, _json);
        await ValidarRespuestaAsync(r);
        return await r.Content.ReadFromJsonAsync<GrupoPublico>(_json);
    }

    public async Task UnirseGrupoAsync(string idGrupo)
    {
        var r = await Cliente.PostAsync($"grupos/{idGrupo}/unirse", null);
        await ValidarRespuestaAsync(r);
    }

    public async Task PedirInvitacionAsync(string idGrupo)
    {
        var r = await Cliente.PostAsJsonAsync($"grupos/{idGrupo}/pedir-invitacion",
            new { comentario = (string?)null }, _json);
        await ValidarRespuestaAsync(r);
    }

    public async Task SalirGrupoAsync(string idGrupo)
    {
        var r = await Cliente.PostAsync($"grupos/{idGrupo}/salir", null);
        await ValidarRespuestaAsync(r);
    }

    public async Task ExpulsarMiembroAsync(string idGrupo, string correoUsuario)
    {
        var r = await Cliente.PostAsync(
            $"grupos/{idGrupo}/expulsar?usuario_a_expulsar={Uri.EscapeDataString(correoUsuario)}", null);
        await ValidarRespuestaAsync(r);
    }

    public async Task CambiarRolAsync(string idGrupo, string correoMiembro, string nuevoRol)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch,
            $"grupos/{idGrupo}/cambiar-rol?correo_miembro={Uri.EscapeDataString(correoMiembro)}&nuevo_rol={nuevoRol}");
        var r = await Cliente.SendAsync(request);
        await ValidarRespuestaAsync(r);
    }

    public async Task EditarGrupoAsync(string idGrupo, string? nombreGrupo, string? descripcion, string? fotoGrupo)
    {
        var cuerpo = new { nombreGrupo, descripcion, fotoGrupo };
        var request = new HttpRequestMessage(HttpMethod.Patch, $"grupos/{idGrupo}/editar")
        {
            Content = JsonContent.Create(cuerpo)
        };
        var r = await Cliente.SendAsync(request);
        await ValidarRespuestaAsync(r);
    }

    public async Task AceptarSolicitudAsync(string idGrupo, string correoAspirante)
    {
        var r = await Cliente.PostAsync(
            $"grupos/{idGrupo}/aceptar-solicitud?correo_aspirante={Uri.EscapeDataString(correoAspirante)}", null);
        await ValidarRespuestaAsync(r);
    }

    public async Task RechazarSolicitudAsync(string idGrupo, string correoAspirante)
    {
        var r = await Cliente.PostAsync(
            $"grupos/{idGrupo}/rechazar-solicitud?correo_aspirante={Uri.EscapeDataString(correoAspirante)}", null);
        await ValidarRespuestaAsync(r);
    }
}
