using AppFavoresEscritorio.VistaModelos.Inicio;
 
namespace AppFavoresEscritorio.Vistas.Inicio;
 
public partial class PaginaInicio : ContentPage
{
    private readonly InicioVistaModelo _vm;
 
    public PaginaInicio(InicioVistaModelo vm)
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
            await _vm.CargarTodoAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PaginaInicio.OnAppearing] {ex.Message}");
        }
    }
}