using System.Text.Json.Serialization;

namespace AppFavoresEscritorio.Modelos.Autenticacion;

public class RespuestaLogin
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;

    [JsonPropertyName("usuario")]
    public UsuarioResumen? Usuario { get; set; }
}

public class UsuarioResumen
{
    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("correo")]
    public string Correo { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("rol")]
    public string Rol { get; set; } = string.Empty;
}
