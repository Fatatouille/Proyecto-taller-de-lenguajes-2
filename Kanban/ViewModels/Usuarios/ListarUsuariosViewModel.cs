using Kanban.Models;

namespace Kanban.ViewModels.Usuarios
{
    public class ListarUsuariosViewModel
    {
        public List<UsuarioViewModel> Usuarios { get; set; }
    }

    public class UsuarioViewModel
    {
        public int Id { get; set; }
        public string NombreDeUsuario { get; set; }
        public string Rol { get; set; }
    }
}