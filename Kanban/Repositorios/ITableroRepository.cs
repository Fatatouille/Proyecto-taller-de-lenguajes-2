using Kanban.Models;

namespace Kanban.Interfaces
{
    public interface ITableroRepository
    {
        bool CrearTablero(Tablero tablero);
        bool ModificarTablero(int id, Tablero tablero);
        Tablero ObtenerTableroPorId(int id);
        List<Tablero> ListarTableros();
        List<Tablero> ListarTablerosPorUsuario(int idUsuario);
        bool EliminarTablero(int id);
    }
}