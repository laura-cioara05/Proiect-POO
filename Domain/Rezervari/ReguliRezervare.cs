using PROIECT_POO.Domain.Exceptii;

namespace PROIECT_POO.Domain.Rezervari;

public class ReguliRezervare
{
    public TimeSpan DurataStandard { get;  private set;}
    public TimeSpan AnulareMinima { get;  private set;}
    
    public int NumarMaximRezervariSimultane { get; private set; }
    
    
    public ReguliRezervare(TimeSpan durata, TimeSpan anulare, int numarMaximRezervariSimultane)
    {
        DurataStandard = durata;
        AnulareMinima = anulare;
        NumarMaximRezervariSimultane = numarMaximRezervariSimultane;
    }
    
    public void ModificaDurataStandard(TimeSpan durataNoua)
    {
        if (durataNoua <= TimeSpan.Zero)
            throw new RezervareException("Durata invalida");

        DurataStandard = durataNoua;
    }
    
    public void ModificaAnulareMinima(TimeSpan nou)
    {
        if (nou < TimeSpan.Zero)
            throw new RezervareException("Timp minim de anulare invalid");

        AnulareMinima = nou;
    }
    
    public void ModificaNumarMaximRezervari(int nou)
    {
        if (nou <= 0)
            throw new RezervareException("Numar invalid");

        NumarMaximRezervariSimultane = nou;
    }
}

