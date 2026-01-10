using System.Text.Json.Serialization;
namespace  PROIECT_POO.Domain.Utilizatori;

class Client:Utilizator
{
    [JsonConstructor]
    public Client(Guid id, string username,string password)
        : base(id, username, password) { }
}