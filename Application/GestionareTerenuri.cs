using PROIECT_POO.Domain.Terenuri;
using PROIECT_POO.Domain.Rezervari;
using PROIECT_POO.Domain.Common;
using PROIECT_POO.Infrastructure;


namespace PROIECT_POO.Application;

public class GestionareTerenuri// serviciu/coordonator de terenuri(Application Layer)
{
    private readonly List<TerenDeSport> _terenuri ;
    private readonly ILogger _logger ;

    public IReadOnlyList<TerenDeSport> Terenuri => _terenuri.AsReadOnly();
    public GestionareTerenuri(ILogger logger, IEnumerable<TerenDeSport>? terenuriInitiale = null)
    {
        // Dacă terenuriInitiale este null (ex: fișier lipsă), creăm o listă goală
        _terenuri = terenuriInitiale?.ToList() ?? new List<TerenDeSport>();
       _logger = logger;
    }

    // ===============================
    // 1.1 ADMINISTRAREA TERENURILOR
    // ===============================

    public TerenDeSport? GetTeren(Guid terenId)
    { 
        
        return _terenuri.FirstOrDefault(t => t.Id == terenId); // _terenuri e lista internă din GestionareTerenuri
    }

    
    public void AdaugaTeren(TerenDeSport teren)
    {
        _terenuri.Add(teren);
        //Adauga teren in JSON File
    }
    
    public void StergeTeren(Guid terenId, IEnumerable<Rezervare> rezervari)
    {
        var terenGasit = _terenuri.FirstOrDefault(t => t.Id == terenId);
        if (terenGasit == null)
        {
            _logger.LogError($"Terenul (TerenId: {terenId}) nu exista");
            throw new Exception("Terenul nu exista!");
        }
         
        //Se verifica daca exista rezervari active pentru terenul ales
        bool existaRezervariActive=rezervari.Any(r=>r.TerenId==terenId && r.Status==RezervareStatus.Activa);

        if (existaRezervariActive)
        {
            _logger.LogError($"Terenul (TerenId: {terenId}) nu poate fi sters");
            throw new Exception("Terenul nu poate fi sters daca are rezervari active!");
        }
        _terenuri.Remove(terenGasit);
        _logger.LogInfo($"Terenul (TerenId: {terenId}) a fost sters");
    }
    
    public void StergeTerenuriDupaTip(TipTeren tip)
    {
        //Se verifica daca exista terenuri de acest tip 
        bool exista=_terenuri.Any(t => t.Tip == tip);

        if (!exista)
        {
            _logger.LogError($"Terenul (tip: {tip}) nu exista");
            throw new InvalidOperationException($"Nu exista terenuri de tipul {tip}.");
        }
        _terenuri.RemoveAll(t => t.Tip == tip);
        _logger.LogInfo($"Terenul (tip: {tip}) a fost sters");
    }
    
    // ===============================
    //  1.2 MODIFICARE : PROGRAM TEREN/ INTERVAL INDISPONIBIL 
    // ===============================
    
    public void ModificaProgramTeren(Guid terenId, TimeSpan oraDeschidereNoua, TimeSpan oraInchidereNoua)
    {
        var teren = _terenuri.FirstOrDefault(t => t.Id == terenId);
        if (teren == null)
        {
            _logger.LogError($"Terenul (TerenId: {terenId}) nu exista");
            throw new Exception("Terenul nu exista.");
        }

        teren.ModificaProgramFunctionare(oraDeschidereNoua, oraInchidereNoua);
        _logger.LogInfo($"Programul de functionare a terenului (TerenId={terenId}) a fost schimbat");
    }

    public void AdaugaIntervalIndisponibilTeren(Guid terenId, IntervalOrar interval)
    {
        var teren = _terenuri.FirstOrDefault(t => t.Id == terenId);
        if (teren == null)
        {
            _logger.LogError($"Terenul (TerenId: {terenId}) nu exista");
            throw new Exception("Terenul nu exista.");
        }

        teren.AdaugaIntervalIndisponibil(interval);
        _logger.LogInfo($"A fost adaugat interval indisponibil de functionare a terenului (TerenId={terenId})");
    }

    public void StergeIntervalIndisponibilTeren(Guid terenId, IntervalOrar interval)
    {
        var teren = _terenuri.FirstOrDefault(t => t.Id == terenId);
        if (teren == null)
        {
            _logger.LogError($"Terenul (TerenId: {terenId}) nu exista");
            throw new Exception("Terenul nu exista.");
        }

        teren.StergeIntervalIndisponibil(interval);
        _logger.LogInfo($"A fost sters inteval indisponibil a  terenului (TerenId={terenId}) ");
    }
}
