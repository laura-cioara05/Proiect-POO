using System.Text.Json.Serialization;
namespace  PROIECT_POO.Domain.Utilizatori;

class AdministratorComplexSportiv:Utilizator
{
    [JsonConstructor]
    public AdministratorComplexSportiv (Guid id, string username,string password)
        : base(id, username,password) { }
}