using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

class Program
{
    static Dictionary<string, Rgba32> ColorMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "красн", new Rgba32(255, 0, 0) },
            { "ал", new Rgba32(220, 20, 60) },
            { "багр", new Rgba32(139, 0, 0) },

            { "зелен", new Rgba32(0, 128, 0) },
            { "изумруд", new Rgba32(60, 179, 113) },
            { "малахит", new Rgba32(60, 179, 113) },

            { "син", new Rgba32(0, 0, 255) },
            { "голуб", new Rgba32(173, 216, 230) },
            { "лазур", new Rgba32(135, 206, 250) },
            { "ультрамарин", new Rgba32(0, 0, 205) },

            { "желт", new Rgba32(255, 255, 0) },
            { "золот", new Rgba32(255, 215, 0) },
            { "лимон", new Rgba32(255, 250, 205) },

            { "бел", new Rgba32(255, 255, 255) },
            { "черн", new Rgba32(0, 0, 0) },
            { "сер", new Rgba32(128, 128, 128) },

            { "фиолетов", new Rgba32(128, 0, 128) },
            { "лилов", new Rgba32(128, 0, 128) },

            { "оранжев", new Rgba32(255, 165, 0) },
            { "коричнев", new Rgba32(165, 42, 42) },
            { "розов", new Rgba32(255, 192, 203) },
            { "бирюз", new Rgba32(64, 224, 208) }
        };

    static HashSet<string> ColorEndings = new()
    {
        "ый",
        "ий",
        "ой",

        "ая",
        "яя",
        "оя",

        "ое",
        "ее",

        "ые",
        "ие",

        "ым",
        "им",

        "ом",
        "ем",

        "ых",
        "их",

        "ую",
        "юю",

        "ого",
        "его",

        "ому",
        "ему",

        "ыми",
        "ими",

        "ой",
        "ей"
    };

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("=================================");
        Console.WriteLine("     АНАЛИЗАТОР ЦВЕТОВ ТЕКСТА");
        Console.WriteLine("=================================");
        Console.WriteLine();

        Console.Write("Введите путь к TXT-файлу: ");
        string? path = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(path))
        {
            Console.WriteLine("Ошибка: путь не указан.");
            return;
        }

        if (!File.Exists(path))
        {
            Console.WriteLine("Ошибка: файл не найден.");
            return;
        }

        if (!Path.GetExtension(path).Equals(
                ".txt",
                StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Ошибка: файл должен быть TXT.");
            return;
        }

        try
        {
            string text = File.ReadAllText(
                path,
                Encoding.UTF8
            );

            if (string.IsNullOrWhiteSpace(text))
            {
                Console.WriteLine("Ошибка: файл пуст.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Файл успешно прочитан.");

            text = text.ToLower();

            text = Regex.Replace(
                text,
                @"[^\p{L}\p{N}\s]",
                " "
            );

            string[] words = text.Split(
                new[] { ' ', '\n', '\r', '\t' },
                StringSplitOptions.RemoveEmptyEntries
            );

            Console.WriteLine(
                $"Количество слов: {words.Length}"
            );

            List<Rgba32> foundColors = new();
            List<string> foundColorNames = new();
            List<string> foundWords = new();

            foreach (string word in words)
            {
                foreach (var color in ColorMap)
                {
                    if (IsColorWord(word, color.Key))
                    {
                        foundColors.Add(color.Value);
                        foundColorNames.Add(color.Key);
                        foundWords.Add(word);

                        break;
                    }
                }
            }


            Console.WriteLine(
                $"Найдено упоминаний цветов: {foundColors.Count}"
            );
            for (int i = 0; i < foundWords.Count; i++)
            {
                Console.WriteLine(
                    $"Слово: {foundWords[i]}, Цвет: {foundColorNames[i]}"
                );
            }

            if (foundColors.Count == 0)
            {
                Console.WriteLine(
                    "Цветовые слова не найдены."
                );

                return;
            }

            Dictionary<string, int> statistics =
                new(StringComparer.OrdinalIgnoreCase);

            foreach (string colorName in foundColorNames)
            {
                if (statistics.ContainsKey(colorName))
                {
                    statistics[colorName]++;
                }
                else
                {
                    statistics[colorName] = 1;
                }
            }

            Console.WriteLine();
            Console.WriteLine("========== СТАТИСТИКА ==========");

            foreach (var stat in statistics
                         .OrderByDescending(x => x.Value))
            {
                Console.WriteLine(
                    $"{stat.Key}: {stat.Value}"
                );
            }

            Console.WriteLine();
            Console.WriteLine("Выберите визуализацию:");
            Console.WriteLine("1 - Линия");
            Console.WriteLine("2 - Сетка");

            Console.Write("Ваш выбор: ");
            string? choice = Console.ReadLine();

            if (choice != "1" && choice != "2")
            {
                Console.WriteLine(
                    "Ошибка: выберите 1 или 2."
                );

                return;
            }

            const int squareSize = 50;
            const int margin = 10;

            int columns;

            if (choice == "1")
            {
                columns = foundColors.Count;
            }
            else
            {
                columns = Math.Min(
                    20,
                    foundColors.Count
                );
            }

            int rows = (int)Math.Ceiling(
                foundColors.Count / (double)columns
            );

            int width =
                columns * squareSize + margin * 2;

            int height =
                rows * squareSize + margin * 2;

            using Image<Rgba32> image =
                new(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    image[x, y] =
                        new Rgba32(255, 255, 255);
                }
            }

            for (int i = 0; i < foundColors.Count; i++)
            {
                int row = i / columns;
                int column = i % columns;

                int startX =
                    margin + column * squareSize;

                int startY =
                    margin + row * squareSize;

                Rgba32 color = foundColors[i];

                for (
                    int y = startY;
                    y < startY + squareSize && y < height;
                    y++
                )
                {
                    for (
                        int x = startX;
                        x < startX + squareSize && x < width;
                        x++
                    )
                    {
                        image[x, y] = color;
                    }
                }
            }

            string outputFile =
                "colors_palette.png";

            image.Save(outputFile);

            Console.WriteLine();
            Console.WriteLine(
                "Палитра успешно создана!"
            );

            Console.WriteLine(
                $"Файл: {outputFile}"
            );

            Console.WriteLine(
                $"Полный путь: {Path.GetFullPath(outputFile)}"
            );
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine(
                "Ошибка: нет доступа к файлу."
            );
        }
        catch (IOException ex)
        {
            Console.WriteLine(
                $"Ошибка ввода-вывода: {ex.Message}"
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Произошла ошибка: {ex.Message}"
            );
        }
    }

    static bool IsColorWord(string word, string root)
    {
        if (!word.StartsWith(
                root,
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        string ending = word.Substring(root.Length);

        if (string.IsNullOrEmpty(ending))
        {
            return false;
        }

        return ColorEndings.Contains(ending);
    }
}