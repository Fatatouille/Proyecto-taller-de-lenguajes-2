namespace Kanban.Models
{
    public enum RolUsuario
    {
        Administrador = 1,
        Operador = 0
    }

    public enum EstadoTarea
    {
        Ideas= 0,
        ToDo = 1,
        Doing = 2,
        Review = 3,
        Done = 4
    }
}