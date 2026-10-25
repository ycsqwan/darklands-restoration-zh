namespace Darklands.Presentation.Graphics;

/// <summary>
/// Turns the game's byte text into codepoints.
/// </summary>
// The game's own texts are bytes: 0x00 ends a string, 0x01..0x1F are layout control codes, 0x20..0x7F are
// characters, and a byte with the top bit set is a colour code (TextPainter reads them that way). Chinese
// text is UTF-8, so a byte 0x80..0xFF that begins a valid UTF-8 sequence is read as one codepoint and any
// other byte 0x80..0xFF stays a colour code. Every colour code the game uses (0x80..0x8F, 0xFF) is an
// invalid UTF-8 start byte, so the original texts decode exactly as they read today.
public static class TextDecoder
{
    /// <summary>Reads <paramref name="text"/> as codepoints; a colour code comes back as a negative byte.</summary>
    // A translation table, when the game has one, replaces the printable runs on the way through; the
    // codes are still read here, so a translated text decodes exactly like the original.
    public static int[] Decode(ReadOnlySpan<byte> text) =>
        TranslationTable.Current is { } table ? table.Decode(text) : DecodeLiteral(text);

    /// <summary>Reads <paramref name="text"/> as codepoints without consulting any translation table.</summary>
    public static int[] DecodeLiteral(ReadOnlySpan<byte> text)
    {
        var codepoints = new int[text.Length];
        var count = 0;
        for (var i = 0; i < text.Length;)
        {
            var lead = text[i];
            if (lead < 0x80)
            {
                codepoints[count++] = lead;
                i++;
                continue;
            }
            var length = SequenceLength(text, i);
            if (length > 0)
            {
                codepoints[count++] = DecodeSequence(text, i, length);
                i += length;
            }
            else
            {
                codepoints[count++] = -lead;
                i++;
            }
        }
        return count == codepoints.Length ? codepoints : [.. codepoints[..count]];
    }

    /// <summary>Whether a UTF-8 sequence starts here, and how long it is (0 when the byte is not a valid lead).</summary>
    internal static int SequenceLength(ReadOnlySpan<byte> text, int i)
    {
        var lead = text[i];
        var length = lead switch
        {
            >= 0xC2 and <= 0xDF => 2,
            >= 0xE0 and <= 0xEF => 3,
            >= 0xF0 and <= 0xF4 => 4,
            _ => 0,
        };
        if (length == 0 || i + length > text.Length)
        {
            return 0;
        }
        for (var continuation = 1; continuation < length; continuation++)
        {
            if (text[i + continuation] is < 0x80 or > 0xBF)
            {
                return 0;
            }
        }
        // An overlong form, a surrogate or anything above U+10FFFF is not a sequence the encoding allows.
        return DecodeSequence(text, i, length) switch
        {
            >= 0x110000 => 0,
            >= 0xD800 and <= 0xDFFF => 0,
            var codepoint => length switch
            {
                2 => codepoint >= 0x80 ? 2 : 0,
                3 => codepoint >= 0x800 ? 3 : 0,
                _ => codepoint >= 0x10000 ? 4 : 0,
            },
        };
    }

    internal static int DecodeSequence(ReadOnlySpan<byte> text, int i, int length) => length switch
    {
        2 => (text[i] & 0x1F) << 6 | (text[i + 1] & 0x3F),
        3 => (text[i] & 0x0F) << 12 | (text[i + 1] & 0x3F) << 6 | (text[i + 2] & 0x3F),
        _ => (text[i] & 0x07) << 18 | (text[i + 1] & 0x3F) << 12 | (text[i + 2] & 0x3F) << 6 | (text[i + 3] & 0x3F),
    };
}
