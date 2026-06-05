using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AppFavoresEscritorio.Servicios.Base;

public abstract class ServicioApi
{
    protected readonly HttpClient Cliente;

    private const string UrlBase = "http://localhost:8000/"; //meter en el .env en el futuro

    protected ServicioApi(HttpClient cliente)
    {
        Cliente = cliente;
        Cliente.BaseAddress = new Uri(UrlBase);
    }

    public void AplicarToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            Cliente.DefaultRequestHeaders.Authorization = null;
        else
            Cliente.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Valida la respuesta HTTP. Si no es exitosa, lanza HttpRequestException
    /// con el mensaje de error devuelto por la API (campo "detail" de FastAPI).
    /// </summary>
    protected static async Task ValidarRespuestaAsync(HttpResponseMessage respuesta)
    {
        if (respuesta.IsSuccessStatusCode)
            return;

        string? detalle = null;
        try
        {
            var json = await respuesta.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            if (json is not null && json.TryGetValue("detail", out var d))
                detalle = d?.ToString();
        }
        catch { /* ignorar errores de parsing */ }

        throw new HttpRequestException(
            detalle ?? $"Error {(int)respuesta.StatusCode}: {respuesta.ReasonPhrase}",
            null,
            respuesta.StatusCode);
    }
}
