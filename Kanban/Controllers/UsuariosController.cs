using Kanban.Interfaces;
using Kanban.Models;
using Kanban.ViewModels.Usuarios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using SQLitePCL;

public class UsuariosController : Controller
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(IUsuarioRepository usuarioRepository, ILogger<UsuariosController> logger)
    {
        _usuarioRepository = usuarioRepository;
        _logger = logger;
    }

    private bool EsAdministrador()
    {
        return HttpContext.Session.GetString("Rol") == "Administrador";
    }

    public IActionResult Index()
    {
        if (!EsAdministrador()) return RedirectToAction("Login", "Login");

        try
        {
            var usuarios = _usuarioRepository.ListarUsuarios()
                .Select(u => new UsuarioViewModel
                {
                    Id = u.id,
                    NombreDeUsuario = u.nombreDeUsuario,
                    Rol = u.rol.ToString()
                }).ToList();

            var viewModel = new ListarUsuariosViewModel
            {
                Usuarios = usuarios
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar usuarios.");
            return View("Error");
        }
    }

    public IActionResult Crear()
    {
        if (!EsAdministrador()) return RedirectToAction("Login", "Login");
        return View(new CrearUsuarioViewModel());
    }

    [HttpPost]
    public IActionResult Crear(CrearUsuarioViewModel model)
    {
        if (!EsAdministrador()) return RedirectToAction("Login", "Login");

        try
        {
            if (ModelState.IsValid)
            {
                var usuario = new Usuario
                {
                    nombreDeUsuario = model.NombreDeUsuario,
                    password = model.Password,
                    rol = model.Rol
                };

                bool creado = _usuarioRepository.CrearUsuario(usuario);
                if (creado)
                    return RedirectToAction("Index");

                ModelState.AddModelError("", "El usuario ya existe.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario.");
            ModelState.AddModelError("", "Ocurrió un error inesperado.");
        }
        return View(model);
    }

    public IActionResult Editar(int id)
    {
        if (!EsAdministrador()) return RedirectToAction("Login", "Login");

        try
        {
            var usuario = _usuarioRepository.ObtenerUsuarioPorId(id);
            if (usuario == null)
                return RedirectToAction("Index");

            var viewModel = new ModificarUsuarioViewModel
            {
                Id = usuario.id,
                NombreDeUsuario = usuario.nombreDeUsuario,
                Password = usuario.password,
                Rol = usuario.rol
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario para edición.");
            return View("Error");
        }
    }

    [HttpPost]
    public IActionResult Editar(int id, ModificarUsuarioViewModel model)
    {
        if (!EsAdministrador()) return RedirectToAction("Login", "Login");

        try
        {
            if (ModelState.IsValid)
            {
                var usuarioExistente = _usuarioRepository.ObtenerUsuarioPorId(id);
                if (usuarioExistente == null)
                {
                    return NotFound();
                }

                string nuevaPassword = string.IsNullOrEmpty(model.Password) 
                    ? usuarioExistente.password 
                    : model.Password;

                var usuario = new Usuario
                {
                    id = model.Id,
                    nombreDeUsuario = model.NombreDeUsuario,
                    password = nuevaPassword,
                    rol = model.Rol
                };

                bool modificado = _usuarioRepository.ModificarUsuario(id, usuario);
                
                if (modificado)return RedirectToAction("Index");

                ModelState.AddModelError("", "No se pudo modificar el usuario.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al modificar usuario.");
            ModelState.AddModelError("", "Ocurrió un error inesperado.");
        }
        return View(model);
    }

    public IActionResult Eliminar(int id)
    {
        if (!EsAdministrador()) return RedirectToAction("Login", "Login");

        try
        {
            var usuario = _usuarioRepository.ObtenerUsuarioPorId(id);
            if (usuario == null)
                return RedirectToAction("Index");

            return View(usuario);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario para eliminación.");
            return View("Error");
        }
    }

    [HttpPost]
    public IActionResult ConfirmarEliminar(int id)
    {
        if (!EsAdministrador()) return RedirectToAction("Login", "Login");

        try
        {
            bool eliminado = _usuarioRepository.EliminarUsuario(id);
            if (eliminado)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "No se pudo eliminar el usuario.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar usuario.");
            ModelState.AddModelError("", "Ocurrió un error inesperado.");
        }
        return RedirectToAction("Index");
    }
}
