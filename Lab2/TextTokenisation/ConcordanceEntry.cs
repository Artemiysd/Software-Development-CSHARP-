using System.Collections.Generic;

namespace TextTokenisation;

public class ConcordanceEntry
{
    public int Frequency { get; private set; }

    public SortedSet<int> LineNumbers { get; } = new();

    public void AddOccurrence(int lineNumber)
    {
        Frequency++;
        LineNumbers.Add(lineNumber);
    }

    public override string ToString()
    {
        return $"Частота: {Frequency} | Строки: {string.Join(", ", LineNumbers)}";
    }
}