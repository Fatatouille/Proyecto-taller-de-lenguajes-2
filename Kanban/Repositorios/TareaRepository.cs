using Kanban.Models;
using Kanban.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Kanban.Repositorios
{
    public class TareaRepository : ITareaRepository
    {
        private readonly string _cadenaDeConexion;
        private readonly ILogger<TareaRepository> _logger;

        public TareaRepository(string cadenaDeConexion, ILogger<TareaRepository> logger)
        {
            _cadenaDeConexion = cadenaDeConexion;
            _logger = logger;
        }

        public Tarea CrearTarea(int idTablero, Tarea tarea)
        {
            string query = "INSERT INTO Tarea (nombre, descripcion, color, estado, id_tablero, id_usuario_asignado) VALUES ($nombre, $descripcion, $color, $estado, $idTablero, $idUsuarioAsignado)";
            try
            {
                using (SqliteConnection conexion = new SqliteConnection(_cadenaDeConexion))
                {
                    conexion.Open();
                    SqliteCommand command = new SqliteCommand(query, conexion);
                    command.Parameters.AddWithValue("$nombre", tarea.nombre);
                    command.Parameters.AddWithValue("$descripcion", tarea.descripcion);
                    command.Parameters.AddWithValue("$color", tarea.color);
                    command.Parameters.AddWithValue("$estado", (int)tarea.estado);
                    command.Parameters.AddWithValue("$idTablero", idTablero);
                    command.Parameters.AddWithValue("$idUsuarioAsignado", tarea.idUsuarioAsignado);
                    int cantidad_filas = command.ExecuteNonQuery();
                    if (cantidad_filas > 0)
                    {
                        command.CommandText = "SELECT last_insert_rowid()";
                        tarea.id = (int)(long)command.ExecuteScalar();
                        return tarea;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear tarea: {Tarea}", tarea.nombre);
            }
            return null;
        }

        public bool ModificarTarea(int id, Tarea tarea)
        {
            string query = "UPDATE Tarea SET nombre = $nombre, descripcion = $descripcion, color = $color, estado = $estado, id_usuario_asignado = $asignado WHERE id = $id";
            try
            {
                using (SqliteConnection conexion = new SqliteConnection(_cadenaDeConexion))
                {
                    conexion.Open();
                    SqliteCommand command = new SqliteCommand(query, conexion);
                    command.Parameters.AddWithValue("$nombre", tarea.nombre);
                    command.Parameters.AddWithValue("$descripcion", tarea.descripcion);
                    command.Parameters.AddWithValue("$color", tarea.color);
                    command.Parameters.AddWithValue("$estado", (int)tarea.estado);
                    command.Parameters.AddWithValue("$asignado", tarea.idUsuarioAsignado);
                    command.Parameters.AddWithValue("$id", id);
                    int cantidad_filas = command.ExecuteNonQuery();
                    return cantidad_filas > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar tarea con ID {Id}", id);
                return false;
            }
        }

        public List<Tarea> ListarTareasDeTablero(int idTablero)
        {
            string query = "SELECT * FROM Tarea WHERE id_tablero = $idTablero";
            var tareas = new List<Tarea>();
            try
            {
                using (SqliteConnection conexion = new SqliteConnection(_cadenaDeConexion))
                {
                    conexion.Open();
                    SqliteCommand command = new SqliteCommand(query, conexion);
                    command.Parameters.AddWithValue("$idTablero", idTablero);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tareas.Add(new Tarea
                            {
                                id = Convert.ToInt32(reader["id"]),
                                nombre = reader["nombre"].ToString(),
                                descripcion = reader["descripcion"].ToString(),
                                color = reader["color"].ToString(),
                                estado = (EstadoTarea)Convert.ToInt32(reader["estado"]),
                                idTablero = Convert.ToInt32(reader["id_tablero"]),
                                idUsuarioAsignado = reader["id_usuario_asignado"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["id_usuario_asignado"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar tareas de tablero");
            }
            return tareas;
        }

        public Tarea ObtenerTareaPorId(int id)
        {
            string query = "SELECT * FROM Tarea WHERE id = $id";
            try
            {
                using (SqliteConnection conexion = new SqliteConnection(_cadenaDeConexion))
                {
                    conexion.Open();
                    SqliteCommand command = new SqliteCommand(query, conexion);
                    command.Parameters.AddWithValue("$id", id);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Tarea
                            {
                                id = Convert.ToInt32(reader["id"]),
                                nombre = reader["nombre"].ToString(),
                                descripcion = reader["descripcion"].ToString(),
                                color = reader["color"].ToString(),
                                estado = (EstadoTarea)Convert.ToInt32(reader["estado"]),
                                idTablero = Convert.ToInt32(reader["id_tablero"]),
                                idUsuarioAsignado = reader["id_usuario_asignado"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["id_usuario_asignado"])
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tarea con ID {Id}", id);
            }
            return null;
        }

        public bool EliminarTarea(int id)
        {
            string query = "DELETE FROM Tarea WHERE id = $id";
            try
            {
                using (SqliteConnection conexion = new SqliteConnection(_cadenaDeConexion))
                {
                    conexion.Open();
                    SqliteCommand command = new SqliteCommand(query, conexion);
                    command.Parameters.AddWithValue("$id", id);
                    int cantidad_filas = command.ExecuteNonQuery();
                    return cantidad_filas > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar tarea con ID {Id}", id);
                return false;
            }
        }

        public bool AsignarUsuarioATarea(int idTarea, int idUsuario)
        {
            string query = "UPDATE Tarea SET id_usuario_asignado = $idUsuario WHERE id = $idTarea";
            try
            {
                using (SqliteConnection conexion = new SqliteConnection(_cadenaDeConexion))
                {
                    conexion.Open();
                    SqliteCommand command = new SqliteCommand(query, conexion);
                    command.Parameters.AddWithValue("$idUsuario", idUsuario);
                    command.Parameters.AddWithValue("$idTarea", idTarea);
                    int cantidad_filas = command.ExecuteNonQuery();
                    return cantidad_filas > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar usuario con ID {IdUsuario} a tarea con ID {IdTarea}", idUsuario, idTarea);
                return false;
            }
        }

        public List<Tarea> ListarTareasDeUsuario(int idUsuario)
        {
            string query = "SELECT * FROM Tarea WHERE id_usuario_asignado = $idUsuario";
            var tareas = new List<Tarea>();
            try
            {
                using (SqliteConnection conexion = new SqliteConnection(_cadenaDeConexion))
                {
                    conexion.Open();
                    SqliteCommand command = new SqliteCommand(query, conexion);
                    command.Parameters.AddWithValue("$idUsuario", idUsuario);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tareas.Add(new Tarea
                            {
                                id = Convert.ToInt32(reader["id"]),
                                nombre = reader["nombre"].ToString(),
                                descripcion = reader["descripcion"].ToString(),
                                color = reader["color"].ToString(),
                                estado = (EstadoTarea)Convert.ToInt32(reader["estado"]),
                                idTablero = Convert.ToInt32(reader["id_tablero"]),
                                idUsuarioAsignado = reader["id_usuario_asignado"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["id_usuario_asignado"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar tareas de usuario");
            }
            return tareas;
        }
    }
}