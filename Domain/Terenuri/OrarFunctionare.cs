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
        TimeSpan oraDeschidere,
        TimeSpan oraInchidere,
        IEnumerable<IntervalOrar>? intervaleIndisponibile = null)
    {
        if (oraDeschidere >= oraInchidere)
            throw new ArgumentException("Ora de deschidere trebuie să fie înainte de ora de închidere.");

        OraDeschidere = oraDeschidere;
        OraInchidere = oraInchidere;
        IntervaleIndisponibile = intervaleIndisponibile?
                                     .ToList()
                                     .AsReadOnly()
                                 ?? new List<IntervalOrar>().AsReadOnly();
    }
    
    //Se verifica daca programul ales se suprapune cu vreunul din cele indisponibile
    public bool EsteDisponibil(IntervalOrar interval)
    {
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