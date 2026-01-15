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

bool userLogat = true;
while (true)
{
    Console.WriteLine("Usename:");
    string username = Console.ReadLine();
    Console.WriteLine("Password");
    string password = Console.ReadLine();
    try
    {
        Utilizator utilizatorLogat = complex.AutentificareUtilizator(username, password);
        utilizatorLogat.ExecutaMeniu(complex);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"EROARE: {ex.Message}");
    }
}

