using AppFavoresEscritorio.Navegacion;
using Microsoft.Extensions.DependencyInjection;

namespace AppFavoresEscritorio;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var servicios = IPlatformApplication.Current?.Services
            ?? throw new InvalidOperationException("Servicios MAUI no disponibles.");

        var shell = servicios.GetRequiredService<AppShell>();
        shell.InicializarApariencia();
        return new Window(shell);
    }
}
