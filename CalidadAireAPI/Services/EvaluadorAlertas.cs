namespace CalidadAireAPI.Services;

public class EvaluadorAlertas
{
    public string? Evaluar(decimal pm25, decimal pm10, decimal co2)
    {
        if (co2 > 5000 || pm25 > 250)
            return "Extrema";

        if (pm25 > 150 || pm10 > 200)
            return "Critica";

        if (pm25 >= 51 && pm25 <= 100 || co2 > 1000)
            return "Moderada";

        if (pm25 >= 25 && pm25 <= 50)
            return "Leve";

        return null;
    }
}