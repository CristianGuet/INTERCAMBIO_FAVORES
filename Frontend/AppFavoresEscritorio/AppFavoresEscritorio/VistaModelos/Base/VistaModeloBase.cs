using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AppFavoresEscritorio.VistaModelos.Base;

public abstract class VistaModeloBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private bool _estaOcupado;
    private string _titulo = string.Empty;
    private string _mensajeError = string.Empty;

    public bool EstaOcupado
    {
        get => _estaOcupado;
        set
        {
            if (_estaOcupado == value) return;
            _estaOcupado = value;
            Notificar();
        }
    }

    public string Titulo
    {
        get => _titulo;
        set { _titulo = value; Notificar(); }
    }

    public string MensajeError
    {
        get => _mensajeError;
        set { _mensajeError = value; Notificar(); Notificar(nameof(HayError)); }
    }

    public bool HayError => !string.IsNullOrWhiteSpace(_mensajeError);

    protected void LimpiarMensajes() => MensajeError = string.Empty;

    protected void Notificar([CallerMemberName] string? nombre = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));

    protected static string ObtenerTexto(string clave) =>
        Application.Current?.Resources[clave]?.ToString() ?? clave;
}
