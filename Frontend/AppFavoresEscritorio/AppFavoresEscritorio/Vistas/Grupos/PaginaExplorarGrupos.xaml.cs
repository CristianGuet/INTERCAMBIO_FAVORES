using AppFavoresEscritorio.VistaModelos.Grupos;

namespace AppFavoresEscritorio.Vistas.Grupos;

public partial class PaginaExplorarGrupos : ContentPage
{
    public PaginaExplorarGrupos(ExplorarGruposVistaModelo vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}