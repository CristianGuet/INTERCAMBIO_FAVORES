namespace AppFavoresEscritorio.Utilidades;

public static class GestorRecursos
{
    public static Color ObtenerColor(string clave, Color valorPorDefecto)
    {
        if (BuscarRecurso<Color>(clave) is Color color)
            return color;
        return valorPorDefecto;
    }

    public static T? BuscarRecurso<T>(string clave) where T : class =>
        BuscarRecursoInterno<T>(clave);

    private static T? BuscarRecursoInterno<T>(string clave) where T : class
    {
        if (Application.Current?.Resources is not ResourceDictionary raiz)
            return null;

        if (raiz.TryGetValue(clave, out var local) && local is T encontradoLocal)
            return encontradoLocal;

        return BuscarEnDiccionarios<T>(raiz.MergedDictionaries, clave);
    }

    private static T? BuscarEnDiccionarios<T>(ICollection<ResourceDictionary> diccionarios, string clave)
        where T : class
    {
        foreach (var diccionario in diccionarios)
        {
            if (diccionario.TryGetValue(clave, out var valor) && valor is T encontrado)
                return encontrado;

            var enAnidado = BuscarEnDiccionarios<T>(diccionario.MergedDictionaries, clave);
            if (enAnidado is not null)
                return enAnidado;
        }

        return null;
    }
}
