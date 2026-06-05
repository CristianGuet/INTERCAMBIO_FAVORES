using AppFavoresEscritorio.Navegacion;
using AppFavoresEscritorio.Singleton;
using AppFavoresEscritorio.VistaModelos.Autenticacion;

namespace AppFavoresEscritorio.Vistas.Autenticacion;

public partial class PaginaLogin : ContentPage
{
    public PaginaLogin(LoginVistaModelo vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (SesionApp.Instancia.EstaLogueado)SesionApp.Instancia.Limpiar();
    }
}