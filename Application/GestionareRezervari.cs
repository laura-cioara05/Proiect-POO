using PROIECT_POO.Domain.Rezervari;
using PROIECT_POO.Domain.Terenuri;
using PROIECT_POO.Domain.Exceptii;
using PROIECT_POO.Domain.Common;
using PROIECT_POO.Infrastructure;
using PROIECT_POO.Domain.Utilizatori;

namespace PROIECT_POO.Application;

public class GestionareRezervari// serviciu/coordonator de rezervari(Application Layer)
{
    private readonly List<Rezervare> _rezervari = new();
    private readonly ReguliRezervare _reguliRezervare;
    private readonly GestionareTerenuri _terenManager ;
    private readonly ILogger _logger;
    public IReadOnlyList<Rezervare> Rezervari => _rezervari.AsReadOnly();

    public GestionareRezervari(GestionareTerenuri terenManager , ReguliRezervare reguliRezervare,ILogger logger,IEnumerable<Rezervare>? rezervariInitiale=null)
    {
        _terenManager = terenManager;
        _reguliRezervare = reguliRezervare;
        _logger = logger;
        // Dacă rezervariInitiale este null (ex: fișier lipsă), creăm o listă goală
        _rezervari = rezervariInitiale?.ToList() ?? new List<Rezervare>();
    }
   // ADMIN
    // ===============================
    //  3.CREARE/ANULARE/MODIFICARE REZERVARI
    // ===============================

    public Rezervare CreeazaRezervare(Guid clientId, Guid terenId, IntervalOrar interval)
    {
        //Se gaseste terenul dupa terenId
        
        var teren = _terenManager.GetTeren(terenId);
        
        //Se verifica existenta terenului
        if (teren == null)
        {
            _logger.LogError($"Terenul nu exista (TerenId={terenId})");
             throw new RezervareException("Terenul nu exista!");
        }
        
        VerificaReguliRezervare(clientId, teren, interval);
        
        //Se creeaza rezervarea dupa indeplinirea tuturor conditiilor de mai sus
        var rezervare = new Rezervare(Guid.NewGuid(), terenId, clientId, interval,RezervareStatus.Activa);
        _rezervari.Add(rezervare);
        _logger.LogInfo($"Rezervare creata cu succes (TerenId={terenId})");

        return rezervare;
    }
    
    public void AnuleazaRezervare(Guid rezervareId, Utilizator solicitant)
    {
        //Se cauta rezervarea
        var rezervare = _rezervari.FirstOrDefault(r=> r.Id == rezervareId);
        
        //Daca nu se gaseste in lista de rezervari, nu exista
       if (rezervare == null)
        {
            _logger.LogError($"Rezervarea nu exista (RezervareId={rezervareId})");
            throw new Exception("Rezervare nu exista!");
        }
        if (solicitant is Client client)
            {
                // 1. Verificăm dacă e rezervarea lui
                if (rezervare.ClientId != client.Id)
                {
                    _logger.LogError($"Anulare neautorizat pentru rezervarea (RezervareId={rezervareId})");
                    throw new RezervareException("Clientul nu are dreptul de a anula rezervarea(Nu pe numele acesta este facuta rezervarea)!");
                }
                // 2. Verificăm timpul limită
                if ((rezervare.Interval.Start - DateTime.Now) < _reguliRezervare.AnulareMinima)
                {
                    _logger.LogError($"Timpul pentru anulare a expirat pentru rezervarea (RezervareId={rezervareId})");
                    throw new RezervareException("Timpul pentru anulare a expirat!");
                }
            }

        // Dacă e Admin SAU dacă e Clientul și a trecut de verificări:
        rezervare.Anuleaza();
        _logger.LogInfo($"Rezervare {rezervareId} anulata cu succes");
    }
    
