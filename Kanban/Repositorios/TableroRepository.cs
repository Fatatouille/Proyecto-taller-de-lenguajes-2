using Kanban.Models;
using Kanban.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Kanban.Repositorios
{
    public class TableroRepository : ITableroRepository
    {
        private readonly string _cadenaDeConexion;
        private readonly ILogger<TableroRepository> _logger;

        public TableroRepository(string cadenaDeConexion, ILogger<TableroRepository> logger)
        {
            _cadenaDeConexion = cadenaDeConexion;
            _logger = logger;
        }

        public bool CrearTablero(Tablero tablero)
        {
            string query = "INSERT INTO Tablero (nombre, descripcion, id_usuario_propietario) VALUES ($nombre, $descripcion, $idUsuarioPropietario)";
            try
            {
                using (SqliteConnection conexion = new SqliteConnection(_cadenaDeConexion))
                {
                    conexion.Open();
                    SqliteCommand command = new SqliteCommand(query, conexion);
                    command.Parameters.AddWithValue("$nombre", tablero.nombre);
                    command.Parameters.AddWithValue("$descripcion", tablero.descripcion);
                    command.Parameters.AddWithValue("$idUsuarioPropietario", tablero.idUsuarioPropietario);
                    int cantidad_filas = command.ExecuteNonQuery();
                    return cantidad_filas > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear tablero: {Tablero}", tablero.nombre);
                return false;
            }
        }

        public bool ModificarTablero(int id, Tablero tablero)
        {
            string query = "UPDATE Tablero SET nombre = $nombre, descripcion = $descripcion, id_usuario_propietario = $idUsuarioPropietario WHERE id = $id";
            try
            {
                using (SqliteConnection conexion = new SqliteConnection(_cadenaDeConexion))
                {
                    conexion.Open();
                    SqliteCommand command = new SqliteCommand(query, conexion);
                    command.Parameters.AddWithValue("$nombre", tablero.nombre);
                    command.Parameters.AddWithValue("$descripcion", tablero.descripcion);
                    command.Parameters.AddWithValue("$idUsuarioPropietario", tablero.idUsuarioPropietario);
                    command.Parameters.AddWithValue("$id", id);
                    int cantidad_filas = command.ExecuteNonQuery();
                    return cantidad_filas > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar tablero con ID {Id}", id);
                return false;
            }
        }

        public List<Tablero> ListarTableros()
        {
            string query = "SELECT * FROM Tablero";
            var tableros = new List<Tablero>();
            try
            {
                using (SqliteConnection conexion = new SqliteConnection(_cadenaDeConexion))
                {
                    conexion.Open();
                    SqliteCommand command = new SqliteCommand(query, conexion);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tableros.Add(new Tablero
                            {
                                id = Convert.ToInt32(reader["id"]),
                                nombre = reader["nombre"].ToString(),
                                descripcion = reader["descripcion"].ToString(),
                                idUsuarioPropietario = Convert.ToInt32(reader["id_usuario_propietario"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar tableros");
            }
            return tableros;
        }

        public List<Tablero> ListarTablerosPorUsuario(int idUsuario)
        {
            string query = "SELECT * FROM Tablero WHERE id_usuario_propietario = $idUsuarioPropietario";
            var tableros = new List<Tablero>();
            try
            {
                using (SqliteConnection conexion = new SqliteConnection(_cadenaDeConexion))
                {
                    conexion.Open();
                    SqliteCommand command = new SqliteCommand(query, conexion);
                    command.Parameters.AddWithValue("$idUsuarioPropietario", idUsuario);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tableros.Add(new Tablero
                            {
                                id = Convert.ToInt32(reader["id"]),
                                nombre = reader["nombre"].ToString(),
                                descripcion = reader["descripcion"].ToString(),
                                idUsuarioPropietario = Convert.ToInt32(reader["id_usuario_propietario"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar tableros por usuario");
            }
            return tableros;
        }

        public Tablero ObtenerTableroPorId(int id)
        {
            string query = "SELECT * FROM Tablero WHERE id = $id";
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
                            return new Tablero
                            {
                                id = Convert.ToInt32(reader["id"]),
                                nombre = reader["nombre"].ToString(),
                                descripcion = reader["descripcion"].ToString(),
                                idUsuarioPropietario = Convert.ToInt32(reader["id_usuario_propietario"])
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tablero con ID {Id}", id);
            }
            return null;
        }

        public bool EliminarTablero(int id)
        {
            string query = "DELETE FROM Tablero WHERE id = $id";
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
                _logger.LogError(ex, "Error al eliminar tablero con ID {Id}", id);
                return false;
            }
        }
    }
}