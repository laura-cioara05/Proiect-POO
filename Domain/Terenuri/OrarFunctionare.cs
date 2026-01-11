using System.Text.Json.Serialization;
using PROIECT_POO.Domain.Common;

namespace PROIECT_POO.
    Domain.Terenuri;

public class OrarFunctionare
{
    public TimeSpan OraDeschidere { get; }
    public TimeSpan OraInchidere { get; }
    public IReadOnlyList<IntervalOrar> IntervaleIndisponibile { get; }
    
    [JsonConstructor]
    public OrarFunctionare(
        TimeSpan OraDeschidere,
        TimeSpan OraInchidere,
        IReadOnlyList<IntervalOrar>? IntervaleIndisponibile = null)
    {
        if (OraDeschidere >= OraInchidere)
            throw new ArgumentException("Ora de deschidere trebuie să fie înainte de ora de închidere.");

        this.OraDeschidere = OraDeschidere;
        this.OraInchidere = OraInchidere;
        this.IntervaleIndisponibile =
            IntervaleIndisponibile?.ToList().AsReadOnly()
            ?? new List<IntervalOrar>().AsReadOnly();
    }
    
    //Se verifica daca programul ales se suprapune cu vreunul din cele indisponibile
    public bool EsteDisponibil(IntervalOrar interval)
    {
        // 1. Verificăm dacă ora de start și cea de end sunt în limitele programului (ex: 08:00 - 22:00)
        bool inProgram = interval.Start.TimeOfDay >= OraDeschidere && 
                         interval.End.TimeOfDay <= OraInchidere;
    
        if (!inProgram) return false;
        // 2. Verificăm dacă nu se bate cu mentenanța (ce aveai deja)
        return !IntervaleIndisponibile
            .Any(i => i.SeSuprapuneCu(interval));
    }
    
    // 🔧 Aministratorul modifică programul de funcționare
    public OrarFunctionare ModificaProgram(
        TimeSpan oraDeschidereNoua,
        TimeSpan oraInchidereNoua)
    {
        return new OrarFunctionare(
            oraDeschidereNoua,
            oraInchidereNoua,
            IntervaleIndisponibile);
    }
    
    //  Administratorul adaugă un interval indisponibil
    public OrarFunctionare AdaugaIntervalIndisponibil(IntervalOrar interval)
    {
        if (interval == null)
            throw new ArgumentNullException(nameof(interval));

        var intervaleNoi = IntervaleIndisponibile.ToList();
        intervaleNoi.Add(interval);

        return new OrarFunctionare(OraDeschidere, OraInchidere, intervaleNoi);
    }
    
    //  Administratorul șterge un interval indisponibil
    public OrarFunctionare StergeIntervalIndisponibil(IntervalOrar interval)
    {
        if (interval == null)
            throw new ArgumentNullException(nameof(interval));

        var intervaleNoi = IntervaleIndisponibile
            .Where(i => !i.Equals(interval))
            .ToList();

        return new OrarFunctionare(OraDeschidere, OraInchidere, intervaleNoi);
    }
    
}