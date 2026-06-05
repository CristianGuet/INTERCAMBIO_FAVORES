using AppFavoresEscritorio.VistaModelos.Autenticacion;

namespace AppFavoresEscritorio.Vistas.Autenticacion;

public partial class PaginaRegistro : ContentPage
{
    public PaginaRegistro(RegistroVistaModelo vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (Shell.Current is not null)
            Shell.SetTabBarIsVisible(Shell.Current, true);
    }
}
