namespace CalidadAireAPI.Models
{
    public class LecturaAire
    {
        public int Id { get; set; }
        public int SensorId { get; set; }
        public decimal PM2_5 { get; set; } = 0;
        public decimal PM10 { get; set; } = 0;
        public decimal CO2 { get; set; } = 0;
        public DateTime FechaHora { get; set; } = DateTime.Now;
        public SensorCalidadAire Sensor { get; set; } = null!;

    }
}
