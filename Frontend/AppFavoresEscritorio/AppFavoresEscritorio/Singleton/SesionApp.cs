using AppFavoresEscritorio.Modelos.Autenticacion;

namespace AppFavoresEscritorio.Singleton;

public sealed class SesionApp
{
    public static SesionApp Instancia { get; } = new();

    public string? Token { get; private set; }
    public UsuarioResumen? Usuario { get; private set; }
    public string Rol { get; private set; } = "Usuario";

    public bool EstaLogueado => !string.IsNullOrWhiteSpace(Token);
    public bool EsAdmin => string.Equals(Rol, "Admin", StringComparison.OrdinalIgnoreCase);

    private SesionApp() { }

    public void EstablecerSesion(RespuestaLogin respuesta)
    {
        Token = respuesta.AccessToken;
        Usuario = respuesta.Usuario;
        Rol = "Usuario";
    }

    public void EstablecerUsuario(UsuarioResumen perfil) => Usuario = perfil;

    public void EstablecerRol(string rol) => Rol = rol;

    public void Limpiar()
    {
        Token = null;
        Usuario = null;
        Rol = "Usuario";
    }
}
