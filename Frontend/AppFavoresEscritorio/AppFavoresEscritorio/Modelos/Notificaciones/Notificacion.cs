using System.Text.Json.Serialization;

namespace AppFavoresEscritorio.Modelos.Notificaciones;

public class Notificacion
{
    [JsonPropertyName("_id")] public string Id { get; set; } = string.Empty;
    [JsonPropertyName("usuarioDestino")] public string UsuarioDestino { get; set; } = string.Empty;
    [JsonPropertyName("mensaje")] public string Mensaje { get; set; } = string.Empty;
    [JsonPropertyName("tipo")] public string Tipo { get; set; } = "info";
    [JsonPropertyName("leida")] public bool Leida { get; set; }
    [JsonPropertyName("idReferencia")] public string? IdReferencia { get; set; }
    [JsonPropertyName("fecha")] public string Fecha { get; set; } = string.Empty;

    public string TextoTipo => Tipo switch
    {
        "exito" or "solicitud_aceptada" => "Exito",
        "alerta" or "expulsion_grupo" or "solicitud_rechazada" => "Alerta",
        "invitacion_grupo" => "Grupo",
        "cambio_rol" or "cambio_nombre_grupo" => "Cambio",
        _ => "Aviso"
    };

    public string ColorTipo => Tipo switch
    {
        "exito" or "solicitud_aceptada" => "#10B981",
        "alerta" or "expulsion_grupo" or "solicitud_rechazada" => "#EF4444",
        "invitacion_grupo" => "#6366F1",
        _ => "#6B7280"
    };

    [JsonIgnore]
    public Color ColorTipoVisual => Color.FromArgb(ColorTipo);
}
