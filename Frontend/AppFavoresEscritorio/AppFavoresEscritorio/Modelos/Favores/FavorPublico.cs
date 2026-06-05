using System.Text.Json.Serialization;
using AppFavoresEscritorio.Singleton;

namespace AppFavoresEscritorio.Modelos.Favores;

public class FavorPublico
{
    [JsonPropertyName("_id")]              public string  Id                { get; set; } = string.Empty;
    [JsonPropertyName("titulo")]           public string  Titulo            { get; set; } = string.Empty;
    [JsonPropertyName("descripcion")]      public string  Descripcion       { get; set; } = string.Empty;
    [JsonPropertyName("modalidad")]        public string  Modalidad         { get; set; } = string.Empty;
    [JsonPropertyName("tipoCompensacion")] public string  TipoCompensacion  { get; set; } = string.Empty;
    [JsonPropertyName("cantidadDinero")]   public float?  CantidadDinero    { get; set; }
    [JsonPropertyName("recompensaFavor")]  public string? RecompensaFavor   { get; set; }
    [JsonPropertyName("ubicacion")]        public string? Ubicacion         { get; set; }
    [JsonPropertyName("estado")]           public string  Estado            { get; set; } = string.Empty;
    [JsonPropertyName("fechaCreacion")]    public string  FechaCreacion     { get; set; } = string.Empty;
    [JsonPropertyName("idUsuarioOfrece")]  public string  IdUsuarioOfrece   { get; set; } = string.Empty;
    [JsonPropertyName("idUsuarioSolicita")]public string? IdUsuarioSolicita { get; set; }

    // Asignado desde el ViewModel antes de mostrar
    [JsonIgnore] public bool PuedeAceptar { get; set; } = false;

    [JsonIgnore]
    public string ResumenCompensacion => TipoCompensacion switch
    {
        "dinero" => $"{CantidadDinero} €",
        "favor"  => RecompensaFavor ?? "Favor a acordar",
        "ambos"  => $"{CantidadDinero} € + favor",
        _        => string.Empty
    };

    [JsonIgnore]
    public string TextoModalidad => Modalidad == "presencial" ? "Presencial" : "Remoto";

    [JsonIgnore]
    public string ColorEstado => Estado switch
    {
        "disponible" => "#10B981",
        "aceptado"   => "#F59E0B",
        "finalizado" => "#6366F1",
        "cancelado"  => "#EF4444",
        _            => "#6B7280"
    };

    [JsonIgnore]
    public Color ColorEstadoVisual => Color.FromArgb(ColorEstado);

    [JsonIgnore]
    public string TextoEstado => Estado switch
    {
        "disponible" => "DISPONIBLE",
        "aceptado"   => "ACEPTADO",
        "finalizado" => "FINALIZADO",
        "cancelado"  => "CANCELADO",
        _            => Estado.ToUpperInvariant()
    };

    // ─── Propiedades de visibilidad de botones ──────────────────────────────

    private string MiCorreo =>
        SesionApp.Instancia.Usuario?.Correo ?? string.Empty;

    /// <summary>
    /// Muestra el botón Eliminar cuando el favor está disponible/cancelado/finalizado
    /// Y soy el creador.
    /// </summary>
    [JsonIgnore]
    public bool MostrarEliminar =>
        Estado != "aceptado"
        && string.Equals(IdUsuarioOfrece, MiCorreo, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Muestra los botones Cancelar y Finalizar cuando el favor está aceptado
    /// Y soy participante (creador o solicitante).
    /// </summary>
    [JsonIgnore]
    public bool MostrarCancelarFinalizar =>
        Estado == "aceptado"
        && (string.Equals(IdUsuarioOfrece,   MiCorreo, StringComparison.OrdinalIgnoreCase)
         || string.Equals(IdUsuarioSolicita, MiCorreo, StringComparison.OrdinalIgnoreCase));
}
