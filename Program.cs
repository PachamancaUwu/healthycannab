using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using healthycannab.Data;
using healthycannab.Hubs;
using Microsoft.OpenApi.Models;
using healthycannab.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Configuracion de la cadena de conexion
var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//identity
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

//swagger serivicios:
//builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo {Title = "API de Productos", Version = "v1"});
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    
}
else
{
    app.UseExceptionHandler("/Main/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=Inicio}/{id?}");

    app.MapHub<ChatHub>("/chatHub");

app.MapRazorPages();

app.Run();
