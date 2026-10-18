using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace TextTokenisation;

[Serializable]
[XmlType("Sentence")]
public class Sentence
{
    [XmlIgnore]
    public List<IToken> Tokens { get; set; } = new();

    [XmlElement("word", typeof(Word))]
    [XmlElement("punctuation", typeof(Punctuation))]
    public List<object> XmlTokens
    {
        get => Tokens.Cast<object>().ToList();

        set => Tokens = value
            .Cast<IToken>()
            .ToList();
    }

    [XmlIgnore]
    public List<Word> Words => Tokens.OfType<Word>().ToList();

    [XmlIgnore]
    public int WordCount => Words.Count;

    [XmlIgnore]
    public int Length => ToString().Length;

    [XmlIgnore]
    public bool IsInterrogative =>
        Tokens.OfType<Punctuation>()
            .Any(p => p.Text == "?");

    public void Add(IToken token)
    {
        Tokens.Add(token);
    }

    public void RemoveWords(Func<Word, bool> predicate)
    {
        Tokens.RemoveAll(token =>
            token is Word word && predicate(word));
    }

    public void RemoveWords(int length)
    {
        RemoveWords(word => word.Text.Length == length);
    }

    public void ReplaceWords(int length, string replacement)
    {
        for (int i = 0; i < Tokens.Count; i++)
        {
            if (Tokens[i] is Word word &&
                word.Text.Length == length)
            {
                Tokens[i] = new Word(replacement, word.LineNumber);
            }
        }
    }

    public void ReplaceWords(
        Func<Word, bool> predicate,
        string replacement)
    {
        for (int i = 0; i < Tokens.Count; i++)
        {
            if (Tokens[i] is Word word && predicate(word))
            {
                Tokens[i] = new Word(replacement, word.LineNumber);
            }
        }
    }

    public override string ToString()
    {
        return string.Concat(Tokens.Select(token => token.Text));
    }
}