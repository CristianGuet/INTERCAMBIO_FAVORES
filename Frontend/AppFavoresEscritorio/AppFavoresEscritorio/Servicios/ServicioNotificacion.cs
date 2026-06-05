using System.Net.Http.Json;
using System.Text.Json;
using AppFavoresEscritorio.Modelos.Notificaciones;
using AppFavoresEscritorio.Servicios.Base;

namespace AppFavoresEscritorio.Servicios;

public class ServicioNotificacion : ServicioApi
{
    private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

    public ServicioNotificacion(HttpClient cliente) : base(cliente) { }

    public async Task<List<Notificacion>> ObtenerNotificacionesAsync()
    {
        var r = await Cliente.GetAsync("notificaciones/mis-notificaciones");
        await ValidarRespuestaAsync(r);
        return await r.Content.ReadFromJsonAsync<List<Notificacion>>(_json) ?? [];
    }

    public async Task MarcarTodasLeidasAsync()
    {
        var r = await Cliente.PostAsync("notificaciones/marcar-leida", null);
        await ValidarRespuestaAsync(r);
    }
}
