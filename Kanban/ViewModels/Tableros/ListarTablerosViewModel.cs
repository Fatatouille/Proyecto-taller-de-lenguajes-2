using Kanban.Models;
using System.Collections.Generic;

namespace Kanban.ViewModels.Tableros
{
    public class ListarTablerosViewModel
    {
        public List<TableroViewModel> Tableros { get; set; }
    }

    public class TableroViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string NombrePropietario { get; set; }
        public List<TareaViewModel> Tareas { get; set; } = new List<TareaViewModel>();
        
        public bool EsPropietario {get; set; }
    }

    public class TareaViewModel
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
    }
}