using AppFavoresEscritorio.VistaModelos.Grupos;

namespace AppFavoresEscritorio.Vistas.Grupos;

public partial class PaginaDetalleGrupo : ContentPage
{
    private readonly DetalleGrupoVistaModelo _vm;

    public PaginaDetalleGrupo(DetalleGrupoVistaModelo vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _vm.CargarAsync();
    }
}