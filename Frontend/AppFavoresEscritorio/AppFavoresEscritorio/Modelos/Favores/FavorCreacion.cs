using System.Text.Json.Serialization;

namespace AppFavoresEscritorio.Modelos.Favores;

public class FavorCreacion
{
    [JsonPropertyName("titulo")]
    public string Titulo { get; set; } = string.Empty;

    [JsonPropertyName("descripcion")]
    public string Descripcion { get; set; } = string.Empty;

    [JsonPropertyName("modalidad")]
    public string Modalidad { get; set; } = "remoto"; // "remoto" | "presencial"

    [JsonPropertyName("ubicacion")]
    public string? Ubicacion { get; set; }

    [JsonPropertyName("tipoCompensacion")]
    public string TipoCompensacion { get; set; } = "favor"; // "favor" | "dinero" | "ambos"

    [JsonPropertyName("cantidadDinero")]
    public float? CantidadDinero { get; set; }

    [JsonPropertyName("recompensaFavor")]
    public string? RecompensaFavor { get; set; }
}
