using System.ComponentModel.DataAnnotations;
using Kanban.Models;

namespace Kanban.ViewModels.Tableros
{
    public class ModificarTableroViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }
        public int? IdUsuarioAsignado { get; set; }
        public List<Usuario>? UsuariosDisponibles { get; set; }
    }
}