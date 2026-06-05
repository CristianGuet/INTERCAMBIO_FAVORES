using System.Collections.ObjectModel;
using System.Windows.Input;
using AppFavoresEscritorio.Modelos.Grupos;
using AppFavoresEscritorio.Servicios;
using AppFavoresEscritorio.VistaModelos.Base;

namespace AppFavoresEscritorio.VistaModelos.Grupos;

public class ExplorarGruposVistaModelo : VistaModeloBase
{
    private readonly ServicioGrupo _servicioGrupo;
    private readonly ServicioAutenticacion _servicioAuth;
    private bool _isRefreshing;
    private string _textoBusqueda = string.Empty;

    public ExplorarGruposVistaModelo(ServicioGrupo servicioGrupo, ServicioAutenticacion servicioAuth)
    {
        _servicioGrupo = servicioGrupo;
        _servicioAuth = servicioAuth;
        Resultados = new ObservableCollection<GrupoBusqueda>();

        ComandoBuscar = new Command(async () => await BuscarAsync());
        ComandoUnirse = new Command<GrupoBusqueda>(async g => await UnirseAsync(g));
    }

    public string TextoBusqueda
    {
        get => _textoBusqueda;
        set { _textoBusqueda = value; Notificar(); }
    }

    public ObservableCollection<GrupoBusqueda> Resultados { get; }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set { _isRefreshing = value; Notificar(); }
    }

    public bool SinResultados => Resultados.Count == 0 && !EstaOcupado;

    public ICommand ComandoBuscar { get; }
    public ICommand ComandoUnirse { get; }

    private async Task BuscarAsync()
    {
        LimpiarMensajes();

        if (TextoBusqueda.Trim().Length < 3)
        {
            MensajeError = "Escribe al menos 3 caracteres";
            Notificar(nameof(SinResultados));
            return;
        }

        try
        {
            IsRefreshing = true;
            EstaOcupado = true;
            Notificar(nameof(SinResultados));

            var token = await _servicioAuth.ObtenerTokenAsync();
            _servicioGrupo.AplicarToken(token);

            var lista = await _servicioGrupo.BuscarGruposAsync(TextoBusqueda.Trim());

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Resultados.Clear();
                foreach (var g in lista)
                    Resultados.Add(g);
                Notificar(nameof(SinResultados));
            });
        }
        catch (HttpRequestException ex)
        {
            MensajeError = string.IsNullOrWhiteSpace(ex.Message)
                ? ObtenerTexto("Error_Generico")
                : ex.Message;
        }
        catch (Exception)
        {
            MensajeError = ObtenerTexto("Error_Generico");
        }
        finally
        {
            IsRefreshing = false;
            EstaOcupado = false;
            Notificar(nameof(SinResultados));
        }
    }

    private async Task UnirseAsync(GrupoBusqueda? grupo)
    {
        if (grupo is null) return;

        try
        {
            EstaOcupado = true;
            LimpiarMensajes();

            var token = await _servicioAuth.ObtenerTokenAsync();
            _servicioGrupo.AplicarToken(token);

            await _servicioGrupo.UnirseGrupoAsync(grupo.Id);

            await Shell.Current.DisplayAlertAsync(
                "Grupo",
                $"Te has unido a '{grupo.NombreGrupo}' correctamente.",
                "Aceptar");

            await Shell.Current.GoToAsync($"detallegrupo?grupoId={grupo.Id}");
        }
        catch (HttpRequestException ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Error",
                string.IsNullOrWhiteSpace(ex.Message) ? ObtenerTexto("Error_Generico") : ex.Message,
                "Aceptar");
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlertAsync(
                "Error",
                ObtenerTexto("Error_Generico"),
                "Aceptar");
        }
        finally
        {
            EstaOcupado = false;
        }
    }
}