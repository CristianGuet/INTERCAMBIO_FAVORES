using AppFavoresEscritorio.Servicios;
using AppFavoresEscritorio.VistaModelos.Administracion;

namespace AppFavoresEscritorio.Vistas.Administracion;
public partial class PaginaAdministracion : ContentPage
{
    private readonly AdministracionVistaModelo _vm;

    public PaginaAdministracion(ServicioAdmin servicioAdmin, ServicioAutenticacion servicioAuth)
    {
        InitializeComponent();
        _vm = new AdministracionVistaModelo(servicioAdmin, servicioAuth);
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        categoriaPicker.SelectedIndex = 0;
        _vm.CategoriaSeleccionada = 0;
        await _vm.CargarDatosAsync();
    }

    private async void OnCategoriaChanged(object sender, EventArgs e)
    {
        _vm.CategoriaSeleccionada = categoriaPicker.SelectedIndex;
        await _vm.CargarDatosAsync();
    }
}