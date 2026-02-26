using Kanban.Interfaces;
using Kanban.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

public class LoginController : Controller
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ILogger<LoginController> _logger;

    public LoginController(IUsuarioRepository usuarioRepository, ILogger<LoginController> logger)
    {
        _usuarioRepository = usuarioRepository;
        _logger = logger;
    }

    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            _logger.LogInformation("Intento de inicio de sesión para el usuario: {NombreDeUsuario}", model.NombreDeUsuario);

            var usuario = _usuarioRepository.ListarUsuarios()
                .FirstOrDefault(u => u.nombreDeUsuario == model.NombreDeUsuario);

            if (usuario == null)
            {
                _logger.LogWarning("Intento de inicio de sesión fallido: Usuario no encontrado.");
                ViewData["Error"] = "Usuario o contraseña incorrectos.";
                return View(model);
            }

            if (usuario.password != model.Password)
            {
                _logger.LogWarning("Intento de inicio de sesión fallido: Contraseña incorrecta para {NombreDeUsuario}.", model.NombreDeUsuario);
                ViewData["Error"] = "Usuario o contraseña incorrectos.";
                return View(model);
            }

            HttpContext.Session.SetString("UsuarioId", usuario.id.ToString());
            HttpContext.Session.SetString("Rol", usuario.rol.ToString());
            _logger.LogInformation("Inicio de sesión exitoso para {NombreDeUsuario}.", model.NombreDeUsuario);
            return RedirectToAction("Index", "Tableros");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en el proceso de autenticación.");
            ViewData["Error"] = "Ocurrió un error inesperado. Inténtelo nuevamente.";
            return View(model);
        }
    }

    public IActionResult Logout()
    {
        try
        {
            HttpContext.Session.Clear();
            _logger.LogInformation("Usuario cerró sesión correctamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cerrar sesión.");
        }

        return RedirectToAction("Login", "Login");
    }
}