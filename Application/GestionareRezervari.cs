using PROIECT_POO.Domain.Rezervari;
using PROIECT_POO.Domain.Terenuri;
using PROIECT_POO.Domain.Exceptii;
using PROIECT_POO.Domain.Common;

namespace PROIECT_POO.Application;

public class GestionareRezervari// serviciu/coordonator de rezervari(Application Layer)
{
    private readonly List<Rezervare> _rezervari = new();
    private readonly ReguliRezervare _reguliRezervare;
    private readonly GestionareTerenuri _terenManager ;

    public IReadOnlyList<Rezervare> Rezervari => _rezervari.AsReadOnly();

    public GestionareRezervari(GestionareTerenuri terenManager , ReguliRezervare reguliRezervare,IEnumerable<Rezervare>? rezervariInitiale=null)
    {
        _terenManager = terenManager;
        _reguliRezervare = reguliRezervare;
        // Dacă rezervariInitiale este null (ex: fișier lipsă), creăm o listă goală
        _rezervari = rezervariInitiale?.ToList() ?? new List<Rezervare>();
    }
    
    // ===============================
    //  3.CREARE/ANULARE/MODIFICARE REZERVARI
    // ===============================

    public Rezervare CreeazaRezervare(Guid clientId, Guid terenId, IntervalOrar interval)
    {
        //Se gaseste terenul dupa terenId
        
        var teren = _terenManager.GetTeren(terenId);
        
        //Se verifica existenta terenului
        if (teren == null)
            throw new RezervareException("Terenul nu exista!");
        
        VerificaReguliRezervare(clientId, teren, interval);
        
        //Se creeaza rezervarea dupa indeplinirea tuturor conditiilor de mai sus
        var rezervare = new Rezervare(Guid.NewGuid(), terenId, clientId, interval,RezervareStatus.Activa);
        _rezervari.Add(rezervare);
        
        //Se adauga intervalul rezervarii in lista de intervale indisponibile
        
        teren.AdaugaIntervalIndisponibil(interval);
        
        
        return rezervare;
    }
    
    public void AnuleazaRezervare(Guid rezervareId, Guid clientId)
    {
        //Se cauta rezervarea
        var rezervare = _rezervari.FirstOrDefault(r=> r.Id == rezervareId);
        
        //Daca nu se gaseste in lista de rezervari, nu exista
        if(rezervare==null)
            throw new Exception("Rezervare nu exista!");
        
        //Persoana care doreste sa anuleze rezervarea nu este aceeasi care a si facut rezervarea
        if(rezervare.ClientId != clientId)
            throw new RezervareException("Clientul nu are dreptul de a anula rezervarea(Nu pe numele acesta este facuta rezervarea)!");

        //Timpul alocat anularii unei rezervari a expirat
        if ((rezervare.Interval.Start - DateTime.Now) < _reguliRezervare.AnulareMinima)
            throw new RezervareException("Nu se mai poate anula rezervarea,timpul acordat anularii a expirat!");
        rezervare.Anuleaza(); 
    }
    
    public void ModificaRezervare(Guid rezervareId, IntervalOrar intervalNou)
    {
        var rezervare = _rezervari.FirstOrDefault(r => r.Id == rezervareId)
                        ?? throw new RezervareException("Rezervarea nu exista!");

        var teren = _terenManager.GetTeren(rezervare.TerenId)
                    ?? throw new RezervareException("Terenul rezervării nu exista!");
        VerificaReguliRezervare(rezervare.ClientId, teren, intervalNou);
       
       
        // modifică intervalul
        rezervare.ModificaInterval(intervalNou);

        // actualizează intervalele indisponibile ale terenului
        teren.AdaugaIntervalIndisponibil(intervalNou);
    }
    
    private void VerificaReguliRezervare(Guid clientId, TerenDeSport teren, IntervalOrar interval,Guid? rezervareId = null)
    {
        // verifică dacă intervalul este disponibil
        if (!teren.Program.EsteDisponibil(interval))
            throw new RezervareException("Intervalul ales nu este disponibil!");

        // verifică durata standard
        if (interval.Durata < _reguliRezervare.DurataStandard)
            throw new RezervareException("Durata rezervarii nu respecta regula standard!");

        // verifică numarul maxim de rezervări simultane per client
        int rezervariActiveClient = _rezervari.Count(r =>
            r.ClientId == clientId &&
            r.Status == RezervareStatus.Activa);

        if (rezervariActiveClient > _reguliRezervare.NumarMaximRezervariSimultane)
            throw new RezervareException("Ai atins numarul maxim de rezervari active!");
        
        //  Suprapuneri cu alte rezervări
        bool suprapunere = _rezervari.Any(r =>
            r.TerenId == teren.Id &&
            r.Status == RezervareStatus.Activa &&
            (rezervareId == null || r.Id != rezervareId.Value) &&
            r.Interval.SeSuprapuneCu(interval));

        if (suprapunere)
            throw new RezervareException("Intervalul se suprapune cu o altă rezervare activă!");

    }
    
    // ===============================
    //  3.VIZUALIZARE REZERVARI ACTIVE SAU ISTORICE
    // ===============================

    public IReadOnlyList<Rezervare> GetRezervariActive(Guid terenId)
    {
        return _rezervari
            .Where(r => r.TerenId == terenId && r.Status == RezervareStatus.Activa)
            .ToList()
            .AsReadOnly();
    }

    public IReadOnlyList<Rezervare> GetRezervariIstorice(Guid terenId)
    {
        return _rezervari
            .Where(r => r.TerenId == terenId && r.Status != RezervareStatus.Activa)
            .ToList()
            .AsReadOnly();
    }
    
    
    // ===============================
    //  3.GESTIONAREA REZERVARILOR PERSONALE
    // ===============================

    
    public IReadOnlyList<Rezervare> GetRezervariClient(Guid clientId)
    {
        return _rezervari
            .Where(r => r.ClientId == clientId)
            .ToList()
            .AsReadOnly();
    }
    
    //SAU 2 METODE(ISTORICE--VIITOARE)
    public IReadOnlyList<Rezervare> GetRezervariViitoareClient(Guid clientId)
    {
        return _rezervari
            .Where(r => r.ClientId == clientId && r.Interval.Start > DateTime.Now && r.Status == RezervareStatus.Activa)
            .ToList();
    }

    public IReadOnlyList<Rezervare> GetIstoricRezervariClient(Guid clientId)
    {
        return _rezervari
            .Where(r => r.ClientId == clientId && (r.Interval.End < DateTime.Now || r.Status != RezervareStatus.Activa))
            .ToList();
    }
}