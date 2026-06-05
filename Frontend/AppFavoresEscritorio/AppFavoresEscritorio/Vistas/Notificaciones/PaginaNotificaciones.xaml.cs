using AppFavoresEscritorio.VistaModelos.Notificaciones;

namespace AppFavoresEscritorio.Vistas.Notificaciones;

public partial class PaginaNotificaciones : ContentPage
{
    public PaginaNotificaciones(NotificacionesVistaModelo vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
