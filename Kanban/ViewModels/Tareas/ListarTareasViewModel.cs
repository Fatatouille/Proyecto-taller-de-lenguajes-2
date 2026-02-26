using Kanban.Models;

namespace Kanban.ViewModels.Tareas
{
    public class ListarTareasViewModel
    {
        public List<TareaViewModel> Tareas { get; set; }
        public int IdTablero { get; set; }
    }

    public class TareaViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Color { get; set; }
        public string Estado { get; set; }
        public string NombrePropietario { get; set; }
        public int? IdUsuarioAsignado { get; set; }
        
    }
}