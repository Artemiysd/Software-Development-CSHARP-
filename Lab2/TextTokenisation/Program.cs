using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TextTokenisation;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        string inputPath = args.Length > 0
            ? args[0]
            : "input.txt";

        if (!File.Exists(inputPath))
        {
            Console.WriteLine(
                $"Файл не найден: {Path.GetFullPath(inputPath)}");

            Console.WriteLine(
                "Создайте файл input.txt рядом с программой.");

            return;
        }

        Text text = Parser.ParseFile(inputPath);

        Console.WriteLine(
            $"Загружено предложений: {text.Sentences.Count}");

        Console.WriteLine("\n=== Все предложения ===");

        foreach (var s in text.Sentences)
        {
            Console.WriteLine(
                $"{s} [words={s.WordCount}, len={s.Length}]");
        }

        Console.WriteLine(
            "\n=== 1. Сортировка по количеству слов ===");

        foreach (var s in text.OrderByWordCount())
            Console.WriteLine($"({s.WordCount}) {s}");

        Console.WriteLine(
            "\n=== 2. Сортировка по длине ===");

        foreach (var s in text.OrderByLength())
            Console.WriteLine($"({s.Length}) {s}");

        Console.Write("\nВведите длину слова (Enter = 3): ");

        string lenInput = Console.ReadLine() ?? "";

        int searchLen = int.TryParse(lenInput, out var sl)
            ? sl
            : 3;

        Console.WriteLine(
            $"\n=== 3. Слова длины {searchLen} в вопросительных предложениях ===");

        var found = text
            .WordsOfLengthInInterrogative(searchLen)
            .ToList();

        if (found.Count == 0)
            Console.WriteLine("(ничего не найдено)");

        foreach (var w in found)
            Console.WriteLine(w);

        var copyForRemove = Parser.ParseFile(inputPath);

        copyForRemove.RemoveConsonantWords(searchLen);

        Console.WriteLine(
            $"\n=== 4. Удаление слов длины {searchLen}, начинающихся с согласной ===");

        foreach (var s in copyForRemove.Sentences)
            Console.WriteLine(s);

        var copyForReplace = Parser.ParseFile(inputPath);

        Console.Write(
            $"\nВведите номер предложения для замены (0..{copyForReplace.Sentences.Count - 1}, Enter = 0): ");

        string idxInput = Console.ReadLine() ?? "";

        int idx = int.TryParse(idxInput, out var ix)
            ? ix
            : 0;

        Console.Write(
            "Введите замену (Enter = [X]): ");

        string replacement = Console.ReadLine() ?? "";

        if (string.IsNullOrEmpty(replacement))
            replacement = "[X]";

        if (idx >= 0 &&
            idx < copyForReplace.Sentences.Count)
        {
            copyForReplace.ReplaceWordsInSentence(
                idx, searchLen, replacement);

            Console.WriteLine("\n=== 5. После замены ===");

            foreach (var s in copyForReplace.Sentences)
                Console.WriteLine(s);
        }
        else
        {
            Console.WriteLine("Неверный номер предложения.");
        }

        var stopwords = LoadStopwords("stopwords_en.txt");

        stopwords.UnionWith(
            LoadStopwords("stopwords_ru.txt"));

        var copyForStop = Parser.ParseFile(inputPath);

        copyForStop.RemoveStopwords(stopwords);

        Console.WriteLine("\n=== 6. После удаления стоп-слов ===");

        foreach (var s in copyForStop.Sentences)
            Console.WriteLine(s);

        string xmlPath = Path.ChangeExtension(
            inputPath, ".xml");

        text.ExportToXml(xmlPath);

        Console.WriteLine(
            $"\n=== 7. XML сохранён: {Path.GetFullPath(xmlPath)} ===");
        Console.WriteLine("\n=== 8. Конкорданс ===");

        string originalText = File.ReadAllText(inputPath, Encoding.UTF8);

        var concordance = text.BuildConcordance(originalText);

        foreach (var entry in concordance)
        {
            Console.WriteLine(
            $"{entry.Key,-20} | {entry.Value}");
}
    }

    private static HashSet<string> LoadStopwords(string path)
    {
        var set = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);

        if (!File.Exists(path))
        {
            Console.WriteLine(
                $"Предупреждение: файл не найден: {path}");

            return set;
        }

        foreach (var line in File.ReadLines(path, Encoding.UTF8))
        {
            var w = line.Trim();

            if (w.Length > 0 && !w.StartsWith('#'))
                set.Add(w);
        }

        return set;
    }
}