using AppFavoresEscritorio.VistaModelos.Grupos;
 
namespace AppFavoresEscritorio.Vistas.Grupos;
 
public partial class PaginaDetalleChatGrupo : ContentPage
{
    private readonly DetalleChatGrupoVistaModelo _vm;
 
    public PaginaDetalleChatGrupo(DetalleChatGrupoVistaModelo vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }
 
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Shell.SetNavBarIsVisible(this, false);
        try
        {
            await _vm.CargarDatosAsync();
            await ScrollToBottomAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DetalleChatGrupo.OnAppearing] {ex.Message}");
        }
    }
 
    private async Task ScrollToBottomAsync()
    {
        if (_vm.Mensajes.Count == 0) return;
        await Task.Delay(150);
        mensajesCollectionView.ScrollTo(_vm.Mensajes.Count - 1,
            position: ScrollToPosition.End, animate: true);
    }
}
 