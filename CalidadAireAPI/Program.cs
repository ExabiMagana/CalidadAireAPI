using CalidadAireAPI.Data;
using CalidadAireAPI.Models;
using CalidadAireAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var connectionString =
    builder.Configuration.GetConnectionString("CalidadAire")
    ?? throw new InvalidOperationException(
        "No se configuró la conexión 'CalidadAire'.");

builder.Services.AddDbContext<CalidadAireDbContext>(options =>
    options.UseMySQL(connectionString));

builder.Services.AddScoped<EvaluadorAlertas>();
builder.Services.AddHttpClient<ClimaService>();
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Falta Jwt:Key.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });


var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<CalidadAireDbContext>();

    if (!context.Usuarios.Any())
    {
        var hasher = scope.ServiceProvider
            .GetRequiredService<IPasswordHasher<Usuario>>();

        var usuario = new Usuario
        {
            UsuarioNombre = "admin"
        };

        usuario.ContrasenaHash = hasher.HashPassword(
            usuario,
            "Admin123*"
        );

        context.Usuarios.Add(usuario);
        context.SaveChanges();
    }
}


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "API Calidad Aire v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();