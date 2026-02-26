namespace Kanban.Models
{
    public class Usuario
    {
        private int Id;
        private string NombreDeUsuario;
        private string Password;
        private RolUsuario Rol;

        public int id {get=>Id;set=>Id=value;}
        public string nombreDeUsuario {get=>NombreDeUsuario; set=> NombreDeUsuario=value;}
        public string password {get=>Password; set=>Password=value;}
        public RolUsuario rol {get=>Rol; set=>Rol=value;}

        public Usuario(){}
    }
}