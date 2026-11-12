using System.ComponentModel.DataAnnotations;

namespace CalidadAireAPI.DTOs;

public class CrearLecturaDto
{
    [Range(1, int.MaxValue)]
    public int SensorId { get; set; }

    [Required]
    [Range(typeof(decimal), "0", "99999999.99")]
    public decimal? PM2_5 { get; set; }

    [Required]
    [Range(typeof(decimal), "0", "99999999.99")]
    public decimal? PM10 { get; set; }

    [Required]
    [Range(typeof(decimal), "0", "99999999.99")]
    public decimal? CO2 { get; set; }

}