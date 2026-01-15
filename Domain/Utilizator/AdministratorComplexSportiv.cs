using System.Text.Json.Serialization;
using PROIECT_POO.Application;
using PROIECT_POO.Domain.Common;
using PROIECT_POO.Domain.Terenuri;

namespace  PROIECT_POO.Domain.Utilizatori;

class AdministratorComplexSportiv:Utilizator
{
    public AdministratorComplexSportiv (Guid id, string username,string password)
        : base(id, username,password) { }

    public override void AfisareMeniu()
    {
        Console.WriteLine($"--- PANOU ADMIN (Logat ca: {Username}) ---");
        Console.WriteLine("1. Adauga Teren Nou"); 
        Console.WriteLine("2. Sterge Teren (dupa ID)");
        Console.WriteLine("3. Sterge toate terenurile de un anumit Tip");
        Console.WriteLine("4. Modifica Program Functionare Teren");
        Console.WriteLine("5. Adauga Interval Mentenanta (Indisponibil)");
        Console.WriteLine("6. Sterge Interval Mentenanta");
        Console.WriteLine("7. Vezi Rezervari Active Teren");
        Console.WriteLine("8. Vezi Istoric Rezervari Teren");
        Console.WriteLine("9. Modifica Reguli Globale (Durata/Anulare/Max)");
        Console.WriteLine("10. Anuleaza Rezervare (Orice rezervare)");
        Console.WriteLine("11. Modifica Rezervare (Orice rezervare)");
        Console.WriteLine("0. Iesire");
    }

