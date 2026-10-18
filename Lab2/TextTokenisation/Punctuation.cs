using System.Xml.Serialization;

namespace TextTokenisation;

[Serializable]
public class Punctuation : IToken
{
    [XmlText]
    public string Text { get; set; } = string.Empty;

    public Punctuation() { }

    public Punctuation(char c)
    {
        Text = c.ToString();
    }

    public Punctuation(string s)
    {
        Text = s;
    }

    public char Symbol => string.IsNullOrEmpty(Text) ? '\0' : Text[0];

    public override string ToString() => Text;
}