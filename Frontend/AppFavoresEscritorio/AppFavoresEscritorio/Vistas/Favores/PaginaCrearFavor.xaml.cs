using AppFavoresEscritorio.VistaModelos.Favores;

namespace AppFavoresEscritorio.Vistas.Favores;

public partial class PaginaCrearFavor : ContentPage
{
    public PaginaCrearFavor(CrearFavorVistaModelo vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
