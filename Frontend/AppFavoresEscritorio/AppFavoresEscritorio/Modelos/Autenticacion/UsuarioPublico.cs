using System.Text.Json.Serialization;

namespace AppFavoresEscritorio.Modelos.Autenticacion;

public class UsuarioPublico
{
    [JsonPropertyName("_id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("correo")]
    public string Correo { get; set; } = string.Empty;

    [JsonPropertyName("nombreUsuario")]
    public string NombreUsuario { get; set; } = string.Empty;

    [JsonPropertyName("descripcion")]
    public string Descripcion { get; set; } = string.Empty;

    [JsonPropertyName("fotoPerfil")]
    public string FotoPerfil { get; set; } = string.Empty;

    [JsonPropertyName("calificacionMedia")]
    public float? CalificacionMedia { get; set; }

    [JsonPropertyName("rol")]
    public string Rol { get; set; } = "Usuario";
}