    public void ModificaIntervalRezervare(Guid rezervareId, Utilizator solicitant, IntervalOrar intervalNou)
    {
        //Se cauta rezervarea
        var rezervare = _rezervari.FirstOrDefault(r=> r.Id == rezervareId);
        
        //Daca nu se gaseste in lista de rezervari, nu exista
         if (rezervare == null)
        {
            _logger.LogError($"Rezervarea nu exista (RezervareId={rezervareId})");
            throw new Exception("Rezervare nu exista!");
        }
        if (solicitant is Client client)
        {
            // Un client poate modifica DOAR rezervarea proprie
            if (rezervare.ClientId != client.Id)
                {
                    _logger.LogError($"Anulare neautorizat pentru rezervarea (RezervareId={rezervareId})");
                    throw new RezervareException("Clientul nu are dreptul de a anula rezervarea(Nu pe numele acesta este facuta rezervarea)!");
                }
            // Un client trebuie să respecte limita de timp (ex: minim 2 ore înainte)
            if ((rezervare.Interval.Start - DateTime.Now) < _reguliRezervare.AnulareMinima)
                {
                    _logger.LogError($"Timpul pentru anulare a expirat pentru rezervarea (RezervareId={rezervareId})");
                    throw new RezervareException("Timpul pentru anulare a expirat!");
                }
            }
        // Daca este AdministratorComplexSportiv, sarim peste verificarile de mai sus

        var teren = _terenManager.GetTeren(rezervare.TerenId)
                    ?? throw new RezervareException("Terenul nu mai exista!");

        // Verificăm dacă noul interval este valid (nu se suprapune, e în programul terenului, etc.)
        // Pasăm rezervareId pentru a ignora propria rezervare la verificarea suprapunerilor
        VerificaReguliRezervare(rezervare.ClientId, teren, intervalNou, rezervareId);
    
        rezervare.ModificaInterval(intervalNou);
        _logger.LogInfo($"Rezervare modificata (RezervareId={rezervare.Id})");
    }
    
    private void VerificaReguliRezervare(Guid clientId, TerenDeSport teren, IntervalOrar interval,Guid? rezervareId = null)
    {
        // verifică dacă intervalul este disponibil
        if (!teren.Program.EsteDisponibil(interval))
        {
            _logger.LogError($"Inteval {interval} indisponibil");
            throw new RezervareException("Intervalul ales nu este disponibil!");
        }
        // verifică durata standard
        if (interval.Durata < _reguliRezervare.DurataStandard)
        {
            _logger.LogError($"Durata rezervarii prea scurta pentru client {clientId}.Interval={interval}");
            throw new RezervareException("Durata rezervarii nu respecta regula standard!");
        }
        // verifică numarul maxim de rezervări simultane per client
        int rezervariActiveClient = _rezervari.Count(r =>
            r.ClientId == clientId &&
            r.Status == RezervareStatus.Activa && 
            (rezervareId == null || r.Id != rezervareId.Value));

        if (rezervariActiveClient >= _reguliRezervare.NumarMaximRezervariSimultane)
        {
            _logger.LogError($"Numar maxim de rezervari atins de catre client {clientId}");
            throw new RezervareException("Ai atins numarul maxim de rezervari active!");
        }
        
        //  Suprapuneri cu alte rezervări
        bool suprapunere = _rezervari.Any(r =>
            r.TerenId == teren.Id &&
            r.Status == RezervareStatus.Activa &&
            (rezervareId == null || r.Id != rezervareId.Value) &&
            r.Interval.SeSuprapuneCu(interval));

        if (suprapunere)
        {
            _logger.LogError($"Intervalul se suprapune cu o alta rezervrea.TerenId={teren.Id}, Interval={interval}");
            throw new RezervareException("Intervalul se suprapune cu o altă rezervare activă!");
        }

    }
    
    
    // ===============================
    // ADMIN - 2.MODIFICARE REGULI DE REZERVARE
    // ===============================

    public void ModificaDurataStandard(TimeSpan durataNoua)
        =>_reguliRezervare.ModificaDurataStandard(durataNoua);
    public void ModificaAnulareMinima(TimeSpan timp) 
        => _reguliRezervare.ModificaAnulareMinima(timp);

    public void ModificaNumarMaximRezervariSimultane(int numar) 
        => _reguliRezervare.ModificaNumarMaximRezervari(numar);
    
    
    //ADMIN
    // ===============================
    //  3.VIZUALIZARE REZERVARI ACTIVE SAU ISTORICE ALE UNUI TEREN
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
  
    
    //CLIENT
    // ===============================
    //  3.GESTIONAREA REZERVARILOR PERSONALE ALE UNUI CLIENT
    // ===============================

    
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