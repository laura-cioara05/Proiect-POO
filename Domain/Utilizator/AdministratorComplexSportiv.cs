using System.Text.Json.Serialization;
namespace  PROIECT_POO.Domain.Utilizatori;

class AdministratorComplexSportiv:Utilizator
{
    public AdministratorComplexSportiv (Guid id, string username,string password)
        : base(id, username,password) { }
}