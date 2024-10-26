using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using healthycannab.Data;
using healthycannab.Services;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi.Models;
using healthycannab.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Configuración de la cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configuración de identidad con autenticación basada en cookies
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
// Configuración de CORS
builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
    });

builder.Services.AddTransient<ProductoService>();
//servicios de signalR
builder.Services.AddSignalR();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/InicioSesion/Login";  // Página de inicio de sesión
        options.LogoutPath = "/InicioSesion/Logout";
        options.AccessDeniedPath = "/Home/AccessDenied";
    });
//swagger serivicios:
//builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo {Title = "API de Productos", Version = "v1"});
});


//Pago Paypal
builder.Services.AddScoped<PayPalService>();

builder.Services.AddHttpClient<EmailValidation>(); // Agregar HttpClient para EmailValidation
builder.Services.AddControllersWithViews();   // Agregar soporte para controladores y vistas

var app = builder.Build();

// Configuración del pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    
}
else
{
    app.UseExceptionHandler("/Main/Error");
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Productos v1");
    });


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");

app.UseRouting();

// Activa autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=Inicio}/{id?}");

    app.MapHub<ChatHub>("/chatHub");

app.MapRazorPages();

app.Run();
