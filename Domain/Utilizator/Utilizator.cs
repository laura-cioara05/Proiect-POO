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

    [JsonConstructor]
    protected Utilizator(Guid id, string username, string password)
    {
        Id = id;
        Username = username;
        Password = password;
    }

    public abstract void AfisareMeniu();
    public abstract void ExecutaMeniu(ComplexSportiv complex);
}