using PROIECT_POO.Application;
using PROIECT_POO.Infrastructure;
using PROIECT_POO.Infrastructure.Logging;

namespace PROIECT_POO;

class Program
{
    static void Main(string[] args)
    {
        ILogger logger = new ConsoleLogger();
        var sfdsd = new JsonStocareDate();
        ComplexSportiv c1n = new ComplexSportiv(sfdsd,logger);
        
        var a = c1n.AutentificareUtilizator("admin","989");
        var b =c1n.AutentificareUtilizator("client","7645");

    }
}