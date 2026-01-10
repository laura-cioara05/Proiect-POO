using PROIECT_POO.Domain.Common;
using PROIECT_POO.Domain.Rezervari;
using PROIECT_POO.Domain.Terenuri;
using PROIECT_POO.Domain.Utilizatori;
using PROIECT_POO.Infrastructure;

namespace PROIECT_POO.Application;

public class ComplexSportiv//facade/ punct de acces central UI
{
    private readonly IStocareDate _storage;
    private readonly GestionareTerenuri _terenuri;
    private readonly GestionareRezervari _rezervari;
    private readonly ReguliRezervare _reguliRezervare;
    private readonly Autentificare _autentificare;
    private readonly ILogger _logger;
    // ===============================
    // CONSTRUCTOR
    // ===============================
    
    public ComplexSportiv(IStocareDate storage,ILogger logger)
    {
        _storage = storage;
        //1. Încărcăm regulile (sau setăm default)
        var reguliSalvate = _storage.Incarca<ReguliRezervare>("reguli.json");
        _reguliRezervare = reguliSalvate.FirstOrDefault() ?? new ReguliRezervare(TimeSpan.FromHours(1), TimeSpan.FromHours(2), 3);
        
        //2. Încărcăm terenurile
        var dateTerenuri = _storage.Incarca<TerenDeSport>("terenuri.json");
        _terenuri = new GestionareTerenuri(logger,dateTerenuri);
        
        //3. Încărcăm rezervările
        var dateRezervari = _storage.Incarca<Rezervare>("rezervari.json");
        _rezervari = new GestionareRezervari(_terenuri, _reguliRezervare,logger,dateRezervari);
        
        _autentificare = new Autentificare(storage,logger);
    }

    public Utilizator AutentificareUtilizator(string username, string password) //returneaza utilizator logat
        => _autentificare.Login(username,password);
    
    // ======================
    // ADMIN - TERENURI
    // ======================

    public void AdaugaTeren(TerenDeSport teren)
        => _terenuri.AdaugaTeren(teren);

    public void StergeTeren(Guid terenId)
        => _terenuri.StergeTeren(terenId, _rezervari.Rezervari);

    public void StergeTerenuriDupaTip(TipTeren tip)
        => _terenuri.StergeTerenuriDupaTip(tip);

    public void ModificaProgramTeren(Guid terenId, TimeSpan oraDeschidere, TimeSpan oraInchidere)
        => _terenuri.ModificaProgramTeren(terenId, oraDeschidere, oraInchidere);

    public void AdaugaIntervalIndisponibil(Guid terenId, IntervalOrar interval)
        => _terenuri.AdaugaIntervalIndisponibilTeren(terenId, interval);

    public void StergeIntervalIndisponibil(Guid terenId, IntervalOrar interval)
        => _terenuri.StergeIntervalIndisponibilTeren(terenId, interval);

   
    // ======================
    // ADMIN - REGULI DE REZERVARE
    // ======================

    public void ModificaDurataStandardRezervare(TimeSpan durataNoua)
        => _reguliRezervare.ModificaDurataStandard(durataNoua);

    public void ModificaAnulareMinimaRezervare(TimeSpan nou)
        => _reguliRezervare.ModificaAnulareMinima(nou);

    public void ModificaAnulareMinimaRezervare(int numarNou)
        => _reguliRezervare.ModificaNumarMaximRezervari(numarNou);
    
    
    
    // ======================
    // ADMIN - REZERVARI
    // ======================

    public Rezervare CreeazaRezervare(Guid clientId, Guid terenId, IntervalOrar interval)
        => _rezervari.CreeazaRezervare(clientId, terenId, interval);

    public void AnuleazaRezervare(Guid rezervareId, Guid clientId)
        => _rezervari.AnuleazaRezervare(rezervareId, clientId);
    
    // ======================
    // MENIU - UTILIZATOR
    // ======================
    
    
}
