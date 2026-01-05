using System.Text.Json.Serialization;
using PROIECT_POO.Domain.Common;
namespace PROIECT_POO.Domain.Terenuri;

public class TerenDeSport//IMUTABIL
{
    public Guid Id { get; }
    public TipTeren Tip { get; } // fotbal, tenis etc.
    public string Locatie { get; }
    public OrarFunctionare Program { get; private set; }

    [JsonConstructor]
    public TerenDeSport(Guid id, TipTeren tip, string locatie, OrarFunctionare program)
    {
        Id = id;
        Tip = tip;
        Locatie = locatie;
        Program = program;
    }
    
    public void ModificaProgramFunctionare(TimeSpan oraDeschidereNoua, TimeSpan oraInchidereNoua)
    {
        Program = Program.ModificaProgram(oraDeschidereNoua, oraInchidereNoua);
    }

    public void AdaugaIntervalIndisponibil(IntervalOrar interval)
    {
        Program = Program.AdaugaIntervalIndisponibil(interval);
    }

    public void StergeIntervalIndisponibil(IntervalOrar interval)
    {
        Program = Program.StergeIntervalIndisponibil(interval);
    }


}