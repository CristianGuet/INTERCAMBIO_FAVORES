using System.Globalization;

namespace AppFavoresEscritorio.Utilidades;

public class ConvertidorEstadoFavor : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string estado)
        {
            string clave = estado switch
            {
                "disponible" => "EstadoDisponible",
                "aceptado" => "EstadoAceptado",
                "finalizado" => "EstadoFinalizado",
                "cancelado" => "EstadoCancelado",
                _ => null
            };
            if (clave != null && Application.Current.Resources.TryGetValue(clave, out var texto))
                return texto;
        }
        return value?.ToString()?.ToUpperInvariant() ?? "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}