using System;
using System.IO;
using System.Text;

namespace TextTokenisation;

public static class Parser
{
    public static Text ParseFile(string path)
    {
        var text = new Text();
        var sentence = new Sentence();
        var buffer = new StringBuilder();

        void FlushWord()
        {
            if (buffer.Length > 0)
            {
                sentence.Add(new Word(buffer.ToString()));
                buffer.Clear();
            }
        }

        using var reader = new StreamReader(path, Encoding.UTF8);

        int next = reader.Read();

        while (next != -1)
        {
            char c = (char)next;

            if (IsWordChar(c))
            {
                buffer.Append(c);
            }
            else
            {
                FlushWord();

                if (IsSentenceEnd(c))
                {
                    sentence.Add(new Punctuation(c));

                    next = reader.Read();

                    while (next != -1 &&
                           IsSentenceEnd((char)next))
                    {
                        sentence.Add(
                            new Punctuation((char)next));

                        next = reader.Read();
                    }

                    text.Add(sentence);
                    sentence = new Sentence();

                    continue;
                }
                else if (c ==' ')
                {
                    sentence.Add(new Punctuation(c));
                }
                else if (!char.IsWhiteSpace(c))
                {
                    sentence.Add(new Punctuation(c));
                }
            }

            next = reader.Read();
        }

        FlushWord();

        if (sentence.Tokens.Count > 0)
            text.Add(sentence);

        return text;
    }

    public static Text Parse(string input)
    {
        var temp = Path.GetTempFileName();

        File.WriteAllText(temp, input, Encoding.UTF8);

        try
        {
            return ParseFile(temp);
        }
        finally
        {
            File.Delete(temp);
        }
    }

    private static bool IsWordChar(char c) =>
        char.IsLetterOrDigit(c) ||
        c == '-' ||
        c == '\'';

    private static bool IsSentenceEnd(char c) =>
        c == '.' || c == '!' || c == '?' ||
        c == '…' || c == '？' || c == '！';
}