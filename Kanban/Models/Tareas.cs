namespace Kanban.Models
{
    public class Tarea
    {
        private int Id;
        private int IdTablero;
        private string Nombre;
        private string Descripcion;
        private string Color;
        private EstadoTarea Estado;
        private int? IdUsuarioAsignado;

        public int id {get=>Id; set=> Id=value;}
        public int idTablero {get=>IdTablero; set=> IdTablero=value;}
        public string nombre {get=>Nombre; set=> Nombre=value;}
        public string descripcion {get=>Descripcion; set=>Descripcion=value;}
        public string color {get=>Color; set=>Color=value;}
        public EstadoTarea estado {get=>Estado; set=>Estado=value;}
        public int? idUsuarioAsignado {get=>IdUsuarioAsignado; set =>IdUsuarioAsignado=value;}

        public Tarea()
        {
            this.IdUsuarioAsignado = null;
        }
    }
}