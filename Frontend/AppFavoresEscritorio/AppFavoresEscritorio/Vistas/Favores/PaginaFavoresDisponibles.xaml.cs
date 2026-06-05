using AppFavoresEscritorio.VistaModelos.Favores;

namespace AppFavoresEscritorio.Vistas.Favores;

public partial class PaginaFavoresDisponibles : ContentPage
{
    private readonly FavoresDisponiblesVistaModelo _vm;

    public PaginaFavoresDisponibles(FavoresDisponiblesVistaModelo vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _vm.CargarFavoresAsync();
    }

    // Handler del ToolbarItem
    private void OnPublicarFavorClicked(object sender, EventArgs e) =>
        Shell.Current.GoToAsync("crearfavor");
}
