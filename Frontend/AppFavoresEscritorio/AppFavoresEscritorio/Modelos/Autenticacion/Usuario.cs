using System.Text.Json.Serialization;

namespace AppFavoresEscritorio.Modelos.Autenticacion;

public class UsuarioCreacion
{
    [JsonPropertyName("correo")]
    public string Correo { get; set; } = string.Empty;

    [JsonPropertyName("contrasenia")]
    public string Contrasenia { get; set; } = string.Empty;

    [JsonPropertyName("nombreUsuario")]
    public string NombreUsuario { get; set; } = string.Empty;

    [JsonPropertyName("descripcion")]
    public string Descripcion { get; set; } = "Me encanta esta aplicacion";

    [JsonPropertyName("fotoPerfil")]
    public string FotoPerfil { get; set; } = "default.png";
}
