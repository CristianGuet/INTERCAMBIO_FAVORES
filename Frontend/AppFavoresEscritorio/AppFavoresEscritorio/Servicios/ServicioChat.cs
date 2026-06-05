using System.Net.Http.Json;
using System.Text.Json;
using AppFavoresEscritorio.Modelos.Chats;
using AppFavoresEscritorio.Servicios.Base;

namespace AppFavoresEscritorio.Servicios;

public class ServicioChat : ServicioApi
{
    private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

    public ServicioChat(HttpClient cliente) : base(cliente) { }

    /// <summary>Obtiene todos los chats del usuario autenticado.</summary>
    public async Task<List<ChatPublico>> ObtenerMisChatsAsync()
    {
        var r = await Cliente.GetAsync("chat/mis-chats");
        await ValidarRespuestaAsync(r);
        return await r.Content.ReadFromJsonAsync<List<ChatPublico>>(_json) ?? [];
    }

    /// <summary>Obtiene o crea un chat privado entre dos usuarios.</summary>
    public async Task<ChatPublico?> ObtenerOCrearChatPrivadoAsync(string correoOtro)
    {
        var r = await Cliente.PostAsJsonAsync("chat/obtener-o-crear_chat",
            new { participantes = new[] { correoOtro } }, _json);
        await ValidarRespuestaAsync(r);
        return await r.Content.ReadFromJsonAsync<ChatPublico>(_json);
    }

    /// <summary>Obtiene o crea un chat grupal a partir de una lista de participantes.</summary>
    public async Task<ChatPublico?> ObtenerOCrearChatGrupoAsync(List<string> participantes)
    {
        var r = await Cliente.PostAsJsonAsync("chat/obtener-o-crear_chat",
            new { participantes }, _json);
        await ValidarRespuestaAsync(r);
        return await r.Content.ReadFromJsonAsync<ChatPublico>(_json);
    }

    public async Task<MensajePublico?> EnviarMensajeAsync(string idChat, string contenido)
    {
        var r = await Cliente.PostAsJsonAsync($"chat/{idChat}/enviar",
            new { contenido }, _json);
        await ValidarRespuestaAsync(r);
        return await r.Content.ReadFromJsonAsync<MensajePublico>(_json);
    }

    public async Task MarcarLeidoAsync(string idChat)
    {
        var r = await Cliente.PutAsync($"chat/{idChat}/leer-chats", null);
        await ValidarRespuestaAsync(r);
    }
}
