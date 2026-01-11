using PROIECT_POO.Application;
using PROIECT_POO.Application.Interfaces;
using PROIECT_POO.Domain.Common;
using PROIECT_POO.Domain.Terenuri;
using PROIECT_POO.Domain.Utilizatori;
using PROIECT_POO.Infrastructure;
using PROIECT_POO.Infrastructure.Logging;

// 1. Initializare
IStocareDate storage = new JsonStocareDate();
ILogger logger = new ConsoleLogger();
ComplexSportiv complex = new ComplexSportiv(storage,logger);

// Simulare Login (In realitate aici ai cere email/parola)
// Cream obiectele de test pentru a vedea cum functioneaza polimorfismul
Utilizator adminLogat = new AdministratorComplexSportiv(Guid.NewGuid(), "Admin_Sef","pa");
Utilizator clientLogat = new Client(Guid.Parse("00000000-0000-0000-0000-000000000001"), "Ion_Popescu", "pass");

bool rulare = true;
while (rulare)
{
    Console.Clear();
    Console.WriteLine("=== SISTEM GESTIUNE COMPLEX SPORTIV ===");
    Console.WriteLine("1. Intre ca ADMIN (Simulare)");
    Console.WriteLine("2. Intre ca CLIENT (Simulare)");
    Console.WriteLine("0. Iesire");
    Console.Write("\nSelectati rolul: ");

    switch (Console.ReadLine())
    {
        case "1": MeniuAdmin(complex, adminLogat); break;
        case "2": MeniuClient(complex, clientLogat); break;
        case "0": rulare = false; break;
    }
}

void MeniuAdmin(ComplexSportiv complex, Utilizator user)
{
    bool inAdmin = true;
    while (inAdmin)
    {
        Console.Clear();
        Console.WriteLine($"--- PANOU ADMIN (Logat ca: {user.Username}) ---");
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
        Console.WriteLine("0. Inapoi");
        
        Console.Write("\nOptiune: ");
        string opt = Console.ReadLine();

        try {
            switch (opt)
            {
                case "1":
                    Console.Write("Locatie: "); string locatie = Console.ReadLine();
                    Console.Write("Tip (0-Fotbal, 1-Tenis, 2-Baschet): "); TipTeren tip = (TipTeren)int.Parse(Console.ReadLine());
                    complex.AdaugaTeren(new TerenDeSport(Guid.NewGuid(),tip, locatie,new OrarFunctionare(TimeSpan.FromHours(8), TimeSpan.FromHours(22))));
                    break;
                case "2":
                    Console.Write("ID Teren: "); complex.StergeTeren(Guid.Parse(Console.ReadLine()));
                    break;
                case "3":
                    Console.Write("Tip (0, 1, 2): "); complex.StergeTerenuriDupaTip((TipTeren)int.Parse(Console.ReadLine()));
                    break;
                case "4":
                    Console.Write("ID Teren: "); Guid tId = Guid.Parse(Console.ReadLine());
                    Console.Write("Ora Deschidere (HH:mm): "); TimeSpan od = TimeSpan.Parse(Console.ReadLine());
                    Console.Write("Ora Inchidere (HH:mm): "); TimeSpan oi = TimeSpan.Parse(Console.ReadLine());
                    complex.ModificaProgramTeren(tId, od, oi);
                    break;
                case "5":
                    Console.Write("ID Teren: "); Guid tIdM = Guid.Parse(Console.ReadLine());
                    Console.Write("Inceput Mentenanta (yyyy-MM-dd HH:mm): "); DateTime sm = DateTime.Parse(Console.ReadLine());
                    Console.Write("Sfarsit Mentenanta (yyyy-MM-dd HH:mm): "); DateTime em = DateTime.Parse(Console.ReadLine());
                    complex.AdaugaIntervalIndisponibil(tIdM, new IntervalOrar(sm, em));
                    break;
                case "7":
                    Console.Write("ID Teren: "); var active = complex.GetRezervariActiveTeren(Guid.Parse(Console.ReadLine()));
                    foreach(var r in active) Console.WriteLine($"ID: {r.Id} | Start: {r.Interval.Start}");
                    break;
                case "9":
                    Console.WriteLine("a. Modifica Durata Minima (HH:mm)");
                    if(Console.ReadLine() == "a") complex.ModificaDurataStandardRezervare(TimeSpan.Parse(Console.ReadLine()));
                    break;
                case "10":
                    Console.Write("ID Rezervare: "); complex.AnuleazaRezervare(Guid.Parse(Console.ReadLine()), user);
                    break;
                case "11":
                    Console.Write("ID Rezervare: "); Guid rezId = Guid.Parse(Console.ReadLine());
                    Console.Write("Data Noua Start: "); DateTime ns = DateTime.Parse(Console.ReadLine());
                    complex.ModificaRezervare(rezId, user, new IntervalOrar(ns, ns.AddHours(1)));
                    break;
                case "0": inAdmin = false; break;
            }
        } catch (Exception ex) { Console.WriteLine($"EROARE: {ex.Message}"); }
        if(opt != "0") { Console.WriteLine("\nApasati tasta..."); Console.ReadKey(); }
    }
}

void MeniuClient(ComplexSportiv complex, Utilizator user)
{
    bool inClient = true;
    while (inClient)
    {
        Console.Clear();
        Console.WriteLine($"--- MENIU CLIENT (Logat ca: {user.Username}) ---");
        Console.WriteLine("1. Cauta Terenuri Libere dupa Tip si Interval");
        Console.WriteLine("2. Vezi Info Detaliate Teren");
        Console.WriteLine("3. Vezi Toate Intervale Libere (Azi)");
        Console.WriteLine("4. Creaza Rezervare");
        Console.WriteLine("5. Rezervarile mele ACTIVE");
        Console.WriteLine("6. Istoricul meul de rezervari");
        Console.WriteLine("7. Anuleaza o rezervare proprie");
        Console.WriteLine("8. Modifica o rezervare proprie");
        Console.WriteLine("0. Inapoi");

        Console.Write("\nOptiune: ");
        string opt = Console.ReadLine();

        try {
            switch (opt)
            {
                case "1":
                    Console.Write("Tip (0,1,2): "); TipTeren t = (TipTeren)int.Parse(Console.ReadLine());
                    Console.Write("Start (yyyy-MM-dd HH:mm): "); DateTime s = DateTime.Parse(Console.ReadLine());
                    var libere = complex.CautaTerenuriLibere(t, new IntervalOrar(s, s.AddHours(1)));
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
                    complex.CreeazaRezervare(user.Id, tid, new IntervalOrar(rs, rs.AddHours(1)));
                    break;
                case "5":
                    var active = complex.GetRezervariActiveClient(user.Id);
                    foreach(var r in active) Console.WriteLine($"ID: {r.Id} | Teren: {r.TerenId} | Data: {r.Interval.Start}");
                    break;
                case "7":
                    Console.Write("ID Rezervare: "); complex.AnuleazaRezervare(Guid.Parse(Console.ReadLine()), user);
                    break;
                case "8":
                    Console.Write("ID Rezervare: "); Guid rid = Guid.Parse(Console.ReadLine());
                    Console.Write("Data Noua Start: "); DateTime nrs = DateTime.Parse(Console.ReadLine());
                    complex.ModificaRezervare(rid, user, new IntervalOrar(nrs, nrs.AddHours(1)));
                    break;
                case "0": inClient = false; break;
            }
        } catch (Exception ex) { Console.WriteLine($"EROARE: {ex.Message}"); }
        if(opt != "0") { Console.WriteLine("\nApasati tasta..."); Console.ReadKey(); }
    }
}