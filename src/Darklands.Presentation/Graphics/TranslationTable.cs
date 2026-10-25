using System.Text;
using Darklands.Platform;

namespace Darklands.Presentation.Graphics;

/// <summary>
/// The Chinese translation of the game's English text.
/// </summary>
// The game's texts are bytes: a control code (0x01..0x1F) or a colour code (0x80..0xFF) separates the
// printable runs, and only a run is translated. A run is a maximal sequence of bytes 0x20..0x7F, so a
// whole sentence or a whole option line is one key and the codes that carry its layout are never touched.
// The key is the run's exact text, so one sentence is translated once wherever it appears, and a run the
// table does not have stays English.
//
// The card texts are translated before TextVariables.Expand, so a translation keeps the $names it had and
// the game still substitutes them; the texts that are already expanded (the data files, the interface)
// are translated where they are drawn instead.
public sealed class TranslationTable
{
    private readonly Dictionary<string, string> _translations;

    private TranslationTable(Dictionary<string, string> translations) => _translations = translations;

    /// <summary>How many English runs have a Chinese text.</summary>
    public int Count => _translations.Count;

    /// <summary>The table the game runs with; null when there is none, and every text stays English.</summary>
    public static TranslationTable? Current => _current ??= LoadDefault();

    private static TranslationTable? _current;

    /// <summary>Reads a table from a UTF-8 file of <c>english&lt;TAB&gt;chinese</c> lines.</summary>
    // A line without a tab, a blank line and a line starting with '#' is skipped, and so is a line whose
    // Chinese side is empty: the table is a work in progress and an unfinished entry must not blank a text.
    public static TranslationTable Load(string path)
    {
        using var stream = File.OpenRead(path);
        return Load(stream);
    }

    /// <summary>Reads a table from a stream of <c>english&lt;TAB&gt;chinese</c> lines.</summary>
    public static TranslationTable Load(Stream stream)
    {
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        using var reader = new StreamReader(stream, Encoding.UTF8);
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            if (line.Length == 0 || line[0] == '#')
            {
                continue;
            }
            var tab = line.IndexOf('\t');
            if (tab <= 0 || tab == line.Length - 1)
            {
                continue;
            }
            map[line[..tab]] = line[(tab + 1)..];
        }
        return new TranslationTable(map);
    }

    // The table is a file next to the game when local/paths.json or DARKLANDS_TRANSLATIONS names one, so
    // the text can be edited without rebuilding; otherwise the copy embedded in the assembly is used.
    private static TranslationTable? LoadDefault()
    {
        var path = LocalPaths.Get("translations", "DARKLANDS_TRANSLATIONS");
        if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
        {
            try
            {
                return Load(path);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"cannot read {path}: {e.Message}");
            }
        }
        var assembly = typeof(TranslationTable).Assembly;
        var name = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith("translations_zh.tsv", StringComparison.Ordinal));
        if (name is null)
        {
            return null;
        }
        using var stream = assembly.GetManifestResourceStream(name);
        return stream is null ? null : Load(stream);
    }

    /// <summary>The Chinese text of one English run, or false when the table does not have it.</summary>
    public bool TryTranslate(string english, out string chinese) => _translations.TryGetValue(english, out chinese!);

    /// <summary>Replaces the printable runs of a byte text by their Chinese; the codes pass through.</summary>
    // This is the card text's translation: it runs before TextVariables.Expand, so the $names survive.
    public byte[] Translate(byte[] text)
    {
        if (_translations.Count == 0)
        {
            return text;
        }
        var result = new List<byte>(text.Length);
        var i = 0;
        while (i < text.Length)
        {
            if (text[i] is >= 0x20 and <= 0x7F)
            {
                var start = i;
                while (i < text.Length && text[i] is >= 0x20 and <= 0x7F)
                {
                    i++;
                }
                var run = Encoding.Latin1.GetString(text, start, i - start);
                if (TryTranslate(run, out var chinese))
                {
                    result.AddRange(Encoding.UTF8.GetBytes(chinese));
                }
                else
                {
                    for (var k = start; k < i; k++)
                    {
                        result.Add(text[k]);
                    }
                }
            }
            else
            {
                result.Add(text[i]);
                i++;
            }
        }
        return [.. result];
    }

    /// <summary>Decodes a byte text to codepoints, translating its printable runs; see <see cref="TextDecoder"/>.</summary>
    // This is the translation of a text that is drawn as it is (the data files, the interface), after any
    // $names it had are already expanded.
    public int[] Decode(ReadOnlySpan<byte> text)
    {
        var result = new List<int>(text.Length);
        var i = 0;
        while (i < text.Length)
        {
            var lead = text[i];
            if (lead is >= 0x20 and <= 0x7F)
            {
                var start = i;
                while (i < text.Length && text[i] is >= 0x20 and <= 0x7F)
                {
                    i++;
                }
                var run = Encoding.Latin1.GetString(text.Slice(start, i - start));
                if (TryTranslate(run, out var chinese))
                {
                    Add(result, chinese);
                }
                else
                {
                    for (var k = start; k < i; k++)
                    {
                        result.Add(text[k]);
                    }
                }
            }
            else if (lead < 0x80)
            {
                result.Add(lead);
                i++;
            }
            else
            {
                var length = TextDecoder.SequenceLength(text, i);
                if (length > 0)
                {
                    result.Add(TextDecoder.DecodeSequence(text, i, length));
                    i += length;
                }
                else
                {
                    result.Add(-lead);
                    i++;
                }
            }
        }
        return [.. result];
    }

    private static void Add(List<int> codepoints, string text)
    {
        for (var i = 0; i < text.Length;)
        {
            var cp = char.ConvertToUtf32(text, i);
            codepoints.Add(cp);
            i += char.IsSurrogatePair(text, i) ? 2 : 1;
        }
    }
}
