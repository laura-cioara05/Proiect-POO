using PROIECT_POO.Domain.Utilizatori;
using PROIECT_POO.Infrastructure;
using Microsoft.Extensions.Logging;
namespace PROIECT_POO.Application;

public class Autentificare 
{
    private readonly List<Utilizator> _utilizatori;
    //private readonly ILogger _logger;
   // public Autentificare(IStocareDate stocareDate,ILogger logger)
    public Autentificare(IStocareDate stocareDate)

    {
        //Incarca utilizatorii din fisierul JSON
        _utilizatori = stocareDate.Incarca<Utilizator>("C:\\Users\\Diana\\Desktop\\POO\\Proiect Rezervare terenuri sport\\Proiect-POO\\Infrastructure\\JsonFiles\\utilizatori.json");
        //_logger = logger;
    }

    public Utilizator Login(string username, string password)
    {
        var utilizator =
            _utilizatori.FirstOrDefault(u => u.Password == password && u.Username == username);
        if (utilizator == null)
        {
           // _logger.LogError("Username sau Password incorect");
            throw new Exception("Username sau Password incorect");
        }
       // _logger.LogInformation("Utilizator {Username} logat cu succes}");

        return utilizator;
    }
}