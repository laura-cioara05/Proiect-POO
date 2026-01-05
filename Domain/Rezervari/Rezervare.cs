using System.Text.Json.Serialization;
using PROIECT_POO.Domain.Common;

namespace PROIECT_POO.Domain.Rezervari;

public class Rezervare
{
    public Guid Id { get; }
    public Guid TerenId { get; }
    public Guid ClientId { get; }
    public IntervalOrar Interval { get; private set; }   
    public RezervareStatus Status { get; private set; }

    [JsonConstructor]
    public Rezervare(Guid id, Guid terenId, Guid clientId,IntervalOrar interval, RezervareStatus status)
    {
        Id = id;
        TerenId = terenId;
        ClientId = clientId;
        Interval=interval;
        Status = status;
    }
    
    public void Anuleaza()
    {
        if (Status != RezervareStatus.Activa)
            throw new InvalidOperationException("Rezervarea nu poate fi anulata pentru ca nu e activa!");

        Status = RezervareStatus.Anulata;
    }
    public void ModificaInterval(IntervalOrar intervalNou)
    {
        if (Status != RezervareStatus.Activa)
            throw new InvalidOperationException("Nu poți modifica o rezervare inactivă.");
        Interval = intervalNou;
    }
}