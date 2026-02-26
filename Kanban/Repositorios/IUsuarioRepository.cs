using Kanban.Models;

namespace Kanban.Interfaces
{
    public interface IUsuarioRepository
    {
        bool CrearUsuario(Usuario usuario);
        bool ModificarUsuario(int id, Usuario usuario);
        List<Usuario> ListarUsuarios();
        Usuario ObtenerUsuarioPorId(int id);
        bool EliminarUsuario(int id);
        bool CambiarPassword(int id, string nuevaPassword);
    }
}