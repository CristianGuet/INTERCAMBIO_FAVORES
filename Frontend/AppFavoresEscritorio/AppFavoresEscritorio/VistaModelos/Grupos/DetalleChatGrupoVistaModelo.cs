using System.Collections.ObjectModel;
using System.Windows.Input;
using AppFavoresEscritorio.Modelos.Chats;
using AppFavoresEscritorio.Servicios;
using AppFavoresEscritorio.Singleton;
using AppFavoresEscritorio.VistaModelos.Base;
 
namespace AppFavoresEscritorio.VistaModelos.Grupos;
 
public class DetalleChatGrupoVistaModelo : VistaModeloBase, IQueryAttributable
{
    private readonly ServicioChat _servicioChat;
    private readonly ServicioGrupo _servicioGrupo;
    private readonly ServicioAutenticacion _servicioAuth;
 
    private string _grupoId = string.Empty;
    private string _idChat = string.Empty;
    private string _tituloGrupo = string.Empty;
    private string _textoNuevoMensaje = string.Empty;
 
    public DetalleChatGrupoVistaModelo(
        ServicioChat servicioChat,
        ServicioGrupo servicioGrupo,
        ServicioAutenticacion servicioAuth)
    {
        _servicioChat  = servicioChat;
        _servicioGrupo = servicioGrupo;
        _servicioAuth  = servicioAuth;
 
        Mensajes = new ObservableCollection<MensajePublico>();
        ComandoEnviar = new Command(async () => await EnviarMensajeAsync());
        ComandoVolver = new Command(async () => await Shell.Current.GoToAsync("//PaginaMisGrupos"));
    }
 
    public ObservableCollection<MensajePublico> Mensajes { get; }
 
    public string TituloGrupo
    {
        get => _tituloGrupo;
        set { _tituloGrupo = value; Notificar(); }
    }
 
    public string TextoNuevoMensaje
    {
        get => _textoNuevoMensaje;
        set { _textoNuevoMensaje = value; Notificar(); }
    }
 
    public string MiCorreo => SesionApp.Instancia.Usuario?.Correo ?? string.Empty;
 
    public ICommand ComandoEnviar { get; }
    public ICommand ComandoVolver { get; }
 
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("grupoId", out var val))
            _grupoId = val?.ToString() ?? string.Empty;
    }
 
    public async Task CargarDatosAsync()
    {
        if (string.IsNullOrEmpty(_grupoId)) return;
 
        try
        {
            EstaOcupado = true;
            LimpiarMensajes();
 
            var token = await _servicioAuth.ObtenerTokenAsync();
            if (string.IsNullOrWhiteSpace(token)) return;
 
            _servicioGrupo.AplicarToken(token);
            _servicioChat.AplicarToken(token);
 
            var grupos = await _servicioGrupo.ObtenerMisGruposAsync();
            var grupo = grupos.FirstOrDefault(g => g.Id == _grupoId);
            TituloGrupo = grupo?.NombreGrupo ?? "Chat de grupo";
 
            var participantes = grupo?.Miembros.Select(m => m.CorreoUsuario).ToList()
                                ?? new List<string>();
 
            if (participantes.Count == 0)
                return;
 
            var chat = await _servicioChat.ObtenerOCrearChatGrupoAsync(participantes);
            if (chat is null) return;
 
            _idChat = chat.Id;
 
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Mensajes.Clear();
                foreach (var m in chat.Mensajes)
                    Mensajes.Add(m);
            });
 
            await _servicioChat.MarcarLeidoAsync(_idChat);
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
            EstaOcupado = false;
        }
    }
 
    private async Task EnviarMensajeAsync()
    {
        if (string.IsNullOrWhiteSpace(TextoNuevoMensaje) || string.IsNullOrEmpty(_idChat))
            return;
 
        try
        {
            EstaOcupado = true;
            LimpiarMensajes();
 
            var token = await _servicioAuth.ObtenerTokenAsync();
            if (string.IsNullOrWhiteSpace(token)) return;
 
            _servicioChat.AplicarToken(token);
 
            var mensaje = await _servicioChat.EnviarMensajeAsync(_idChat, TextoNuevoMensaje);
            if (mensaje is not null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Mensajes.Add(mensaje);
                    TextoNuevoMensaje = string.Empty;
                });
            }
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
            EstaOcupado = false;
        }
    }
}