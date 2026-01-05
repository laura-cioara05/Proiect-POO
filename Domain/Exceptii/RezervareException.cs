namespace PROIECT_POO.Domain.Exceptii;

public class RezervareException : Exception
{
    public RezervareException(){}

    public RezervareException(string message) : base(message){}
       
    public RezervareException(string message, Exception inner) : base(message, inner){}
}

