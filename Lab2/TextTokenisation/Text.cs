using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TextTokenisation;

[Serializable]
[XmlRoot("Text")]
public class Text
{
    [XmlArray("Sentences")]
    [XmlArrayItem("Sentence")]
    public List<Sentence> Sentences { get; set; } = new();

    public void Add(Sentence s) => Sentences.Add(s);

    public IEnumerable<Sentence> OrderByWordCount() =>
        Sentences.OrderBy(s => s.WordCount);

    public IEnumerable<Sentence> OrderByLength() =>
        Sentences.OrderBy(s => s.Length);

    public IEnumerable<string> WordsOfLengthInInterrogative(int length)
    {
        return Sentences
            .Where(s => s.IsInterrogative)
            .SelectMany(s => s.Words)
            .Where(w => w.Text.Length == length)
            .Select(w => w.Text.ToLowerInvariant())
            .Distinct();
    }

    public void RemoveConsonantWords(int length)
    {
        foreach (var s in Sentences)
        {
            s.RemoveWords(w =>
                w.Text.Length == length &&
                StartsWithConsonant(w.Text));
        }
    }

    public void ReplaceWordsInSentence(
        int sentenceIndex,
        int length,
        string replacement)
    {
        if (sentenceIndex < 0 ||
            sentenceIndex >= Sentences.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sentenceIndex));
        }

        Sentences[sentenceIndex].ReplaceWords(
            length, replacement);
    }

    public void RemoveStopwords(HashSet<string> stopwords)
    {
        foreach (var s in Sentences)
        {
            s.RemoveWords(w =>
                stopwords.Contains(w.Text));
        }
    }
    public SortedDictionary<string, ConcordanceEntry> BuildConcordance(string originalText)
{
    var concordance = new SortedDictionary<string, ConcordanceEntry>(
        StringComparer.OrdinalIgnoreCase);

    string[] lines = originalText.Split(
        new[] { "\r\n", "\n", "\r" },
        StringSplitOptions.None);

    for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
    {
        string line = lines[lineIndex];

        var words = System.Text.RegularExpressions.Regex.Matches(
            line,
            @"[\p{L}\p{N}]+(?:[-'][\p{L}\p{N}]+)*");

        foreach (System.Text.RegularExpressions.Match match in words)
        {
            string word = match.Value.ToLowerInvariant();

            if (!concordance.ContainsKey(word))
            {
                concordance[word] = new ConcordanceEntry();
            }

            concordance[word].AddOccurrence(lineIndex + 1);
        }
    }

    return concordance;
}

    public void ExportToXml(string path)
    {
        var serializer = new XmlSerializer(typeof(Text));

        using var writer = new StreamWriter(
            path, false, new UTF8Encoding(false));

        serializer.Serialize(writer, this);
    }

    private static readonly HashSet<char> LatinConsonants =
        new("bcdfghjklmnpqrstvwxyz");

    private static readonly HashSet<char> CyrillicConsonants =
        new("бвгджзйклмнпрстфхцчшщ");

    public static bool StartsWithConsonant(string word)
    {
        if (string.IsNullOrEmpty(word))
            return false;

        char c = char.ToLowerInvariant(word[0]);

        return LatinConsonants.Contains(c) ||
               CyrillicConsonants.Contains(c);
    }
}