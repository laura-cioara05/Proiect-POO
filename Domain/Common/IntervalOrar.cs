using System.Text.Json.Serialization;
namespace PROIECT_POO.Domain.Common;

public sealed class IntervalOrar
{
    public DateTime Start { get;}
    public DateTime End { get;} 
    
    public TimeSpan Durata => End - Start;
    
    [JsonConstructor]
    public IntervalOrar(DateTime start, DateTime end)
    {
        if (end <= start)
            throw new ArgumentException("Sfârșitul trebuie să fie după început.");

        Start = start;
        End = end;
    }
    
    public bool SeSuprapuneCu(IntervalOrar alt)
    {
        return Start < alt.End && alt.Start < End;
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is IntervalOrar altul)
        {
            return this.Start == altul.Start && this.End == altul.End;
        }
        return false;
    }

    public override int GetHashCode() => HashCode.Combine(Start, End);
    
    public override string ToString()
    {
        if (Start.Date == End.Date)
        {
            return $"{Start:yyyy-MM-dd HH:mm} - {End:HH:mm}";
        }
        return $"{Start:yyyy-MM-dd HH:mm} - {End:yyyy-MM-dd HH:mm}";
    }
}
