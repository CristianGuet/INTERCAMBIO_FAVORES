using AppFavoresEscritorio.Utilidades;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AppFavoresEscritorio.Singleton;

public sealed class ConfiguracionApp : INotifyPropertyChanged
{
    public static ConfiguracionApp Instancia { get; } = new();

    private string _codigoIdioma = "es";
    private bool _esTemaOscuro;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string CodigoIdioma
    {
        get => _codigoIdioma;
        set
        {
            if (_codigoIdioma == value) return;
            _codigoIdioma = value;
            Preferences.Default.Set("idioma", value);
            
            GestorIdioma.AplicarIdioma(value);
            
            Notificar();
            Notificar(nameof(EtiquetaIdiomaActual));
        }
    }

    public bool EsTemaOscuro
    {
        get => _esTemaOscuro;
        set
        {
            if (_esTemaOscuro == value) return;

            _esTemaOscuro = value;
            Preferences.Default.Set("tema_oscuro", value);

            GestorTema.AplicarTema(value);

            Notificar();
            Notificar(nameof(EtiquetaTemaActual));
            Notificar(nameof(LogoImagen));
        }
    }

    public string EtiquetaIdiomaActual => _codigoIdioma == "en" ? "ES" : "EN";

    public string EtiquetaTemaActual => _esTemaOscuro ? "☀️" : "🌙";

    /// <summary>Logo para binding en login; alineado con el recurso LogoImagen del diccionario de tema activo.</summary>
    public string LogoImagen => GestorTema.LogoSegunTema(_esTemaOscuro);

    public void AlternarTema() => EsTemaOscuro = !EsTemaOscuro;

    public void AlternarIdioma() =>
        CodigoIdioma = _codigoIdioma == "es" ? "en" : "es";

    public void CargarPreferencias(bool notificar = true)
    {
        _codigoIdioma = Preferences.Default.Get("idioma", "es");
        _esTemaOscuro = Preferences.Default.Get("tema_oscuro", false);

        if (!notificar) return;

        Notificar(nameof(CodigoIdioma));
        Notificar(nameof(EsTemaOscuro));
        Notificar(nameof(EtiquetaIdiomaActual));
        Notificar(nameof(EtiquetaTemaActual));
        Notificar(nameof(LogoImagen));
    }

    /// <summary>Dispara PropertyChanged para las propiedades visuales sin modificar valores.</summary>
    public void RefrescarEtiquetas()
    {
        Notificar(nameof(EtiquetaTemaActual));
        Notificar(nameof(EtiquetaIdiomaActual));
        Notificar(nameof(LogoImagen));
    }

    private void Notificar([CallerMemberName] string? nombre = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
}
