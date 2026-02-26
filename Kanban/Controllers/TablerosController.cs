using Kanban.Interfaces;
using Kanban.Models;
using Kanban.ViewModels.Tableros;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

public class TablerosController : Controller
{
    private readonly ITableroRepository _tableroRepository;
    private readonly ITareaRepository _tareaRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    private readonly ILogger<TablerosController> _logger;

    public TablerosController(ITableroRepository tableroRepository, IUsuarioRepository usuarioRepository, ITareaRepository tareaRepository, ILogger<TablerosController> logger)
    {
        _tableroRepository = tableroRepository;
        _tareaRepository = tareaRepository;
        _usuarioRepository = usuarioRepository;
        _logger = logger;
    }

    public IActionResult Index(int? idPropietario)
    {
        try
        {
            int? usuarioId = ObtenerUsuarioId();
            if (usuarioId == null)
                return RedirectToAction("Login", "Login");

            string rolUsuario = HttpContext.Session.GetString("Rol");
            ViewBag.IdPropietario = idPropietario;

            var tablerosQuery = _tableroRepository.ListarTableros().AsQueryable();

            if (rolUsuario == "Operador")
            {
                // Si es operador y está en "Todos los Tableros", no filtramos
            }

            if (idPropietario.HasValue)
            {
                tablerosQuery = tablerosQuery.Where(t => t.idUsuarioPropietario == idPropietario.Value);
            }

            var tableros = tablerosQuery
                .Select(t => new TableroViewModel
                {
                    Id = t.id,
                    Nombre = t.nombre,
                    Descripcion = t.descripcion,
                    NombrePropietario = _usuarioRepository.ObtenerUsuarioPorId(t.idUsuarioPropietario).nombreDeUsuario,
                    EsPropietario = t.idUsuarioPropietario == usuarioId
                }).ToList();

            var viewModel = new ListarTablerosViewModel
            {
                Tableros = tableros
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar los tableros.");
            return View("Error");
        }
    }

    public IActionResult MisTableros()
    {
        try
        {
            int? usuarioId = ObtenerUsuarioId();
            if (usuarioId == null)
                return RedirectToAction("Login", "Login");

            var usuario = _usuarioRepository.ObtenerUsuarioPorId((int)usuarioId);

            var idsTablerosPropios = _tableroRepository.ListarTablerosPorUsuario(usuarioId.Value)
                .Select(t => t.id)
                .ToList();

            var idsTablerosConTareas = _tareaRepository.ListarTareasDeUsuario(usuarioId.Value)
                .Select(t => t.idTablero)
                .Distinct()
                .ToList();

            var todosIds = idsTablerosPropios
                .Concat(idsTablerosConTareas)
                .Distinct()
                .ToList();

            var tablerosFiltrados = _tableroRepository.ListarTableros()
                .Where(t => todosIds.Contains(t.id))
                .Select(t => new TableroViewModel
                {
                    Id = t.id,
                    Nombre = t.nombre,
                    Descripcion = t.descripcion,
                    NombrePropietario = usuario.nombreDeUsuario,
                    EsPropietario = t.idUsuarioPropietario == usuarioId
                }).ToList();

            var viewModel = new ListarTablerosViewModel { Tableros = tablerosFiltrados };
            return View("Index", viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar los tableros del usuario.");
            return View("Error");
        }
    }

    public IActionResult Crear()
    {
        ViewBag.Id = ObtenerUsuarioId();
        var usuarios = _usuarioRepository.ListarUsuarios() ?? new List<Usuario>();

        if (ObtenerUsuarioId() == null)
            return RedirectToAction("Login", "Login");

        var viewModel = new CrearTableroViewModel
        {
            UsuariosDisponibles = usuarios
        };

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult Crear(CrearTableroViewModel model)
    {
        try
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

            int? usuarioId = ObtenerUsuarioId();
            if (usuarioId == null)
                return RedirectToAction("Login", "Login");

            if (ModelState.IsValid)
            {   
                var tablero = new Tablero
                {
                    idUsuarioPropietario = model.IdUsuarioAsignado.Value,
                    nombre = model.Nombre,
                    descripcion = model.Descripcion
                };

                bool creado = _tableroRepository.CrearTablero(tablero);
                if (creado)
                    return RedirectToAction("Index");

                ModelState.AddModelError("", "No se pudo crear el tablero.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el tablero.");
            ModelState.AddModelError("", "Ocurrió un error inesperado.");
        }
        return View(model);
    }

    public IActionResult Editar(int id)
    {
        try
        {
            ViewBag.Id = ObtenerUsuarioId();
            var usuarios = _usuarioRepository.ListarUsuarios() ?? new List<Usuario>();

            int? usuarioId = ObtenerUsuarioId();
            if (usuarioId == null)
                return RedirectToAction("Login", "Login");

            var tablero = _tableroRepository.ObtenerTableroPorId(id);
            if (tablero == null)
                return NotFound();

            var viewModel = new ModificarTableroViewModel
            {
                Id = tablero.id,
                Nombre = tablero.nombre,
                Descripcion = tablero.descripcion,
                UsuariosDisponibles = usuarios
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el tablero para edición.");
            return View("Error");
        }
    }

    [HttpPost]
    public IActionResult Editar(int id, ModificarTableroViewModel model)
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
            int? usuarioId = ObtenerUsuarioId();
            if (usuarioId == null)
                return RedirectToAction("Login", "Login");

            var tableroExistente = _tableroRepository.ObtenerTableroPorId(id);
            if (tableroExistente == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                var tablero = new Tablero
                {
                    id = model.Id,
                    idUsuarioPropietario = model.IdUsuarioAsignado.Value,
                    nombre = model.Nombre,
                    descripcion = model.Descripcion
                };

                bool modificado = _tableroRepository.ModificarTablero(id, tablero);
                if (modificado)
                    return RedirectToAction("Index");

                ModelState.AddModelError("", "No se pudo modificar el tablero.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al modificar el tablero.");
            ModelState.AddModelError("", "Ocurrió un error inesperado.");
        }
        return View(model);
    }

    public IActionResult Eliminar(int id)
    {
        try
        {
            int? usuarioId = ObtenerUsuarioId();
            if (usuarioId == null)
                return RedirectToAction("Login", "Login");

            var tablero = _tableroRepository.ObtenerTableroPorId(id);
            if (tablero == null)
                return NotFound();

            return View(tablero);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el tablero para eliminación.");
            return View("Error");
        }
    }

    [HttpPost]
    public IActionResult ConfirmarEliminar(int id)
    {
        try
        {
            int? usuarioId = ObtenerUsuarioId();
            if (usuarioId == null)
                return RedirectToAction("Login", "Login");

            var tablero = _tableroRepository.ObtenerTableroPorId(id);
            if (tablero == null)
                return NotFound();

            bool eliminado = _tableroRepository.EliminarTablero(id);
            if (eliminado)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "No se pudo eliminar el tablero.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el tablero.");
            ModelState.AddModelError("", "Ocurrió un error inesperado.");
        }
        return RedirectToAction("Index");
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
}