    public override void ExecutaMeniu(ComplexSportiv complex)
    {
        bool activ = true;
        while (activ)
        {
            Console.Clear();
            AfisareMeniu();
            Console.WriteLine("\n Optiune: ");
            string opt = Console.ReadLine();
            try
            {
                switch (opt)
                {
                    case "1":
                    Console.Write("Locatie: "); string locatie = Console.ReadLine();
                    Console.Write("Tip (0-Fotbal, 1-Tenis, 2-Baschet, 3-Volei, 4-Handbal): "); TipTeren tip = (TipTeren)int.Parse(Console.ReadLine());
                    complex.AdaugaTeren(new TerenDeSport(Guid.NewGuid(),tip, locatie,new OrarFunctionare(TimeSpan.FromHours(8), TimeSpan.FromHours(22))));
                    break;
                case "2":
                    Console.Write("ID Teren: "); complex.StergeTeren(Guid.Parse(Console.ReadLine()));
                    break;
                case "3":
                    Console.Write("Tip (0-Fotbal, 1-Tenis, 2-Baschet, 3-Volei, 4-Handbal): "); complex.StergeTerenuriDupaTip((TipTeren)int.Parse(Console.ReadLine()));
                    break;
                case "4":
                    Console.Write("ID Teren: "); Guid tId = Guid.Parse(Console.ReadLine());
                    Console.Write("Ora Deschidere (HH:mm): "); TimeSpan od = TimeSpan.Parse(Console.ReadLine());
                    Console.Write("Ora Inchidere (HH:mm): "); TimeSpan oi = TimeSpan.Parse(Console.ReadLine());
                    complex.ModificaProgramTeren(tId, od, oi);
                    break;
                case "5":
                    Console.Write("ID Teren: "); Guid terenID = Guid.Parse(Console.ReadLine());
                    Console.Write("Inceput Mentenanta (yyyy-MM-dd HH:mm): "); DateTime inceput = DateTime.Parse(Console.ReadLine());
                    Console.Write("Sfarsit Mentenanta (yyyy-MM-dd HH:mm): "); DateTime sfarsit = DateTime.Parse(Console.ReadLine());
                    complex.AdaugaIntervalIndisponibil(terenID, new IntervalOrar(inceput, sfarsit));
                    break;
                case "6":
                    Console.Write("ID Teren: "); Guid terenId = Guid.Parse(Console.ReadLine());
                    Console.Write("Inceput Mentenanta (yyyy-MM-dd HH:mm): "); DateTime sm = DateTime.Parse(Console.ReadLine());
                    Console.Write("Sfarsit Mentenanta (yyyy-MM-dd HH:mm): "); DateTime em = DateTime.Parse(Console.ReadLine());
                    complex.StergeIntervalIndisponibil(terenId, new IntervalOrar(sm, em));
                    break;
                case "7":
                    Console.Write("ID Teren: "); var active = complex.GetRezervariActiveTeren(Guid.Parse(Console.ReadLine()));
                    foreach(var r in active) Console.WriteLine($"ID: {r.Id} | Start: {r.Interval.Start}");
                    break;
                case "8":
                    Console.Write("ID Teren: "); var istorice = complex.GetRezervariIstoriceTeren(Guid.Parse(Console.ReadLine()));
                    foreach(var r in istorice) Console.WriteLine($"ID: {r.Id} | Start: {r.Interval.Start}");
                    break;
                case "9":
                    bool inReguli = true;
                    while (inReguli)
                    {
                    Console.Clear(); 
                    
                    Console.WriteLine("\n--- MODIFICARE REGULI GLOBALE ---");
                    Console.WriteLine("a. Modifica Durata Standard (cat dureaza o rezervare)");
                    Console.WriteLine("b. Modifica Timp Minim Anulare (cu cat timp inainte se poate anula)");
                    Console.WriteLine("c. Modifica Numar Maxim Rezervari/Client");
                    Console.WriteLine("0. Inapoi la meniul principal");
                    Console.Write("Selectati regula (a/b/c): ");
    
                    string subOptiune = Console.ReadLine();

                        switch (subOptiune)
                        {
                            case "a":
                                Console.Write("Introdu noua durata standard (HH:mm): ");
                                if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan ds))
                                {
                                    complex.ModificaDurataStandardRezervare(ds);
                                    Console.WriteLine("Durata standard a fost actualizata!");
                                }
                                else Console.WriteLine("Format invalid!");
                                break;

                            case "b":
                                Console.Write("Introdu timpul minim de anulare (HH:mm): ");
                                if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan ta))
                                {
                                    complex.ModificaAnulareMinimaRezervare(ta);
                                    Console.WriteLine("Timpul de anulare a fost actualizat!");
                                }
                                else Console.WriteLine("Format invalid!");
                                break;

                            case "c":
                                Console.Write("Introdu numarul maxim de rezervari simultane: ");
                                if (int.TryParse(Console.ReadLine(), out int max))
                                {
                                    complex.ModificaNumarMaximRezervariSimultane(max);
                                    Console.WriteLine("Limita de rezervari a fost actualizata!");
                                }
                                else Console.WriteLine("Numar invalid!");
                                break;
                            case "0":
                                inReguli = false;
                                break;
                            default:
                                Console.WriteLine("Optiune invalida.");
                                break;
                        }   
                        Console.WriteLine("\nApasati orice tasta pentru a continua...");
                        Console.ReadKey();
                    }
                    break;
                case "10":
                    Console.Write("ID Rezervare: "); complex.AnuleazaRezervare(Guid.Parse(Console.ReadLine()), this);
                    break;
                case "11":
                    Console.Write("ID Rezervare: "); Guid rezId = Guid.Parse(Console.ReadLine());
                    Console.Write("Data Noua Start (yyyy-MM-dd HH:mm): "); DateTime ns = DateTime.Parse(Console.ReadLine());
                    complex.ModificaRezervare(rezId, this, new IntervalOrar(ns, ns.Add(complex.DURATA_REZERVARE_STANDART)));
                    break;
                case "0": activ = false; break;
                }
            }catch(Exception ex) { Console.WriteLine($"EROARE: {ex.Message}"); }
            if(opt != "0") { Console.WriteLine("\nApasati tasta..."); Console.ReadKey(); }
        }
    }
}