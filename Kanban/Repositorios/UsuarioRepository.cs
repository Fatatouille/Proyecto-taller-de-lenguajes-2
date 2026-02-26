using Kanban.Models;
using Kanban.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.Repositorios
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly string _cadenaDeConexion;
        private readonly ILogger<UsuarioRepository> _logger;

        public UsuarioRepository(string cadenaDeConexion, ILogger<UsuarioRepository> logger)
        {
            _cadenaDeConexion = cadenaDeConexion;
            _logger = logger;
        }

        public bool CrearUsuario(Usuario usuario)
        {
            string query = "INSERT INTO Usuario (nombre_de_usuario, password, rolusuario) VALUES ($nombreDeUsuario, $password, $rolUsuario)";
            try
            {
                using (SqliteConnection conexion = new SqliteConnection(_cadenaDeConexion))
                {
                    conexion.Open();
                    SqliteCommand command = new SqliteCommand(query, conexion);
                    command.Parameters.AddWithValue("$nombreDeUsuario", usuario.nombreDeUsuario);
                    command.Parameters.AddWithValue("$password", usuario.password);
                    command.Parameters.AddWithValue("$rolUsuario", (int)usuario.rol);
                    int cantidad_filas = command.ExecuteNonQuery();
                    return cantidad_filas > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario: {Usuario}", usuario.nombreDeUsuario);
                return false;
            }
        }

        public bool ModificarUsuario(int id, Usuario usuario)
        {
            string query = "UPDATE Usuario SET nombre_de_usuario = $nombreDeUsuario, password = $password, rolusuario = $rolUsuario WHERE id = $id";
            try
            {
                using (SqliteConnection conexion = new SqliteConnection(_cadenaDeConexion))
                {
                    conexion.Open();
                    SqliteCommand command = new SqliteCommand(query, conexion);
                    command.Parameters.AddWithValue("$nombreDeUsuario", usuario.nombreDeUsuario);
                    command.Parameters.AddWithValue("$password", usuario.password);
                    command.Parameters.AddWithValue("$rolUsuario", (int)usuario.rol);
                    command.Parameters.AddWithValue("$id", id);
                    int cantidad_filas = command.ExecuteNonQuery();
                    return cantidad_filas > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar usuario con ID {Id}", id);
                return false;
            }
        }

        public List<Usuario> ListarUsuarios()
        {
            string query = "SELECT * FROM Usuario";
            var usuarios = new List<Usuario>();
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
                            usuarios.Add(new Usuario
                            {
                                id = Convert.ToInt32(reader["id"]),
                                nombreDeUsuario = reader["nombre_de_usuario"].ToString(),
                                password = reader["password"].ToString(),
                                rol = (RolUsuario)Convert.ToInt32(reader["rolusuario"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar usuarios");
            }
            return usuarios;
        }

        public Usuario ObtenerUsuarioPorId(int id)
        {
            string query = "SELECT * FROM Usuario WHERE id = $id";
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
                            return new Usuario
                            {
                                id = Convert.ToInt32(reader["id"]),
                                nombreDeUsuario = reader["nombre_de_usuario"].ToString(),
                                password = reader["password"].ToString(),
                                rol = (RolUsuario)Convert.ToInt32(reader["rolusuario"])
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario con ID {Id}", id);
            }
            return null;
        }

        public bool EliminarUsuario(int id)
        {
            string query = "DELETE FROM Usuario WHERE id = $id";
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
                _logger.LogError(ex, "Error al eliminar usuario con ID {Id}", id);
                return false;
            }
        }

        public bool CambiarPassword(int id, string nuevaPassword)
        {
            string query = "UPDATE Usuario SET password = $password WHERE id = $id";
            try
            {
                using (SqliteConnection conexion = new SqliteConnection(_cadenaDeConexion))
                {
                    conexion.Open();
                    SqliteCommand command = new SqliteCommand(query, conexion);
                    command.Parameters.AddWithValue("$password", nuevaPassword);
                    command.Parameters.AddWithValue("$id", id);
                    int cantidad_filas = command.ExecuteNonQuery();
                    return cantidad_filas > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar la contraseña del usuario con ID {Id}", id);
                return false;
            }
        }
    }
}