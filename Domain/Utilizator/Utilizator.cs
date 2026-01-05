using System.Text.Json.Serialization;
namespace PROIECT_POO.Domain.Utilizatori;

public abstract class Utilizator
{
    public Guid Id { get; }
    public string Username { get; }
    public string Password { get; }
    [JsonConstructor]
    protected Utilizator(Guid id, string username,string password)
    {
        Id = id;
        Username = username;
        Password = password;
    }
}