using System.Collections.ObjectModel;
using System.Windows.Input;
using AppFavoresEscritorio.Modelos.Chats;
using AppFavoresEscritorio.Servicios;
using AppFavoresEscritorio.Singleton;
using AppFavoresEscritorio.VistaModelos.Base;
 
namespace AppFavoresEscritorio.VistaModelos.Chats;
 
public class ChatPrivadoVistaModelo : VistaModeloBase, IQueryAttributable
{
    private readonly ServicioChat _servicioChat;
    private readonly ServicioAutenticacion _servicioAuth;
 
    private string _correoOtroUsuario = string.Empty;
    private string _idChat = string.Empty;
    private string _textoNuevoMensaje = string.Empty;
 
    public ChatPrivadoVistaModelo(ServicioChat servicioChat, ServicioAutenticacion servicioAuth)
    {
        _servicioChat = servicioChat;
        _servicioAuth = servicioAuth;
        Mensajes = new ObservableCollection<MensajePublico>();
        ComandoEnviar = new Command(async () => await EnviarMensajeAsync());
    }
 
    public ObservableCollection<MensajePublico> Mensajes { get; }
 
    public string TextoNuevoMensaje
    {
        get => _textoNuevoMensaje;
        set { _textoNuevoMensaje = value; Notificar(); }
    }
 
    public string TituloChat => _correoOtroUsuario;
 
    public string MiCorreo => SesionApp.Instancia.Usuario?.Correo ?? string.Empty;
 
    public ICommand ComandoEnviar { get; }
 
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("correoOtro", out var val1) && val1?.ToString() is { Length: > 0 } v1)
            _correoOtroUsuario = v1;
        else if (query.TryGetValue("correo", out var val2))
            _correoOtroUsuario = val2?.ToString() ?? string.Empty;
    }
 
    public async Task CargarAsync()
    {
        if (string.IsNullOrEmpty(_correoOtroUsuario)) return;
 
        try
        {
            EstaOcupado = true;
            LimpiarMensajes();
 
            var token = await _servicioAuth.ObtenerTokenAsync();
            if (string.IsNullOrWhiteSpace(token)) return;
 
            _servicioChat.AplicarToken(token);
 
            var chat = await _servicioChat.ObtenerOCrearChatPrivadoAsync(_correoOtroUsuario);
            if (chat is null)
            {
                MensajeError = ObtenerTexto("Error_Generico");
                return;
            }
 
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