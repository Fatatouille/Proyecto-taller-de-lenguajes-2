using System.ComponentModel.DataAnnotations;
using Kanban.Models;

namespace Kanban.ViewModels.Usuarios
{
    public class ModificarUsuarioViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        public string NombreDeUsuario { get; set; }
        
        public string Password { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio.")]
        public RolUsuario Rol { get; set; }
    }
}