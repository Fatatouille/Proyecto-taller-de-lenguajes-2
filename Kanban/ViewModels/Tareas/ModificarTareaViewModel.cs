using System.ComponentModel.DataAnnotations;
using Kanban.Models;

namespace Kanban.ViewModels.Tareas
{
    public class ModificarTareaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }
        public string Color { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        public EstadoTarea Estado { get; set; }
        public int? IdUsuarioAsignado { get; set; }
        public List<Usuario>? UsuariosDisponibles { get; set; }
    }
}