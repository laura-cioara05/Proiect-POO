using System.Text.Json.Serialization;
using PROIECT_POO.Application;
using PROIECT_POO.Domain.Terenuri;

namespace  PROIECT_POO.Domain.Utilizatori;

public class AdministratorComplexSportiv:Utilizator
{
    [JsonConstructor]
    public AdministratorComplexSportiv (Guid id, string username,string password)
        : base(id, username,password) { }

    public override void AfiseazaMeniu()
    {
        Console.WriteLine(" MENIU ADMINISTRATOR ");
        Console.WriteLine("1.AdaugaTeren");
        Console.WriteLine("2.StergeTeren");
        Console.WriteLine("3.StergeTerenuriDupaTip");
        Console.WriteLine("4.ModificaProgramTeren");
        Console.WriteLine("5.AdaugaIntervalIndisponibil");
        Console.WriteLine("6.StergeIntervalIndisponibil");
        Console.WriteLine("7.ModificaDurataStandardRezervare");
        Console.WriteLine("8.ModificaAnulareMinimaRezervare");
       //? Console.WriteLine("9.ModificaAnulareMinimaRezervare");
        Console.WriteLine("10.CreeazaRezervare");
        Console.WriteLine("11.AnuleazaRezervare");
        Console.WriteLine("0. Logout");
    }

    public override void ExecutaServiciu(int serviciu, ComplexSportiv sp)
    {
        
    }
    // public override void ExecutaServiciu(int serviciu,ComplexSportiv sp)
    // {
    //     
    //     switch (serviciu)
    //     {
    //         case 1:
    //             Console.WriteLine("Indica tipul de teren");
    //             Enum.TryParse(Console.ReadLine(), out TipTeren tipTeren);
    //             var locatia = Console.ReadLine();
    //             var locatia = ;
    //             var oraDeschidere = ;
    //             var oraInchidere = ;
    //             var terenul = new TerenDeSport(new Guid(), tipTeren, "locatie", new OrarFunctionare())
    //             sp.AdaugaTeren(terenul);
    //             break;
    //         case 2:
    //             sp.StergeTeren();
    //             break;
    //         case 3:
    //             sp.StergeTerenuriDupaTip();
    //             break;
    //         case 4:
    //             sp.ModificaProgramTeren();
    //             break;
    //         case 5:
    //             sp.AdaugaIntervalIndisponibil();
    //             break;
    //         case 6:
    //             sp.StergeIntervalIndisponibil();
    //             break;
    //         case 7:
    //             sp.ModificaDurataStandardRezervare();
    //             break;
    //         case 8:
    //             sp.ModificaAnulareMinimaRezervare();
    //             break;
    //         case 9:
    //             //?
    //             break;
    //         case 10:
    //             sp.CreeazaRezervare();
    //             break;
    //         case 11:
    //             sp.AnuleazaRezervare();
    //             break;
    //         case 0:
    //             Console.WriteLine(" Logout client ");
    //             break;
    //         default:
    //             Console.WriteLine("Serviciu Invalid");
    //             break;
    //         
    //     }
    // }
    //
}