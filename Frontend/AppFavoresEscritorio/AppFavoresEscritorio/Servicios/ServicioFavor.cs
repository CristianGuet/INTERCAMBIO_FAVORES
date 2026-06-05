using System.Net.Http.Json;
using System.Text.Json;
using AppFavoresEscritorio.Modelos.Favores;
using AppFavoresEscritorio.Servicios.Base;
using AppFavoresEscritorio.Singleton;

namespace AppFavoresEscritorio.Servicios;

public class ServicioFavor : ServicioApi
{
    private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

    public ServicioFavor(HttpClient cliente) : base(cliente) { }

    public async Task<List<FavorPublico>> ObtenerFavoresDisponiblesAsync()
    {
        var r = await Cliente.GetAsync("favores/lista?estado=disponible");
        await ValidarRespuestaAsync(r);
        return await r.Content.ReadFromJsonAsync<List<FavorPublico>>(_json) ?? [];
    }

    // PASO 11: la API no tiene un endpoint /mis-publicados; filtramos localmente
    public async Task<List<FavorPublico>> ObtenerMisFavoresPublicadosAsync()
    {
        var r = await Cliente.GetAsync("favores/lista?estado=disponible");
        await ValidarRespuestaAsync(r);
        var todos = await r.Content.ReadFromJsonAsync<List<FavorPublico>>(_json) ?? [];
        var miCorreo = SesionApp.Instancia.Usuario?.Correo ?? string.Empty;
        return todos
            .Where(f => string.Equals(f.IdUsuarioOfrece, miCorreo, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public async Task<List<FavorPublico>> ObtenerFavoresPorEstadoAsync(string estado)
    {
        var r = await Cliente.GetAsync($"favores/lista?estado={estado}");
        await ValidarRespuestaAsync(r);
        return await r.Content.ReadFromJsonAsync<List<FavorPublico>>(_json) ?? [];
    }

    public async Task<FavorPublico?> CrearFavorAsync(FavorCreacion favor)
    {
        var r = await Cliente.PostAsJsonAsync("favores/crear", favor, _json);
        await ValidarRespuestaAsync(r);
        return await r.Content.ReadFromJsonAsync<FavorPublico>(_json);
    }

    public async Task AceptarFavorAsync(string idFavor)
    {
        var r = await Cliente.PutAsync($"favores/{idFavor}/aceptar", null);
        await ValidarRespuestaAsync(r);
    }

    public async Task CancelarFavorAsync(string idFavor)
    {
        var r = await Cliente.PutAsync($"favores/{idFavor}/cancelar", null);
        await ValidarRespuestaAsync(r);
    }

    public async Task FinalizarFavorAsync(string idFavor)
    {
        var r = await Cliente.PutAsync($"favores/{idFavor}/finalizar", null);
        await ValidarRespuestaAsync(r);
    }

    public async Task EliminarFavorAsync(string idFavor)
    {
        var r = await Cliente.DeleteAsync($"favores/{idFavor}/eliminar");
        await ValidarRespuestaAsync(r);
    }
}
