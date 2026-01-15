using System.Text.Json;
using System.Text.Json.Serialization;
using PROIECT_POO.Infrastructure;

namespace PROIECT_POO.Application.Interfaces;

public class JsonStocareDate : IStocareDate
{
    private readonly JsonSerializerOptions _optiuni = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public void Salveaza<T>(string caleFisier, IEnumerable<T> date)
    {
        try
        { 
            // Ne asiguram ca folderul exista
            string? director = Path.GetDirectoryName(creareCaleFisier(caleFisier));
            if (!string.IsNullOrEmpty(director) && !Directory.Exists(director))
            {
                Directory.CreateDirectory(director);
            }

            string json = JsonSerializer.Serialize(date, _optiuni);
            File.WriteAllText(creareCaleFisier(caleFisier), json);
        }
        catch (JsonException)
        {
            Console.WriteLine($"Eroare la salvare in {creareCaleFisier(caleFisier)}.");
        }
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

