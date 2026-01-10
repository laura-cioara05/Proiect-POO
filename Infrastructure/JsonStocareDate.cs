using System.Text.Json;

namespace PROIECT_POO.Application.Interfaces;

public class JsonStocareDate
{
private readonly JsonSerializerOptions _optiuni = new() { WriteIndented = true };

public void Salveaza<T>(string caleFisier, IEnumerable<T> date)
{
    string json = JsonSerializer.Serialize(date, _optiuni);
    File.WriteAllText(caleFisier, json);
}

public List<T> Incarca<T>(string caleFisier)
{
    if (!File.Exists(caleFisier)) return new List<T>();

    try
    {
        string continut = File.ReadAllText(caleFisier);
        return JsonSerializer.Deserialize<List<T>>(continut, _optiuni) ?? new List<T>();
    }
    catch (JsonException)
    {
        Console.WriteLine($"Eroare la citirea fisierului {caleFisier}: Format invalid.");
        return new List<T>();
    }
}
}