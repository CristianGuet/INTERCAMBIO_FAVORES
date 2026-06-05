using System.Globalization;

namespace AppFavoresEscritorio.Utilidades;

public class InvertirBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool activo && !activo;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool activo && !activo;
}