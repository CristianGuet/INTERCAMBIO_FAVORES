using AppFavoresEscritorio.VistaModelos.Grupos;

namespace AppFavoresEscritorio.Vistas.Grupos;

public partial class PaginaMisGrupos : ContentPage
{
    private readonly MisGruposVistaModelo _vm;

    public PaginaMisGrupos(MisGruposVistaModelo vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

protected override async void OnAppearing()
{
    base.OnAppearing();
    try
    {
        await _vm.CargarDatosAsync();
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"[MisGrupos.OnAppearing] {ex.Message}");
    }
}

    private async void OnCrearGrupoClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("creargrupo");
    }
}