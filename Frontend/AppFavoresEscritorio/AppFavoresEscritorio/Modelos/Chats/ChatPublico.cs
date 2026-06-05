using System.Text.Json.Serialization;
using AppFavoresEscritorio.Singleton;

namespace AppFavoresEscritorio.Modelos.Chats;

public class MensajePublico
{
    [JsonPropertyName("emisor")] public string Emisor { get; set; } = string.Empty;
    [JsonPropertyName("contenido")] public string Contenido { get; set; } = string.Empty;
    [JsonPropertyName("fechaEnvio")] public string FechaEnvio { get; set; } = string.Empty;
    [JsonPropertyName("leido")] public bool Leido { get; set; }

    [JsonIgnore] public bool EsMio => Emisor == SesionApp.Instancia.Usuario?.Correo;
}

public class ChatPublico
{
    [JsonPropertyName("_id")] public string Id { get; set; } = string.Empty;
    [JsonPropertyName("tipo")] public string Tipo { get; set; } = "privado";
    [JsonPropertyName("participantes")] public List<string> Participantes { get; set; } = [];
    [JsonPropertyName("mensajes")] public List<MensajePublico> Mensajes { get; set; } = [];
    [JsonPropertyName("ultimaActividad")] public string UltimaActividad { get; set; } = string.Empty;

    [JsonIgnore] public string MiCorreo { get; set; } = string.Empty;

    [JsonIgnore] public string NombreGrupo { get; set; } = string.Empty;

    [JsonIgnore]
    public string TituloMostrar =>
        Tipo == "grupo"
            ? (string.IsNullOrEmpty(NombreGrupo) ? "Chat del grupo" : NombreGrupo)
            : string.Join(", ", Participantes.Where(p =>
                !string.Equals(p, MiCorreo, StringComparison.OrdinalIgnoreCase)));

    [JsonIgnore]
    public string UltimoMensaje => Mensajes.LastOrDefault()?.Contenido ?? "Sin mensajes";

    public int MensajesNoLeidos(string miCorreo) =>
        Mensajes.Count(m => !m.Leido && m.Emisor != miCorreo);

    [JsonIgnore]
    public int NoLeidos => string.IsNullOrEmpty(MiCorreo) ? 0 : MensajesNoLeidos(MiCorreo);
}
