using Kanban.Interfaces;
using Kanban.Models;
using Kanban.ViewModels.Tareas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using Kanban.Repositorios;

public class TareasController : Controller
{
    private readonly ITareaRepository _tareaRepository;
    private readonly ILogger<TareasController> _logger;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITableroRepository _tableroRepository;

    public TareasController(ITareaRepository tareaRepository, ILogger<TareasController> logger, IUsuarioRepository usuarioRepository, ITableroRepository tableroRepository)
    {
        _tareaRepository = tareaRepository;
        _logger = logger;
        _usuarioRepository = usuarioRepository;
        _tableroRepository = tableroRepository;
    }
    private int? ObtenerUsuarioId()
    {
        try
        {
            string usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            return string.IsNullOrEmpty(usuarioIdStr) ? (int?)null : int.Parse(usuarioIdStr);
        }
        catch
        {
            return null;
        }
    }

    private bool EsAdministrador()
    {
        return HttpContext.Session.GetString("Rol") == "Administrador";
    }

    private bool EsPropietarioTablero(int id)
    {
        var tablero = _tableroRepository.ObtenerTableroPorId(id);
        return tablero?.idUsuarioPropietario == ObtenerUsuarioId();
    }

    private bool PuedeModificarTarea(Tarea tarea)
    {
        var usuarioId = ObtenerUsuarioId();
        return EsAdministrador() || 
               EsPropietarioTablero(tarea.idTablero) || 
               tarea.idUsuarioAsignado == usuarioId;
    }

    private IActionResult ValidarAcceso()
    {
        if (!ObtenerUsuarioId().HasValue)
            return RedirectToAction("Login", "Login");
        
        return null;
    }
    public IActionResult Index(int id)
    {
        try
        {
            ViewBag.PuedeEditar = EsAdministrador() || EsPropietarioTablero(id);
            ViewBag.Id = ObtenerUsuarioId();

            var acceso = ValidarAcceso();
            if (acceso != null) return acceso;

            var tablero = _tableroRepository.ObtenerTableroPorId(id);
            if (tablero == null)
            {
                return NotFound();
            }
            var usuario = _usuarioRepository.ObtenerUsuarioPorId((int)ObtenerUsuarioId());

            var tareas = _tareaRepository.ListarTareasDeTablero(id)
                .Select(t => new TareaViewModel
                {
                    Id = t.id,
                    Nombre = t.nombre,
                    Descripcion = t.descripcion,
                    Color = t.color,
                    Estado = t.estado.ToString(),
                    NombrePropietario = usuario.nombreDeUsuario,
                    IdUsuarioAsignado = t.idUsuarioAsignado
                }).ToList();
            
            var viewModel = new ListarTareasViewModel
            {
                Tareas = tareas,
                IdTablero = id
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar tareas.");
            return View("Error");
        }
    }

    public IActionResult Crear(int id)
    {
        var usuarios = _usuarioRepository.ListarUsuarios() ?? new List<Usuario>();
        
        var viewModel = new CrearTareaViewModel
        {
            IdTablero = id,
            UsuariosDisponibles = usuarios,
            Color = "#FFFFFF"
        };
        return View(viewModel);
    }

    [HttpPost]
    public IActionResult Crear(CrearTareaViewModel model)
    {
        var acceso = ValidarAcceso();
        if (acceso != null) return acceso;

        if (!EsAdministrador() && !EsPropietarioTablero(model.IdTablero))
        {
            ModelState.AddModelError("", "No tienes permisos para crear tareas en este tablero");
            return View(model);
        }

        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _logger.LogError($"Error: {error.ErrorMessage}");
            }
            model.UsuariosDisponibles = _usuarioRepository.ListarUsuarios() ?? new List<Usuario>();
            return View(model);
        }
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Intentando crear tarea en tablero {IdTablero}", model.IdTablero);
                
                var tablero = _tableroRepository.ObtenerTableroPorId(model.IdTablero);
                if (tablero == null)
                {
                    _logger.LogError("Tablero con ID {IdTablero} no encontrado", model.IdTablero);
                    ModelState.AddModelError("IdTablero", "El tablero no existe.");
                    return View(model);
                }

                if (model.IdUsuarioAsignado.HasValue)
                {
                    var usuario = _usuarioRepository.ObtenerUsuarioPorId(model.IdUsuarioAsignado.Value);
                    if (usuario == null)
                    {
                        ModelState.AddModelError("IdUsuarioAsignado", "El usuario no existe.");
                        return View(model);
                    }
                }

                var tarea = new Tarea
                {
                    nombre = model.Nombre,
                    descripcion = model.Descripcion,
                    color = model.Color,
                    estado = model.Estado,
                    idUsuarioAsignado = model.IdUsuarioAsignado,
                    idTablero = model.IdTablero
                };

