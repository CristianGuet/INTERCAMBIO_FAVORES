using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AppFavoresEscritorio.Modelos.Autenticacion;
using AppFavoresEscritorio.Modelos.Favores;
using AppFavoresEscritorio.Modelos.Grupos;
using AppFavoresEscritorio.Servicios;
 
namespace AppFavoresEscritorio.VistaModelos.Administracion;
 
public class AdministracionVistaModelo : INotifyPropertyChanged
{
    private readonly ServicioAdmin _servicioAdmin;
    private readonly ServicioAutenticacion _servicioAuth;
 
    private bool _isRefreshing;
    private bool _isCargando;
    private List<object> _items = new();
    private int _categoriaSeleccionada;
 
    public event PropertyChangedEventHandler? PropertyChanged;
 
    public AdministracionVistaModelo(ServicioAdmin servicioAdmin, ServicioAutenticacion servicioAuth)
    {
        _servicioAdmin = servicioAdmin;
        _servicioAuth = servicioAuth;
        RefrescarCommand = new Command(async () => await CargarDatosAsync());
        EliminarCommand = new Command<object>(async (item) => await EliminarItemAsync(item));
    }
 
    public ICommand RefrescarCommand { get; }
    public ICommand EliminarCommand { get; }
 
    public bool IsRefreshing
    {
        get => _isRefreshing;
        set { _isRefreshing = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRefreshing))); }
    }
 
    public bool IsCargando
    {
        get => _isCargando;
        set { _isCargando = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCargando))); }
    }
 
    public List<object> Items
    {
        get => _items;
        set { _items = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Items))); }
    }
 
    public int CategoriaSeleccionada
    {
        get => _categoriaSeleccionada;
        set { _categoriaSeleccionada = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CategoriaSeleccionada))); }
    }
 
    public async Task CargarDatosAsync()
    {
        if (IsCargando) return;
 
        try
        {
            IsCargando = true;
            IsRefreshing = true;
 
            var token = await _servicioAuth.ObtenerTokenAsync();
            if (string.IsNullOrWhiteSpace(token)) return;
 
            _servicioAdmin.AplicarToken(token);
 
            if (CategoriaSeleccionada == 0) // Usuarios
            {
                var usuarios = await _servicioAdmin.ObtenerTodosUsuariosAsync();
                Items = usuarios.Select(u => (object)new ItemWrapper
                {
                    Id = u.Correo,
                    DisplayText = $"{u.NombreUsuario} ({u.Correo}) - Rol: {u.Rol}",
                    Original = u
                }).ToList();
            }
            else if (CategoriaSeleccionada == 1) // Favores
            {
                var favores = await _servicioAdmin.ObtenerTodosFavoresAsync();
                Items = favores.Select(f => (object)new ItemWrapper
                {
                    Id = f.Id,
                    DisplayText = $"{f.Titulo} - {f.Estado} - Ofrecido por: {f.IdUsuarioOfrece}",
                    Original = f
                }).ToList();
            }
            else // Grupos
            {
                var grupos = await _servicioAdmin.ObtenerTodosGruposAsync();
                Items = grupos.Select(g => (object)new ItemWrapper
                {
                    Id = g.Id,
                    DisplayText = $"{g.NombreGrupo} - Miembros: {g.NumeroMiembros}",
                    Original = g
                }).ToList();
            }
        }
        catch (Exception ex)
        {
            // para evitar NullReferenceException durante la navegación
            try
            {
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
            }
            catch { }
        }
        finally
        {
            IsCargando = false;
            IsRefreshing = false;
        }
    }
 
    private async Task EliminarItemAsync(object item)
    {
        if (item is not ItemWrapper wrapper) return;
 
        bool confirm;
        try
        {
            confirm = await Shell.Current.DisplayAlertAsync(
                "Confirmar", $"¿Eliminar {wrapper.DisplayText}?", "Sí", "No");
        }
        catch
        {
            return;
        }
 
        if (!confirm) return;
 
        try
        {
            var token = await _servicioAuth.ObtenerTokenAsync();
            if (string.IsNullOrWhiteSpace(token)) return;
 
            _servicioAdmin.AplicarToken(token);
 
            if (CategoriaSeleccionada == 0)
                await _servicioAdmin.EliminarUsuarioAsync(wrapper.Id);
            else if (CategoriaSeleccionada == 1)
                await _servicioAdmin.EliminarFavorAsync(wrapper.Id);
            else
                await _servicioAdmin.EliminarGrupoAsync(wrapper.Id);
 
            await CargarDatosAsync();
        }
        catch (Exception ex)
        {
            try
            {
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
            }
            catch { }
        }
    }
 
    private class ItemWrapper
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayText { get; set; } = string.Empty;
        public object Original { get; set; } = null!;
    }
}