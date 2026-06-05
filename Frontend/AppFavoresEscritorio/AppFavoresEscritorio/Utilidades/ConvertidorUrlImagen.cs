using System.Globalization;

namespace AppFavoresEscritorio.Utilidades;

public class ConvertidorUrlImagen : IValueConverter
{
    private const string UrlBase = "http://localhost:8000/imagenes/";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string nombre && !string.IsNullOrWhiteSpace(nombre))
            return $"{UrlBase}{nombre}";
        return $"{UrlBase}default.png";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}