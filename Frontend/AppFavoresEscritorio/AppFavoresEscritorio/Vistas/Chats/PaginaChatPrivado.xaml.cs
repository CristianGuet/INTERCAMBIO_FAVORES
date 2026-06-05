using AppFavoresEscritorio.VistaModelos.Chats;

namespace AppFavoresEscritorio.Vistas.Chats;

public partial class PaginaChatPrivado : ContentPage
{
    private readonly ChatPrivadoVistaModelo _vm;

    public PaginaChatPrivado(ChatPrivadoVistaModelo vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Shell.SetNavBarIsVisible(this, false);
        _ = CargarYDesplazar();
    }

    private async Task CargarYDesplazar()
    {
        await _vm.CargarAsync();

        var count = _vm.Mensajes.Count;
        if (count > 0)
        {
            await Task.Delay(100);
            mensajesCollectionView.ScrollTo(count - 1, position: ScrollToPosition.End, animate: true);
        }
    }

    private void OnVolverClicked(object sender, EventArgs e) =>
        Shell.Current.GoToAsync("..");
}
