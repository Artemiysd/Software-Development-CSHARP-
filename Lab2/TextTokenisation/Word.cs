using System;
using System.Xml.Serialization;

namespace TextTokenisation;

[Serializable]
public class Word : IToken
{
    [XmlText]
    public string Text { get; set; } = "";

    [XmlIgnore]
    public int LineNumber { get; set; }

    public Word()
    {
    }

    public Word(string text)
    {
        Text = text;
    }

    public Word(string text, int lineNumber)
    {
        Text = text;
        LineNumber = lineNumber;
    }
}