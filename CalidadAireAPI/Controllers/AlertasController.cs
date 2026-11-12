using CalidadAireAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CalidadAireAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AlertasController : ControllerBase
    {
        private readonly CalidadAireDbContext _context;

        public AlertasController(CalidadAireDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerAlertas(
            CancellationToken cancellationToken)
        {
            var alertas = await _context.AlertasAire
                .AsNoTracking()
                .OrderByDescending(x => x.FechaHora)
                .Select(x => new
                {
                    x.Id,
                    x.SensorId,
                    x.Nivel,
                    x.Mensaje,
                    x.FechaHora
                })
                .ToListAsync(cancellationToken);

            return Ok(alertas);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(
            int id,
            CancellationToken cancellationToken)
        {
            var alerta = await _context.AlertasAire
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.SensorId,
                    x.Nivel,
                    x.Mensaje,
                    x.FechaHora
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (alerta == null)
                return NotFound("No existe una alerta con el identificador indicado.");

            return Ok(alerta);
        }
    }
}
