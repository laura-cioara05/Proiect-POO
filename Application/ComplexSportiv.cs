using PROIECT_POO.Domain.Common;
using PROIECT_POO.Domain.Utilizatori;
using PROIECT_POO.Domain.Rezervari;
using PROIECT_POO.Domain.Terenuri;
using PROIECT_POO.Infrastructure;

namespace PROIECT_POO.Application;

public class ComplexSportiv //facade/ punct de acces central UI
{
    private readonly IStocareDate _storage;
    private readonly GestionareTerenuri _terenuri;
    private readonly GestionareRezervari _rezervari;
    private readonly ReguliRezervare _reguliRezervare;
     private readonly Autentificare _autentificare;
    private readonly ILogger _logger;

    public TimeSpan DURATA_REZERVARE_STANDART;

    // ===============================
    // CONSTRUCTOR
    // ===============================

    public ComplexSportiv(IStocareDate storage,ILogger logger)
    {
        _storage = storage;
        _logger = logger;
        // 1. Încărcăm regulile (sau setăm default)
        var reguliSalvate = _storage.Incarca<ReguliRezervare>("reguliDeRezervari.json");
        _reguliRezervare = reguliSalvate.FirstOrDefault() ??
                           new ReguliRezervare(TimeSpan.FromHours(1), TimeSpan.FromHours(2), 3);
        DURATA_REZERVARE_STANDART = _reguliRezervare.DurataStandard;

        // 2. Încărcăm terenurile
        var dateTerenuri = _storage.Incarca<TerenDeSport>("terenuri.json");
        _terenuri = new GestionareTerenuri(logger,dateTerenuri);

        // 3. Încărcăm rezervările
        var dateRezervari = _storage.Incarca<Rezervare>("rezervari.json");
        _rezervari = new GestionareRezervari(_terenuri, _reguliRezervare,logger, dateRezervari);
        _autentificare = new Autentificare(storage,logger);
    }

    // ======================
    // AUTENTIFICARE UTILIZATOR
    // ======================
    
     public Utilizator AutentificareUtilizator(string username, string password) //returneaza utilizator logat
        => _autentificare.Login(username,password);

    // ======================
    // ADMIN - TERENURI
    // ======================
    
    
    public void AdaugaTeren(TerenDeSport teren)
    {
        _terenuri.AdaugaTeren(teren);
        _storage.Salveaza("terenuri.json", _terenuri.Terenuri);
    }

    public void StergeTeren(Guid terenId)
    {
        _terenuri.StergeTeren(terenId, _rezervari.Rezervari);
        _storage.Salveaza("terenuri.json", _terenuri.Terenuri);
    }

    public void StergeTerenuriDupaTip(TipTeren tip)
    {
        _terenuri.StergeTerenuriDupaTip(tip);
        _storage.Salveaza("terenuri.json", _terenuri.Terenuri);
    }

    public void ModificaProgramTeren(Guid terenId, TimeSpan oraDeschidere, TimeSpan oraInchidere)
    {
        _terenuri.ModificaProgramTeren(terenId, oraDeschidere, oraInchidere);
        _storage.Salveaza("terenuri.json", _terenuri.Terenuri);
    }

    public void AdaugaIntervalIndisponibil(Guid terenId, IntervalOrar interval)
    {
        _terenuri.AdaugaIntervalIndisponibilTeren(terenId, interval);
        _storage.Salveaza("terenuri.json", _terenuri.Terenuri);
    }

    public void StergeIntervalIndisponibil(Guid terenId, IntervalOrar interval)
    {
        _terenuri.StergeIntervalIndisponibilTeren(terenId, interval);
        _storage.Salveaza("terenuri.json", _terenuri.Terenuri);
    }

    // ======================
    // ADMIN - LISTA DE REZERVARI PT UN TEREN( ACTIVE/ISTORICE)
    // ======================
    
    
    public IReadOnlyList<Rezervare> GetRezervariActiveTeren(Guid terenId) 
        => _rezervari.GetRezervariActive(terenId);

    public IReadOnlyList<Rezervare> GetRezervariIstoriceTeren(Guid terenId) 
        => _rezervari.GetRezervariIstorice(terenId);
    
   
    
    // ======================
    // ADMIN - REGULI DE REZERVARE
    // ======================

 

    public void ModificaDurataStandardRezervare(TimeSpan durataNoua)
    {
        // Apelăm metoda din GESTIONARE (Manager)
        _rezervari.ModificaDurataStandard(durataNoua);
    
        // Salvăm starea actuală
        _storage.Salveaza("reguliDeRezervari.json", new List<ReguliRezervare> { _reguliRezervare });
    }

