// JaveragesLibrary/Program.cs

using MiMangaBot.Services.Features.Mangas; // ¡No olvides el using!

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// AQUÍ REGISTRAMOS NUESTROS SERVICIOS
builder.Services.AddScoped<MangaService>();
builder.Services.AddScoped<MiMangaBot.Services.Features.Prestamos.PrestamoService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => // Configuración opcional para Swagger
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "MangaBot API",
        Description = "Una API para gestionar una increíble colección de mangas",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Tu Nombre/Equipo",
            Url = new Uri("https://tuwebsite.com") // Cambia esto
        }
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => // Para que Swagger apunte a nuestra V1 por defecto
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MangaBot API V1");
        options.RoutePrefix = string.Empty; // Para que Swagger UI esté en la raíz (http://localhost:XXXX/)
    });
}

app.UseHttpsRedirection();
app.UseAuthorization(); // Lo veremos más adelante
app.MapControllers();   // Para que las peticiones lleguen a nuestros Controllers

app.Run();