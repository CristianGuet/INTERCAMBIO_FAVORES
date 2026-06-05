using System.Collections.ObjectModel;
using System.Windows.Input;
using AppFavoresEscritorio.Modelos.Chats;
using AppFavoresEscritorio.Modelos.Favores;
using AppFavoresEscritorio.Modelos.Grupos;
using AppFavoresEscritorio.Servicios;
using AppFavoresEscritorio.Singleton;
using AppFavoresEscritorio.VistaModelos.Base;
 
namespace AppFavoresEscritorio.VistaModelos.Inicio;
 
public class InicioVistaModelo : VistaModeloBase
{
    private readonly ServicioChat _servicioChat;
    private readonly ServicioFavor _servicioFavor;
    private readonly ServicioGrupo _servicioGrupo;
    private readonly ServicioAutenticacion _servicioAuth;
    private bool _isRefreshing;
 
    private Task? _tareaInicial;
 
    public InicioVistaModelo(
        ServicioChat servicioChat,
        ServicioFavor servicioFavor,
        ServicioGrupo servicioGrupo,
        ServicioAutenticacion servicioAuth)
    {
        _servicioChat  = servicioChat;
        _servicioFavor = servicioFavor;
        _servicioGrupo = servicioGrupo;
        _servicioAuth  = servicioAuth;
 
        MisChats   = new ObservableCollection<ChatPublico>();
        MisGrupos  = new ObservableCollection<GrupoPublico>();
        MisFavores = new ObservableCollection<FavorPublico>();
 
        ComandoRefrescar  = new Command(async () => await CargarTodoAsync());
        ComandoAbrirChat  = new Command<ChatPublico>(async  c => await AbrirChatAsync(c));
        ComandoAbrirGrupo = new Command<GrupoPublico>(async g => await AbrirGrupoAsync(g));
        ComandoAbrirFavor = new Command<FavorPublico>(async f => await AbrirFavorAsync(f));
        ConfiguracionApp.Instancia.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ConfiguracionApp.CodigoIdioma))
                Notificar(nameof(SaludoCompleto));
        };
    }
 
    public ObservableCollection<ChatPublico>  MisChats   { get; }
    public ObservableCollection<GrupoPublico> MisGrupos  { get; }
    public ObservableCollection<FavorPublico> MisFavores { get; }
 
    public string NombreUsuario => SesionApp.Instancia.Usuario?.Nombre ?? string.Empty;
    public string SaludoCompleto => string.Format(ObtenerTexto("Inicio_BienvenidaFormato"), NombreUsuario);

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set { _isRefreshing = value; Notificar(); }
    }
 
    public ICommand ComandoRefrescar  { get; }
    public ICommand ComandoAbrirChat  { get; }
    public ICommand ComandoAbrirGrupo { get; }
    public ICommand ComandoAbrirFavor { get; }
 
    public Task CargarTodoAsync() => _tareaInicial = EjecutarCargaAsync();
 
    private async Task EjecutarCargaAsync()
    {
        try
        {
            IsRefreshing = true;
            LimpiarMensajes();
 
            var token = await _servicioAuth.ObtenerTokenAsync();
            if (string.IsNullOrWhiteSpace(token)) return;
 
            _servicioChat.AplicarToken(token);
            _servicioFavor.AplicarToken(token);
            _servicioGrupo.AplicarToken(token);
 
            var miCorreo = SesionApp.Instancia.Usuario?.Correo ?? string.Empty;
 
            var tChats   = _servicioChat.ObtenerMisChatsAsync();
            var tGrupos  = _servicioGrupo.ObtenerMisGruposAsync();
            var tFavores = _servicioFavor.ObtenerMisFavoresPublicadosAsync();
 
            await Task.WhenAll(tChats, tGrupos, tFavores);
 
            var chats   = await tChats;
            var grupos  = await tGrupos;
            var favores = await tFavores;
 
            foreach (var c in chats)
                c.MiCorreo = miCorreo;
 
            foreach (var g in grupos)
                g.TextoRol = g.EsAdminCorreo(miCorreo)
                    ? ObtenerTexto("RolAdmin")
                    : ObtenerTexto("RolMiembro");

            MainThread.BeginInvokeOnMainThread(() =>
            {
                MisChats.Clear();

                foreach (var c in chats)
                {
                    c.MiCorreo = miCorreo;
                    if (c.Tipo == "grupo")
                    {
                        var grupoRelacionado = grupos.FirstOrDefault(g =>
                            g.Miembros.Any(m => c.Participantes.Contains(m.CorreoUsuario)));
                        if (grupoRelacionado != null)
                            c.NombreGrupo = grupoRelacionado.NombreGrupo;
                    }
                }

                // Quedarse solo con el más reciente por NombreGrupo
                var chatsSinDuplicados = chats
                    .GroupBy(c => c.Tipo == "grupo" ? c.NombreGrupo : c.Id)
                    .Select(g => g.OrderByDescending(c => c.UltimaActividad).First())
                    .ToList();

                foreach (var c in chatsSinDuplicados)
                    MisChats.Add(c);

                MisGrupos.Clear();
                foreach (var g in grupos) MisGrupos.Add(g);

                MisFavores.Clear();
                foreach (var f in favores) MisFavores.Add(f);
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
        }
    }
 
    private async Task AbrirChatAsync(ChatPublico? chat)
    {
        if (chat is null) return;
        var miCorreo   = SesionApp.Instancia.Usuario?.Correo ?? string.Empty;
        var correoOtro = chat.Participantes.FirstOrDefault(p =>
            !string.Equals(p, miCorreo, StringComparison.OrdinalIgnoreCase)) ?? string.Empty;
        await Shell.Current.GoToAsync($"chatprivado?correoOtro={Uri.EscapeDataString(correoOtro)}");
    }
 
    private async Task AbrirGrupoAsync(GrupoPublico? grupo)
    {
        if (grupo is null) return;
        await Shell.Current.GoToAsync($"detallechatgrupo?grupoId={Uri.EscapeDataString(grupo.Id)}");
    }
 
    private async Task AbrirFavorAsync(FavorPublico? favor)
    {
        if (favor is null) return;
        await Task.CompletedTask;
    }
}