    public void ModificaAnulareMinimaRezervare(TimeSpan nou)
    {
        // Apelăm metoda din GESTIONARE (Manager)
        _rezervari.ModificaAnulareMinima(nou);
    
        _storage.Salveaza("reguliDeRezervari.json", new List<ReguliRezervare> { _reguliRezervare });
    }

    public void ModificaNumarMaximRezervariSimultane(int numarNou)
    {
        // Apelăm metoda din GESTIONARE (Manager)
        _rezervari.ModificaNumarMaximRezervariSimultane(numarNou);
    
        _storage.Salveaza("reguliDeRezervari.json", new List<ReguliRezervare> { _reguliRezervare });
    }


    // ======================
    // ADMIN-CLIENT- REZERVARI
    // ======================

    public Rezervare CreeazaRezervare(Guid clientId, Guid terenId, IntervalOrar interval)
    {
         var rezervareNoua = _rezervari.CreeazaRezervare(clientId, terenId, interval);

         _storage.Salveaza("rezervari.json", _rezervari.Rezervari);

        return rezervareNoua;
    }

    // ======================
    // ANULARE REZERVARE (ADMIN / CLIENT)
    // ======================

    
   
    public void AnuleazaRezervare(Guid rezervareId, Utilizator utilizatorLogat)
    {
        
        _rezervari.AnuleazaRezervare(rezervareId, utilizatorLogat);
        
        _storage.Salveaza("rezervari.json", _rezervari.Rezervari);
    }
    // ======================
    // GESTIONARE MODIFICĂRI (ADMIN / CLIENT)
    // ======================

    
    public void ModificaRezervare(Guid rezervareId, Utilizator utilizatorLogat, IntervalOrar intervalNou)
    {
        _rezervari.ModificaIntervalRezervare(rezervareId, utilizatorLogat, intervalNou);
    
        _storage.Salveaza("rezervari.json", _rezervari.Rezervari);
    }

    // ======================
    // CLIENT -1.DISPONIBILITATE TERENURI 
    // ======================
    
     
    public List<TerenDeSport> CautaTerenuriLibere(TipTeren tip, IntervalOrar interval)
    {
        // Pasăm managerului de terenuri: tipul, intervalul și LISTA de rezervări de care are nevoie
        return _terenuri.CautaTerenuriDisponibile(tip, interval, _rezervari.Rezervari);
    }
   
    
    // ======================
    // CLIENT -2.VIZUALIZARE DETALII TEREN
    // ======================
   
    public string GetInfoTeren(Guid terenId)
    {
        return _terenuri.GenereazaFisaDisponibilitate(terenId, _rezervari.Rezervari);
    }

     
    //OPTIONAL:INTERVALE DISPONIBILE PT UN TEREN
    
    public string GetIntervaleLibereText(Guid terenId)
    {
        var intervale = _terenuri.CalculeazaIntervaleLibere(terenId, _rezervari.Rezervari);
    
        if (intervale.Count == 0) return "Terenul nu are intervale libere azi.";

        string rezultat = "Intervale disponibile pentru rezervare:\n";
        foreach (var i in intervale)
        {
            rezultat += $"  [LIBER]: {i.Start:HH:mm} - {i.End:HH:mm}\n";
        }
        return rezultat;
    }

    
    // ======================
    // CLIENT -3.1 VIZUALIZARE REZERVARI ISTORICE-ACTIVE
    // ======================
    public IReadOnlyList<Rezervare> GetRezervariActiveClient(Guid clientId)
        => _rezervari.GetRezervariViitoareClient(clientId);

    public IReadOnlyList<Rezervare> GetIstoricRezervariClient(Guid clientId)
        => _rezervari.GetIstoricRezervariClient(clientId);

    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    // ======================
    // ACTUALIZAREA DATELOR DIN FISIERE
    // ======================
    private void SalveazaSpecific<T>(T date)
    {
        switch (date)
        {
            case IEnumerable<TerenDeSport> listaTerenuri:
                _storage.Salveaza("terenuri.json", listaTerenuri);
                break;
            
            case IEnumerable<Rezervare> listaRezervari:
                _storage.Salveaza("rezervari.json", listaRezervari);
                break;

            // Cazul nou pentru reguli
            case ReguliRezervare reguli:
                _storage.Salveaza("reguliDeRezervari.json", new List<ReguliRezervare> { reguli });
                break;
            
            case IEnumerable<Utilizator> listaUseri:
                _storage.Salveaza("utilizatori.json", listaUseri);
                break;
            
            default:
                throw new ArgumentException($"Tipul {typeof(T).Name} nu are un fișier de destinație definit.");
        }
    
        Console.WriteLine($"[Sistem] Datele de tip {typeof(T).Name} au fost sincronizate cu succes.");
    }
}
