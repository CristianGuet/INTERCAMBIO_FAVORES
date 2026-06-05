using AppFavoresEscritorio.VistaModelos.Grupos;

namespace AppFavoresEscritorio.Vistas.Grupos;

public partial class PaginaCrearGrupo : ContentPage
{
    public PaginaCrearGrupo(CrearGrupoVistaModelo vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
