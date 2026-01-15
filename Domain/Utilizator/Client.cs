using System.Text.Json.Serialization;
using PROIECT_POO.Application;
using PROIECT_POO.Domain.Common;
using PROIECT_POO.Domain.Terenuri;

namespace  PROIECT_POO.Domain.Utilizatori;

class Client:Utilizator
{
    public Client(Guid id, string username,string password)
        : base(id, username, password) { }

    public override void AfisareMeniu()
    {
        Console.WriteLine($"--- MENIU CLIENT (Logat ca: {Username}) ---");
        Console.WriteLine("1. Cauta Terenuri Libere dupa Tip si Interval");
        Console.WriteLine("2. Vezi Info Detaliate Teren");
        Console.WriteLine("3. Vezi Toate Intervale Libere (Azi)");
        Console.WriteLine("4. Creaza Rezervare");
        Console.WriteLine("5. Rezervarile mele ACTIVE");
        Console.WriteLine("6. Istoricul meul de rezervari");
        Console.WriteLine("7. Anuleaza o rezervare proprie"); 
        Console.WriteLine("8. Modifica o rezervare proprie");
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
                        Console.Write("Tip (0-Fotbal, 1-Tenis, 2-Baschet, 3-Volei, 4-Handbal): "); TipTeren t = (TipTeren)int.Parse(Console.ReadLine());
                        Console.Write("Start (yyyy-MM-dd HH:mm): "); DateTime s = DateTime.Parse(Console.ReadLine());
                        var libere = complex.CautaTerenuriLibere(t, new IntervalOrar(s, s.Add(complex.DURATA_REZERVARE_STANDART)));
                        foreach(var ter in libere) Console.WriteLine($"Disponibil: {ter.Locatie} (ID: {ter.Id})");
                        break;
                    case "2":
                        Console.Write("ID Teren: "); Console.WriteLine(complex.GetInfoTeren(Guid.Parse(Console.ReadLine())));
                        break;
                    case "3":
                        Console.Write("ID Teren: "); Console.WriteLine(complex.GetIntervaleLibereText(Guid.Parse(Console.ReadLine())));
                        break;
                    case "4":
                        Console.Write("ID Teren: "); Guid tid = Guid.Parse(Console.ReadLine());
                        Console.Write("Start (yyyy-MM-dd HH:mm): "); DateTime rs = DateTime.Parse(Console.ReadLine());
                        complex.CreeazaRezervare(this.Id, tid, new IntervalOrar(rs, rs.Add(complex.DURATA_REZERVARE_STANDART)));
                        break;
                    case "5":
                        var active = complex.GetRezervariActiveClient(this.Id);
                        foreach(var r in active) Console.WriteLine($"ID: {r.Id} | Teren: {r.TerenId} | Data: {r.Interval.Start}");
                        break;
                    case "6":
                        Console.Write("ID Client: "); var istorice = complex.GetIstoricRezervariClient(Guid.Parse(Console.ReadLine()));
                        foreach(var r in istorice) Console.WriteLine($"ID: {r.Id} | Start: {r.Interval.Start}");
                        break;
                    case "7":
                        Console.Write("ID Rezervare: "); complex.AnuleazaRezervare(Guid.Parse(Console.ReadLine()), this);
                        break;
                    case "8":
                        Console.Write("ID Rezervare: "); Guid rid = Guid.Parse(Console.ReadLine());
                        Console.Write("Data Noua Start: "); DateTime nrs = DateTime.Parse(Console.ReadLine());
                        complex.ModificaRezervare(rid, this, new IntervalOrar(nrs, nrs.Add(complex.DURATA_REZERVARE_STANDART)));
                        break;
                    case "0": activ = false; break;
                }
            }catch(Exception ex) { Console.WriteLine($"EROARE: {ex.Message}"); }
            if(opt != "0") { Console.WriteLine("\nApasati tasta..."); Console.ReadKey(); }
        }
    }
}