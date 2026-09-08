using Microsoft.EntityFrameworkCore;
using MiApi.Api.Extensions;
using MiApi.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Servicios (Inyección de Dependencias)
builder.Services
    .AddDatabase(builder.Configuration)
    .AddApplicationServices()
    .AddJwtAuthentication(builder.Configuration)
    .AddSwaggerWithJwt();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Captura global de excepciones (debe ir antes de Routing, Auth y Controllers)
app.UseMiddleware<MiApi.Api.Middlewares.ExceptionHandlingMiddleware>();

using var scope = app.Services.CreateScope();
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
  
}
    

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Pipeline de Middleware de Seguridad
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();