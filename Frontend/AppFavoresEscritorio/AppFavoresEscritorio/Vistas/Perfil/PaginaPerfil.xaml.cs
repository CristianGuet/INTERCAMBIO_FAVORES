using AppFavoresEscritorio.VistaModelos.Perfil;
 
namespace AppFavoresEscritorio.Vistas.Perfil;
 
public partial class PaginaPerfil : ContentPage
{
    private readonly PerfilVistaModelo _vm;
 
    public PaginaPerfil(PerfilVistaModelo vm)
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
            await _vm.CargarPerfilAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PaginaPerfil.OnAppearing] {ex.Message}");
        }
    }
}