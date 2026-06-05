using System.Collections.ObjectModel;
using System.Windows.Input;
using AppFavoresEscritorio.Modelos.Favores;
using AppFavoresEscritorio.Modelos.Grupos;
using AppFavoresEscritorio.Servicios;
using AppFavoresEscritorio.Singleton;
using AppFavoresEscritorio.VistaModelos.Base;

namespace AppFavoresEscritorio.VistaModelos.Grupos;

public class MisGruposVistaModelo : VistaModeloBase
{
    private readonly ServicioGrupo _servicioGrupo;
    private readonly ServicioFavor _servicioFavor;
    private readonly ServicioAutenticacion _servicioAuth;
    private bool _isRefreshing;

    public MisGruposVistaModelo(ServicioGrupo servicioGrupo, ServicioFavor servicioFavor, ServicioAutenticacion servicioAuth)
    {
        _servicioGrupo = servicioGrupo;
        _servicioFavor = servicioFavor;
        _servicioAuth = servicioAuth;
        MisGrupos = new ObservableCollection<GrupoPublico>();
        MisFavores = new ObservableCollection<FavorPublico>();

        ComandoCargar = new Command(async () => await CargarDatosAsync());
        ComandoAceptarFavor = new Command<FavorPublico>(async f => await AceptarFavorAsync(f));
        ComandoCancelarFavor = new Command<FavorPublico>(async f => await CancelarFavorAsync(f));
        ComandoFinalizarFavor = new Command<FavorPublico>(async f => await FinalizarFavorAsync(f));
        ComandoEliminarFavor = new Command<FavorPublico>(async f => await EliminarFavorAsync(f));
        ComandoAbrirGrupo = new Command<GrupoPublico>(async g => await AbrirGrupoAsync(g));
    }

    public ObservableCollection<GrupoPublico> MisGrupos { get; }
    public ObservableCollection<FavorPublico> MisFavores { get; }

