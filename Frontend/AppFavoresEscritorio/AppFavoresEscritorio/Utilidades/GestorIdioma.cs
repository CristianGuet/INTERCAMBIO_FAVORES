using AppFavoresEscritorio.Resources.Idiomas;
using AppFavoresEscritorio.Resources.Styles;

namespace AppFavoresEscritorio.Utilidades;

public static class GestorIdioma
{
    public static void AplicarIdioma(string codigo)
    {
        if (Application.Current?.Resources is not ResourceDictionary raiz)
            return;

        void Aplicar()
        {
            var temaActual = raiz.MergedDictionaries
                .FirstOrDefault(d => d is TemaClaro or TemaOscuro);

            raiz.MergedDictionaries.Clear();

            if (temaActual is not null)
                raiz.MergedDictionaries.Add(temaActual);
            else
                raiz.MergedDictionaries.Add(new TemaClaro());

            raiz.MergedDictionaries.Add(
                codigo.Equals("en", StringComparison.OrdinalIgnoreCase)
                    ? new StringsEn()
                    : new StringsEs());

            if (Shell.Current is Navegacion.AppShell shell)
                shell.AplicarColoresShell();
        }

        if (MainThread.IsMainThread)
            Aplicar();
        else
            MainThread.BeginInvokeOnMainThread(Aplicar);
    }
}
