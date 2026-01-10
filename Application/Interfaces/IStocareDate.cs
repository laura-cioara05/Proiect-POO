namespace PROIECT_POO.Infrastructure;

public interface IStocareDate
{
    List<T> Incarca<T>(string caleFisier);
    void Salveaza<T>(string caleFisier, IEnumerable<T> date);
}