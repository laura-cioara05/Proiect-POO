using PROIECT_POO.Application;
using PROIECT_POO.Infrastructure;

namespace PROIECT_POO;

class Program
{
    static void Main(string[] args)
    {
        var sfdsd = new JsonStocareDate();
        ComplexSportiv c1n = new ComplexSportiv(sfdsd);
        
        var a = c1n.AutentificareUtilizator("admin","989");
        var b =c1n.AutentificareUtilizator("client","7645");

    }
}