                var tareaCreada = _tareaRepository.CrearTarea(model.IdTablero, tarea);
                if (tareaCreada == null)
                {
                    ModelState.AddModelError("", "No se pudo crear la tarea.");
                    model.UsuariosDisponibles = _usuarioRepository.ListarUsuarios();
                    return View(model);
                }
                return RedirectToAction("Index", new { id = model.IdTablero });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear tarea.");
            ModelState.AddModelError("", "Ocurrió un error inesperado.");
            model.UsuariosDisponibles = _usuarioRepository.ListarUsuarios();
        }
        return View(model);
    }

    public IActionResult Editar(int id)
    {
        var acceso = ValidarAcceso();
        if (acceso != null) return acceso;
        ViewBag.Id = ObtenerUsuarioId();

        try
        {
            var tarea = _tareaRepository.ObtenerTareaPorId(id);
            var usuarios = _usuarioRepository.ListarUsuarios() ?? new List<Usuario>();

            if (tarea == null)
            {
                return NotFound();
            }

            var viewModel = new ModificarTareaViewModel
            {
                Id = tarea.id,
                Nombre = tarea.nombre,
                Descripcion = tarea.descripcion,
                Color = tarea.color,
                Estado = tarea.estado,
                IdUsuarioAsignado = tarea.idUsuarioAsignado,
                UsuariosDisponibles = usuarios
            };

            ViewBag.EsPropietario = EsPropietarioTablero(viewModel.Id);

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tarea para edición.");
            return View("Error");
        }
    }

    [HttpPost]
    public IActionResult Editar(int id, ModificarTareaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _logger.LogError($"Error: {error.ErrorMessage}");
            }
            model.UsuariosDisponibles = _usuarioRepository.ListarUsuarios() ?? new List<Usuario>();
            return View(model);
        }
        try
        {
            if (ModelState.IsValid)
            {
                var acceso = ValidarAcceso();
                if (acceso != null) return acceso;

                if (model.IdUsuarioAsignado.HasValue)
                {
                    var usuario = _usuarioRepository.ObtenerUsuarioPorId(model.IdUsuarioAsignado.Value);
                    if (usuario == null)
                    {
                        ModelState.AddModelError("IdUsuarioAsignado", "El usuario no existe.");
                        return View(model);
                    }
                }

                var tareaExistente = _tareaRepository.ObtenerTareaPorId(id);
                if (tareaExistente == null) return NotFound();

                bool esAdmin = EsAdministrador();
                bool esPropietario = EsPropietarioTablero(tareaExistente.idTablero);
                bool esAsignado = tareaExistente.idUsuarioAsignado == ObtenerUsuarioId();

                if (!esAdmin && !esPropietario && !esAsignado)
                {
                    return Forbid();
                }

                if (esAsignado && !esAdmin && !esPropietario)
                {
                    var tareaActualizada = new Tarea
                    {
                        id = id,
                        nombre = tareaExistente.nombre,
                        descripcion = tareaExistente.descripcion,
                        color = tareaExistente.color,
                        estado = model.Estado,
                        idUsuarioAsignado = tareaExistente.idUsuarioAsignado,
                        idTablero = tareaExistente.idTablero
                    };

                    bool modificada = _tareaRepository.ModificarTarea(id, tareaActualizada);
                    if (modificada)
                    {
                        return RedirectToAction("Index", new { id = tareaExistente.idTablero });
                    }
                    ModelState.AddModelError("", "No se pudo actualizar el estado");
                    return View(model);
                }

                var tarea = new Tarea
                {
                    id = model.Id,
                    idTablero = id,
                    nombre = model.Nombre,
                    descripcion = model.Descripcion,
                    color = model.Color,
                    estado = model.Estado,
                    idUsuarioAsignado = model.IdUsuarioAsignado
                };

                bool modificadoTotal = _tareaRepository.ModificarTarea(id, tarea);
                if (modificadoTotal)
                {
                    return RedirectToAction("Index", "Tareas", new {id = tareaExistente.idTablero});
                }
                ModelState.AddModelError("", "No se pudo modificar la tarea.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al modificar tarea.");
            ModelState.AddModelError("", "Ocurrió un error inesperado.");
        }
        return View(model);
    }

    public IActionResult Eliminar(int id)
    {
        try
        {
            var tarea = _tareaRepository.ObtenerTareaPorId(id);
            if (tarea == null)
            {
                return NotFound();
            }
            return View(tarea);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tarea para eliminación.");
            return View("Error");
        }
    }

    [HttpPost]
    public IActionResult ConfirmarEliminar(int id)
    {
        try
        {
            var acceso = ValidarAcceso();
            if (acceso != null) return acceso;

            var tarea = _tareaRepository.ObtenerTareaPorId(id);
            if (!EsAdministrador() && !EsPropietarioTablero(tarea.idTablero))
            {
                return Forbid();
            }
            if (tarea == null)
            {
                return NotFound();
            }

            bool eliminado = _tareaRepository.EliminarTarea(id);
            if (eliminado)
            {
                return RedirectToAction("Index", "Tareas", new {id = tarea.idTablero});
            }
            ModelState.AddModelError("", "No se pudo eliminar la tarea.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar tarea.");
            ModelState.AddModelError("", "Ocurrió un error inesperado.");
        }
        return RedirectToAction("Index");
    }
}