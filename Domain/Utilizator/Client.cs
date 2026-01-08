using System.Text.Json.Serialization;
using PROIECT_POO.Application;

namespace  PROIECT_POO.Domain.Utilizatori;

public class Client:Utilizator
{
    [JsonConstructor]
    public Client(Guid id, string username,string password)
        : base(id, username, password) { }

    
    public override void AfiseazaMeniu()
    {
        Console.WriteLine(" MENIU CLIENT ");
        Console.WriteLine("1.Cautarea Teren Disponibil");
        Console.WriteLine("2.Vizualizare Detalii Teren");
        Console.WriteLine("3.Creeaza Rezervare");
        Console.WriteLine("4.Gestionare Rezervare Personala");
        Console.WriteLine("0. Logout");
    }

    public override void ExecutaServiciu(int serviciu,ComplexSportiv sp)
    {
        switch (serviciu)
        {
            case 1:
                
                break;
            case 2:
                
                break;
            case 3:
                
                break;
            case 4:
                
                break;
            case 0:
                Console.WriteLine(" Logout client ");
                break;
            default:
                Console.WriteLine("Serviciu Invalid");
                break;
        }
    }
}