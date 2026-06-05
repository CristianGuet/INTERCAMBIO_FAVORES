using AppFavoresEscritorio.Modelos.Chats;
using System.Globalization;

namespace AppFavoresEscritorio.Utilidades;

public class ConvertidorTituloChat : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ChatPublico chat && !string.IsNullOrEmpty(chat.MiCorreo))
        {
            if (chat.Tipo == "grupo")
            {
                if (Application.Current.Resources.TryGetValue("ChatGrupoTitulo", out var grupoTexto))
                    return grupoTexto?.ToString() ?? "Grupo";
                return "Grupo";
            }
            else
            {
                var otro = chat.Participantes.FirstOrDefault(p =>
                    !string.Equals(p, chat.MiCorreo, StringComparison.OrdinalIgnoreCase));
                return otro ?? "Chat";
            }
        }
        return value?.ToString() ?? "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}