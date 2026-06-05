using System.Text.Json.Serialization;
using AppFavoresEscritorio.Singleton;

namespace AppFavoresEscritorio.Modelos.Grupos;

public class MiembroGrupo
{
    [JsonPropertyName("correoUsuario")] public string CorreoUsuario { get; set; } = string.Empty;
    [JsonPropertyName("rol")]           public string Rol           { get; set; } = "miembro";
}

public class SolicitudGrupo
{
    [JsonPropertyName("correoUsuario")] public string CorreoUsuario  { get; set; } = string.Empty;
    [JsonPropertyName("comentario")]    public string? Comentario    { get; set; }
    [JsonPropertyName("fechaSolicitud")]public string FechaSolicitud { get; set; } = string.Empty;
}

public class GrupoPublico
{
    [JsonPropertyName("_id")]          public string Id          { get; set; } = string.Empty;
    [JsonPropertyName("nombreGrupo")]  public string NombreGrupo { get; set; } = string.Empty;
    [JsonPropertyName("descripcion")]  public string Descripcion { get; set; } = string.Empty;
    [JsonPropertyName("fotoGrupo")]    public string FotoGrupo   { get; set; } = "group_default.png";
    [JsonPropertyName("miembros")]     public List<MiembroGrupo>   Miembros    { get; set; } = [];
    [JsonPropertyName("solicitudes")]  public List<SolicitudGrupo> Solicitudes { get; set; } = [];
    [JsonPropertyName("fechaCreacion")]public string FechaCreacion { get; set; } = string.Empty;

    [JsonIgnore] public string TextoRol { get; set; } = "Miembro";

    public int NumeroMiembros => Miembros.Count;

    [JsonIgnore]
    public bool EsAdmin =>
        Miembros.Any(m =>
            string.Equals(m.CorreoUsuario, SesionApp.Instancia.Usuario?.Correo,
                StringComparison.OrdinalIgnoreCase)
            && m.Rol == "admin");

    public bool EsAdminCorreo(string correo) =>
        Miembros.Any(m =>
            string.Equals(m.CorreoUsuario, correo, StringComparison.OrdinalIgnoreCase)
            && m.Rol == "admin");
}

public class GrupoBusqueda
{
    [JsonPropertyName("id")]             public string Id            { get; set; } = string.Empty;
    [JsonPropertyName("nombreGrupo")]    public string NombreGrupo   { get; set; } = string.Empty;
    [JsonPropertyName("descripcion")]    public string Descripcion   { get; set; } = string.Empty;
    [JsonPropertyName("fotoGrupo")]      public string FotoGrupo     { get; set; } = string.Empty;
    [JsonPropertyName("numeroMiembros")] public int    NumeroMiembros { get; set; }
}
