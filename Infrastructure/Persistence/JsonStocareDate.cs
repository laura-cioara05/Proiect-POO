using System.Text.Json;
using PROIECT_POO.Infrastructure;

namespace PROIECT_POO.Application.Interfaces;

public class JsonStocareDate : IStocareDate
{
    private readonly JsonSerializerOptions _optiuni = new() { WriteIndented = true };

    public void Salveaza<T>(string caleFisier, IEnumerable<T> date)
    {
        string json = JsonSerializer.Serialize(date, _optiuni);
        File.WriteAllText(creareCaleFisier(caleFisier), json);
    }
 

    public List<T> Incarca<T>(string caleFisier)
    {
        if (!File.Exists(creareCaleFisier(caleFisier))) return new List<T>();

        try
        {
            string continut = File.ReadAllText(creareCaleFisier(caleFisier));
            return JsonSerializer.Deserialize<List<T>>(continut, _optiuni) ?? new List<T>();
        }
        catch (JsonException)
        {
            Console.WriteLine($"Eroare la citirea fisierului {creareCaleFisier(caleFisier)}: Format invalid.");
            return new List<T>();
        }
    }

    private string creareCaleFisier(string denumireFile)
    {
        return Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..",
            "Infrastructure",
            "JsonFiles",
            $"{denumireFile}"
        );
    }
}

