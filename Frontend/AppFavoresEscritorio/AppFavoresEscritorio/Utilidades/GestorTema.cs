using AppFavoresEscritorio.Resources.Idiomas;
using AppFavoresEscritorio.Resources.Styles;

namespace AppFavoresEscritorio.Utilidades;

public static class GestorTema
{
    private const string ArchivoLogoClaro = "claro_amistad.png";
    private const string ArchivoLogoOscuro = "oscuro_amistad.png";
    private static bool _aplicando = false;

    public static void AplicarTema(bool oscuro)
    {
        if (Application.Current is not Application app)
            return;


        if (MainThread.IsMainThread)
            AplicarTemaEnUi(app, oscuro);
        else
            MainThread.BeginInvokeOnMainThread(() => AplicarTemaEnUi(app, oscuro));
    }

    public static string LogoSegunTema(bool oscuro) =>
        oscuro ? ArchivoLogoClaro : ArchivoLogoOscuro;

    public static bool EsOscuroActual() =>
        Application.Current?.Resources.MergedDictionaries
            .Any(d => d is TemaOscuro) ?? false;

    private static void AplicarTemaEnUi(Application app, bool oscuro)
    {
        if (app.Resources is not ResourceDictionary raiz)
            return;

        app.UserAppTheme = oscuro ? AppTheme.Dark : AppTheme.Light;

        var idiomaActual = raiz.MergedDictionaries
            .FirstOrDefault(d => d is StringsEs or StringsEn);

        raiz.MergedDictionaries.Clear();
        raiz.MergedDictionaries.Add(oscuro ? new TemaOscuro() : new TemaClaro());

        if (idiomaActual is not null)
            raiz.MergedDictionaries.Add(idiomaActual);
        else
            raiz.MergedDictionaries.Add(new StringsEs());

        if (Shell.Current is Navegacion.AppShell shell)
            shell.AplicarColoresShell();
    }
}
