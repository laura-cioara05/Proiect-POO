using System.Text.Json.Serialization;
using PROIECT_POO.Application;
namespace PROIECT_POO.Domain.Utilizatori;


[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonDerivedType(typeof(AdministratorComplexSportiv), "Admin")]
[JsonDerivedType(typeof(Client), "Client")]
public abstract class Utilizator
{
    public Guid Id { get; }
    public string Username { get; }
    public string Password { get; }
    public string Email { get; }
    public string Telefon { get; }

    protected Utilizator(Guid id, string username, string password, string email, string telefon)
    {
        Id = id;
        Username = username;
        Password = password;
        Email = email;
        Telefon = telefon;
    }
}