
using CalidadAireAPI.DTOs;
using CalidadAireAPI.Services;
using CalidadAireAPI.Data;
using CalidadAireAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CalidadAireAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LecturasController : ControllerBase
    {
        private readonly CalidadAireDbContext _context;
        private readonly EvaluadorAlertas _evaluador;
        private readonly ClimaService _climaService;

        public LecturasController(
            CalidadAireDbContext context,
            EvaluadorAlertas evaluador,
            ClimaService climaService)
        {
            _context = context;
            _evaluador = evaluador;
            _climaService = climaService;
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarLectura(
            CrearLecturaDto dto,
            CancellationToken cancellationToken)
        {
            var sensorExiste = await _context.Sensores
                .AnyAsync(x => x.Id == dto.SensorId, cancellationToken);

            if (!sensorExiste)
                return NotFound("No existe un sensor con el identificador indicado.");

            var lectura = new LecturaAire
            {
                SensorId = dto.SensorId,
                PM2_5 = dto.PM2_5!.Value,
                PM10 = dto.PM10!.Value,
                CO2 = dto.CO2!.Value,
                FechaHora = DateTime.UtcNow
            };

            var nivel = _evaluador.Evaluar(
                lectura.PM2_5, lectura.PM10, lectura.CO2);

            _context.LecturasAire.Add(lectura);

            if (nivel != null)
            {
                _context.AlertasAire.Add(new AlertaAire
                {
                    SensorId = lectura.SensorId,
                    Nivel = nivel,
                    Mensaje = ObtenerMensaje(nivel),
                    FechaHora = lectura.FechaHora
                });
            }

            await _context.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = lectura.Id },
                new
                {
                    lectura.Id,
                    lectura.SensorId,
                    lectura.PM2_5,
                    lectura.PM10,
                    lectura.CO2,
                    lectura.FechaHora,
                    NivelAlerta = nivel
                });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(
            int id,
            CancellationToken cancellationToken)
        {
            var lectura = await _context.LecturasAire
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.SensorId,
                    x.PM2_5,
                    x.PM10,
                    x.CO2,
                    x.FechaHora
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (lectura == null)
                return NotFound("No existe una lectura con el identificador indicado.");

            return Ok(lectura);
        }

        private static string ObtenerMensaje(string nivel)
        {
            return nivel switch
            {
                "Leve" => "La calidad del aire es moderada, se recomienda reducir actividades al aire libre prolongadas.",

                "Moderada" => "La calidad del aire es poco saludable para grupos sensibles (niños, adultos mayores, personas con enfermedades respiratorias).",

                "Critica" => "La calidad del aire es peligrosa. Se recomienda permanecer en interiores y usar mascarilla.",

                "Extrema" => "Nivel de contaminación extremadamente alto. Riesgo severo para la salud.",

                _ => throw new InvalidOperationException("Nivel de alerta no reconocido.")
            };
        }

        [HttpGet("filtrar")]
        public async Task<IActionResult> FiltrarLecturas(
            DateTime? fechaInicio,
            DateTime? fechaFin,
            string? contaminante,
            CancellationToken cancellationToken)
        {
            if (fechaInicio.HasValue && fechaFin.HasValue &&
                fechaInicio > fechaFin)
            {
                return BadRequest("La fecha inicial no puede ser mayor que la fecha final.");
            }

            var consulta = _context.LecturasAire
                .AsNoTracking()
                .AsQueryable();

            if (fechaInicio.HasValue)
                consulta = consulta.Where(x => x.FechaHora >= fechaInicio.Value);

            if (fechaFin.HasValue)
            {
                var limite = fechaFin.Value.Date.AddDays(1);
                consulta = consulta.Where(x => x.FechaHora < limite);
            }

            var lecturas = await consulta
                .OrderByDescending(x => x.FechaHora)
                .ToListAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(contaminante))
                return Ok(lecturas);

            var resultado = contaminante.Trim().ToUpperInvariant() switch
            {
                "PM2.5" => lecturas.Select(x => new
                {
                    x.Id,
                    x.SensorId,
                    Contaminante = "PM2.5",
                    Valor = x.PM2_5,
                    x.FechaHora
                }),

                "PM10" => lecturas.Select(x => new
                {
                    x.Id,
                    x.SensorId,
                    Contaminante = "PM10",
                    Valor = x.PM10,
                    x.FechaHora
                }),

                "CO2" => lecturas.Select(x => new
                {
                    x.Id,
                    x.SensorId,
                    Contaminante = "CO2",
                    Valor = x.CO2,
                    x.FechaHora
                }),

                _ => null
            };

            if (resultado == null)
                return BadRequest("El contaminante debe ser PM2.5, PM10 o CO2.");

            return Ok(resultado);
        }

        [HttpGet("{id:int}/enriquecida")]
        public async Task<IActionResult> ObtenerLecturaEnriquecida(
            int id,
            CancellationToken cancellationToken)
        {
            var lectura = await _context.LecturasAire
                .Include(x => x.Sensor)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (lectura == null)
                return NotFound("No existe una lectura con el identificador indicado.");

            try
            {
                var clima = await _climaService.ObtenerClimaAsync(
                    lectura.Sensor.Ubicacion,
                    cancellationToken);

                if (clima == null)
                    return StatusCode(502, "No se pudo obtener información meteorológica para la ubicación del sensor.");

                var resultado = new LecturaEnriquecidaDto
                {
                    Id = lectura.Id,
                    SensorId = lectura.SensorId,
                    PM2_5 = lectura.PM2_5,
                    PM10 = lectura.PM10,
                    CO2 = lectura.CO2,
                    FechaHora = lectura.FechaHora,
                    TemperaturaC = clima.Value.TemperaturaC,
                    HumedadPorcentaje = clima.Value.HumedadPorcentaje,
                    FechaHoraClima = clima.Value.FechaHoraClima
                };

                return Ok(resultado);
            }
            catch (HttpRequestException)
            {
                return StatusCode(502, "No fue posible conectar con el servicio meteorológico.");
            }
        }


    }
}
