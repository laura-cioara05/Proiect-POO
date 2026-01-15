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

    //   ADMIN
    // ===============================
    // 1.1 ADMINISTRAREA TERENURILOR
    // ===============================

    
    public void AdaugaTeren(TerenDeSport teren)
    {
        if (!Enum.IsDefined(typeof(TipTeren), teren.Tip))
        {
            _logger.LogError($"Teren invalid: {teren.Tip}");
            throw new Exception("Terenul este invalid");
        }
        
        bool existaLocatie = _terenuri.Any(t => t.Locatie.Equals(teren.Locatie, StringComparison.OrdinalIgnoreCase));
        if (existaLocatie)
        {
            _logger.LogError($"Tentativa adaugare duplicat: {teren.Locatie}");
            throw new Exception($"Exista deja un teren cu denumirea '{teren.Locatie}'! Te rugam sa alegi alt nume.");
        }
        _terenuri.Add(teren);
        _logger.LogInfo($"Terenul {teren.Locatie} (tip:{teren.Tip}) a fost adaugat");
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
    //   ADMIN
    // ===============================
    //  1.2 MODIFICARE : PROGRAM TEREN/ INTERVAL INDISPONIBIL 
    // ===============================
    
    public void ModificaProgramTeren(Guid terenId, TimeSpan oraDeschidereNoua, TimeSpan oraInchidereNoua)
    {
        var teren = _terenuri.FirstOrDefault(t => t.Id == terenId);
        if (teren == null)  if (teren == null)
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
    
    
     
    //   CLIENT
    // ===============================
    // 1.Căutarea terenurilor disponibile
    // ===============================

  
    public List<TerenDeSport> CautaTerenuriDisponibile(TipTeren tip, IntervalOrar intervalDorit, IEnumerable<Rezervare> toateRezervarile)
    {
        if (!_terenuri.Any(t => t.Tip == tip))
        {
            _logger.LogError($"Terenul (Tip: {tip}) nu exista");
            throw new Exception("Terenul nu exista.");
        }
        return _terenuri
            .Where(t => t.Tip == tip) // Filtrare după tip
            .Where(t => t.Program.EsteDisponibil(intervalDorit)) // Verifică dacă nu e interval indisponibil (mentenanță/orar)
            .Where(t => !ExistaRezervareSuprapusa(t.Id, intervalDorit, toateRezervarile)) // Verifică să nu fie deja rezervat de altcineva
            .ToList();
    }

    private bool ExistaRezervareSuprapusa(Guid terenId, IntervalOrar interval, IEnumerable<Rezervare> rezervari)
    {
        return rezervari.Any(r => 
            r.TerenId == terenId && 
            r.Status == RezervareStatus.Activa && 
            r.Interval.SeSuprapuneCu(interval));
    }
    
    //   CLIENT
    // ===============================
    // 2.Vizualizarea detaliilor unui teren
    // ===============================
    
    public TerenDeSport? GetTeren(Guid terenId)
    {
        return _terenuri.FirstOrDefault(t => t.Id == terenId); 
    }

    //INTERVALE DISPONIBILE PENTRU UN TEREN
    
     // 1. LOGICA MATEMATICĂ (Păstrează această metodă exact cum e pentru calcule)
    public List<IntervalOrar> CalculeazaIntervaleLibere(Guid terenId, IEnumerable<Rezervare> toateRezervarile)
    {
        var teren = _terenuri.FirstOrDefault(t => t.Id == terenId);
        if (teren == null) return new List<IntervalOrar>();

        DateTime inceputProgram = DateTime.Today.Add(teren.Program.OraDeschidere);
        DateTime sfarsitProgram = DateTime.Today.Add(teren.Program.OraInchidere);

        var blocaje = teren.Program.IntervaleIndisponibile
            .Concat(toateRezervarile
                .Where(r => r.TerenId == terenId && r.Status == RezervareStatus.Activa)
                .Select(r => r.Interval))
            .OrderBy(i => i.Start).ToList();

        List<IntervalOrar> libere = new List<IntervalOrar>();
        DateTime momentCurent = inceputProgram;

        foreach (var blocaj in blocaje)
        {
            if (blocaj.Start > momentCurent)
                libere.Add(new IntervalOrar(momentCurent, blocaj.Start));
            
            if (blocaj.End > momentCurent)
                momentCurent = blocaj.End;
        }

        if (momentCurent < sfarsitProgram)
            libere.Add(new IntervalOrar(momentCurent, sfarsitProgram));

        return libere;
    }

    // 2. AFIȘAREA (Refactorizată să folosească metoda de mai sus)
    public string GenereazaFisaDisponibilitate(Guid terenId, IEnumerable<Rezervare> toateRezervarile)
    {
        var teren = _terenuri.FirstOrDefault(t => t.Id == terenId);
        if (teren == null) return "Terenul nu a fost găsit.";

        // Luăm informațiile de bază (Locație, Program, Mentenanță)
        string info = teren.GetDetaliiComplete();

        // În loc să calculăm iar intervalele ocupate, cerem direct intervalele LIBERE
        var libere = CalculeazaIntervaleLibere(terenId, toateRezervarile);

        info += "\n--- INTERVALE DISPONIBILE PENTRU REZERVARE ---\n";
        if (libere.Any())
        {
            foreach (var interval in libere)
            {
                info += $"  [LIBER]: {interval.Start:HH:mm} - {interval.End:HH:mm}\n";
            }
        }
        else
        {
            info += "  Ne pare rău, terenul este complet ocupat pentru restul zilei.\n";
        }
        return info;
    }
    
}