    public string MiCorreo => SesionApp.Instancia.Usuario?.Correo ?? string.Empty;

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set { _isRefreshing = value; Notificar(); }
    }

    public bool SinGrupos => MisGrupos.Count == 0 && !EstaOcupado;
    public bool SinFavores => MisFavores.Count == 0 && !EstaOcupado;

    public ICommand ComandoCargar { get; }
    public ICommand ComandoAceptarFavor { get; }
    public ICommand ComandoCancelarFavor { get; }
    public ICommand ComandoFinalizarFavor { get; }
    public ICommand ComandoEliminarFavor { get; }
    public ICommand ComandoAbrirGrupo { get; }

    public async Task CargarDatosAsync()
    {
        var token = await _servicioAuth.ObtenerTokenAsync();
        if (string.IsNullOrWhiteSpace(token)) return;

        try
        {
            IsRefreshing = true;
            EstaOcupado = true;
            LimpiarMensajes();

            _servicioGrupo.AplicarToken(token);
            _servicioFavor.AplicarToken(token);

            var grupos = await _servicioGrupo.ObtenerMisGruposAsync();
            var misFavoresPublicados = await _servicioFavor.ObtenerMisFavoresPublicadosAsync();
            var favoresAceptados = await _servicioFavor.ObtenerFavoresPorEstadoAsync("aceptado");
            var favoresFinalizados = await _servicioFavor.ObtenerFavoresPorEstadoAsync("finalizado");
            var favoresCancelados = await _servicioFavor.ObtenerFavoresPorEstadoAsync("cancelado");

            var favoresTodosSet = new HashSet<string>();
            var favoresAll = new List<FavorPublico>();

            foreach (var f in misFavoresPublicados)
            {
                if (favoresTodosSet.Add(f.Id))
                    favoresAll.Add(f);
            }
            foreach (var f in favoresAceptados)
            {
                if (favoresTodosSet.Add(f.Id))
                    favoresAll.Add(f);
            }
            foreach (var f in favoresFinalizados)
            {
                if (favoresTodosSet.Add(f.Id))
                    favoresAll.Add(f);
            }
            foreach (var f in favoresCancelados)
            {
                if (favoresTodosSet.Add(f.Id))
                    favoresAll.Add(f);
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                MisGrupos.Clear();
                foreach (var g in grupos)
                    MisGrupos.Add(g);

                MisFavores.Clear();
                foreach (var f in favoresAll)
                    MisFavores.Add(f);

                Notificar(nameof(SinGrupos));
                Notificar(nameof(SinFavores));
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
        }
    }

    private async Task AceptarFavorAsync(FavorPublico? favor)
    {
        if (favor is null) return;
        try
        {
            EstaOcupado = true;
            LimpiarMensajes();
            var token = await _servicioAuth.ObtenerTokenAsync();
            _servicioFavor.AplicarToken(token);
            await _servicioFavor.AceptarFavorAsync(favor.Id);
            await CargarDatosAsync();
        }
        catch (HttpRequestException ex)
        {
            MensajeError = string.IsNullOrWhiteSpace(ex.Message) ? ObtenerTexto("Error_Generico") : ex.Message;
        }
        catch (Exception)
        {
            MensajeError = ObtenerTexto("Error_Generico");
        }
        finally
        {
            EstaOcupado = false;
        }
    }

    private async Task CancelarFavorAsync(FavorPublico? favor)
    {
        if (favor is null) return;
        try
        {
            EstaOcupado = true;
            LimpiarMensajes();
            var token = await _servicioAuth.ObtenerTokenAsync();
            _servicioFavor.AplicarToken(token);
            await _servicioFavor.CancelarFavorAsync(favor.Id);
            await CargarDatosAsync();
        }
        catch (HttpRequestException ex)
        {
            MensajeError = string.IsNullOrWhiteSpace(ex.Message) ? ObtenerTexto("Error_Generico") : ex.Message;
        }
        catch (Exception)
        {
            MensajeError = ObtenerTexto("Error_Generico");
        }
        finally
        {
            EstaOcupado = false;
        }
    }

    private async Task FinalizarFavorAsync(FavorPublico? favor)
    {
        if (favor is null) return;
        try
        {
            EstaOcupado = true;
            LimpiarMensajes();
            var token = await _servicioAuth.ObtenerTokenAsync();
            _servicioFavor.AplicarToken(token);
            await _servicioFavor.FinalizarFavorAsync(favor.Id);
            await CargarDatosAsync();
        }
        catch (HttpRequestException ex)
        {
            MensajeError = string.IsNullOrWhiteSpace(ex.Message) ? ObtenerTexto("Error_Generico") : ex.Message;
        }
        catch (Exception)
        {
            MensajeError = ObtenerTexto("Error_Generico");
        }
        finally
        {
            EstaOcupado = false;
        }
    }

    private async Task EliminarFavorAsync(FavorPublico? favor)
    {
        if (favor is null) return;

        var aceptar = await Shell.Current.DisplayAlert(
            ObtenerTexto("ConfirmarEliminarTitulo"),
            ObtenerTexto("ConfirmarEliminarMensaje"),
            ObtenerTexto("ConfirmarSi"),
            ObtenerTexto("ConfirmarNo"));

        if (!aceptar) return;

        try
        {
            EstaOcupado = true;
            LimpiarMensajes();
            var token = await _servicioAuth.ObtenerTokenAsync();
            _servicioFavor.AplicarToken(token);
            await _servicioFavor.EliminarFavorAsync(favor.Id);
            await CargarDatosAsync();
        }
        catch (HttpRequestException ex)
        {
            MensajeError = string.IsNullOrWhiteSpace(ex.Message) ? ObtenerTexto("Error_Generico") : ex.Message;
        }
        catch (Exception)
        {
            MensajeError = ObtenerTexto("Error_Generico");
        }
        finally
        {
            EstaOcupado = false;
        }
    }

    private async Task AbrirGrupoAsync(GrupoPublico? grupo)
    {
        if (grupo is null) return;
        await Shell.Current.GoToAsync($"detallegrupo?grupoId={grupo.Id}");
    }
}
