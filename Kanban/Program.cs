using Kanban.Interfaces;
using Kanban.Repositorios;

var builder = WebApplication.CreateBuilder(args);

string CadenaDeConexion = builder.Configuration.GetConnectionString("SqliteConexion")!.ToString();

builder.Services.AddSingleton<string>(CadenaDeConexion);

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ITareaRepository, TareaRepository>();
builder.Services.AddScoped<ITableroRepository, TableroRepository>();

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "login",
    pattern: "Auth/Login",
    defaults: new { controller = "Login", action = "Login" }
);

app.Run();