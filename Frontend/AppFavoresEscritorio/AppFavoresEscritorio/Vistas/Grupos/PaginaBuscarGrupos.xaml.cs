using AppFavoresEscritorio.VistaModelos.Grupos;

namespace AppFavoresEscritorio.Vistas.Grupos;

public partial class PaginaBuscarGrupos : ContentPage
{
    public PaginaBuscarGrupos(BuscarGruposVistaModelo vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
