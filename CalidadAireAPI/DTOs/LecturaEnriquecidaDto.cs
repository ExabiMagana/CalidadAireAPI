namespace CalidadAireAPI.DTOs
{
    public class LecturaEnriquecidaDto
    {
        public int Id { get; set; }

        public int SensorId { get; set; }

        public decimal PM2_5 { get; set; }

        public decimal PM10 { get; set; }

        public decimal CO2 { get; set; }

        public DateTime FechaHora { get; set; }

        public double TemperaturaC { get; set; }

        public double HumedadPorcentaje { get; set; }

        public DateTime FechaHoraClima { get; set; }
    }
}