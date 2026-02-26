using System.ComponentModel.DataAnnotations;
using Kanban.Models;

namespace Kanban.ViewModels.Usuarios
{
    public class CrearUsuarioViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        public string NombreDeUsuario { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, ErrorMessage = "La contraseña debe tener al menos {2} y un máximo de {1} caracteres.", MinimumLength = 8)]
        public string Password { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio.")]
        public RolUsuario Rol { get; set; }
    }
}