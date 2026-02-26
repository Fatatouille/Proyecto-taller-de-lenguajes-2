using Kanban.Models;

namespace Kanban.Interfaces
{
    public interface ITareaRepository
    {
        Tarea CrearTarea(int idTablero, Tarea tarea);
        bool ModificarTarea(int id, Tarea tarea);
        Tarea ObtenerTareaPorId(int id);
        List<Tarea> ListarTareasDeUsuario(int idUsuario);
        List<Tarea> ListarTareasDeTablero(int idTablero);
        bool EliminarTarea(int idTarea);
        bool AsignarUsuarioATarea(int idUsuario, int idTarea);
    }
}