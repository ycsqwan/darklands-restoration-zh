using Darklands.Content.Formats.Graphics;

namespace Darklands.Presentation.Graphics;

/// <summary>Draws text in one of the game's bitmap fonts (1 bit per pixel, up to 16 pixels wide).</summary>
// orig: the graphics driver's string output drv+0x1223 (root thunk 047C:055A, driver slot 21; asm), its parts drv+0x1374 (font set-up),
//   drv+0x12A8 (right clip), drv+0x1254 (left clip), drv+0x130A (rows), drv+0x0FF3 (the glyph loop) and drv+0x0FDC (colour codes);
//   char_width drv+0x11A9 (root 047C:04E3) and font_height drv+0x11FA (root 047C:051B); reached through draw_text root 0265:006E.
//   checked against asm; tests: tools/visuals/compare.py
// C: per byte of the string: 0 ends it; a byte with the top bit set is a colour code (colour = byte & 0x7F, 0xFF back to the call's
//   colour) and takes no room; a byte outside the font's first..last range takes no room and draws nothing; any other byte advances
//   by its width (the width table, or the font's fixed width) plus the font's spacing. Font 0 starts at code 0, so its control codes
//   0x01..0x1F have the widths of its table; fonts 1 and 2 start at 0x1F.
// C: a glyph row is a 16-bit big-endian word whose top bit is the leftmost pixel; only set bits are drawn (the window mode bit 0,
//   an opaque background, is not used by the screens here); the last character also draws its spacing column (always clear).
// C: the colour left by the last colour code is written back into the window record (+0x0C), so the window's later texts keep it;
//   Draw returns it (DrawColor) for callers that reproduce this.
// C: draw_text doubles x and the window's x0/x1 when the screen page is the 640-wide mode; the game runs in 320x200, so that branch is not written.
// DORMANT: the hi-res (doubled x) branch of draw_text / draw_text_far.
public sealed class TextPainter(FntFont font)
{
    public FntFont Font => font;

    /// <summary>
    /// The TrueType font for the characters the bitmap fonts cannot draw (Chinese, and anything else above
    /// Latin-1), or null to keep the original's behaviour. A byte that names no glyph of either font takes no
    /// room, as it always did.
    /// </summary>
    public CjkFont? Cjk { get; init; }

    /// <summary>The font's line height (font_height): the glyph height plus the font's line spacing, or the Chinese font's when one is loaded.</summary>
    // orig: font_height drv+0x11FA: the header bytes at -4 (height) + -2 (line spacing) before the glyph data
    public int LineHeight => Cjk is null ? font.CharacterHeight + font.LineSpacing : Math.Max(font.CharacterHeight + font.LineSpacing, Cjk.LineHeight);

    /// <summary>The colour the last <see cref="Draw(Canvas, int, int, ReadOnlySpan{byte}, byte, int)"/> ended with (after its colour codes).</summary>
    public byte DrawColor { get; private set; }

    /// <summary>Horizontal advance of one character: its width plus the font's spacing (0 for a colour code or a code the font lacks).</summary>
    // orig: char_width drv+0x11A9: 0 for a byte with the top bit set or outside first..last; fixed width (header -5) when non-zero, else the width table; plus the spacing (header -3)
    public int Advance(byte c) =>
        c != 0 && c < 0x80 && font.Contains(c)
            ? (font.Unknown05 != 0 ? font.Unknown05 : font.GetCharacterWidth(c)) + font.CharacterSpacing
            : 0;

    /// <summary>Horizontal advance of one codepoint: the bitmap font's for the codes it has, the Chinese font's above Latin-1, 0 otherwise.</summary>
    public int Advance(int c) =>
        c < 0 ? 0 : c < 0x80 ? Advance((byte)c) : Cjk is { } face && face.Has(c) ? face.Advance(c) : 0;

    public int Measure(ReadOnlySpan<byte> text) => Measure(TextDecoder.Decode(text));

    public int Measure(ReadOnlySpan<int> text)
    {
        var width = 0;
        foreach (var c in text)
        {
            if (c == 0)
            {
                break;
            }
            width += Advance(c);
        }
        return width;
    }

    public int Measure(string text) => Measure(TextDecoder.Decode(Encode(text)));

    /// <summary>
    /// Draws text and returns the x after its last character. A byte from 0x80 switches the colour to its
    /// low seven bits and 0xFF back to <paramref name="color"/> (menus draw their hotkey letters this way:
    /// "\x8c U\xffse this formula"). <paramref name="right"/> is the window's last column: the character
    /// that crosses it is cut there and the rest is not drawn.
    /// </summary>
    // orig: drv+0x123C: set-up, right clip (drv+0x12A8: the string is cut after the character that reaches the last column, that character
    //   loses the excess), left clip, rows, then the glyph loop; the x returned is the drawn width (drv+0x136B)
    public int Draw(Canvas canvas, int x, int y, ReadOnlySpan<byte> text, byte color, int right = int.MaxValue) =>
        Draw(canvas, x, y, text, color, TextMode.Copy, right);

    /// <summary>Draws text combining each glyph pixel with the screen as <paramref name="mode"/> says (see <see cref="TextMode"/>).</summary>
    public int Draw(Canvas canvas, int x, int y, ReadOnlySpan<byte> text, byte color, TextMode mode, int right = int.MaxValue) =>
        Draw(canvas, x, y, TextDecoder.Decode(text), color, mode, right);

    /// <summary>Draws codepoints (see <see cref="TextDecoder"/>) combining each glyph pixel with the screen as <paramref name="mode"/> says.</summary>
    public int Draw(Canvas canvas, int x, int y, ReadOnlySpan<int> text, byte color, TextMode mode, int right = int.MaxValue)
    {
        var current = color;
        var room = right == int.MaxValue ? int.MaxValue : right - x + 1;
        if (room < 0)
        {
            DrawColor = color;
            return x;
        }
        foreach (var c in text)
        {
            if (c == 0)
            {
                break;
            }
            if (c < 0)
            {
                current = ColorCode(-c, color);
                continue;
            }
            if (c >= 0x80)
            {
                if (Cjk is not { } face || !face.Has(c))
                {
                    continue;
                }
                var chinese = face.Advance(c);
                var clipped = Math.Min(chinese, room);
                face.Draw(canvas, x, y, c, current, mode, x + clipped - 1);
                x += clipped;
                room -= chinese;
            }
            else
            {
                var advance = Advance(c);
                if (advance == 0)
                {
                    continue;
                }
                var visible = Math.Min(advance, room);
                DrawGlyph(canvas, x, y, (byte)c, current, visible, mode);
                x += visible;
                room -= advance;
            }
            if (room <= 0)
            {
                break;
            }
        }
        DrawColor = current;
        return x;
    }

    // orig: drv+0x0FDC: a colour code's low seven bits, 0xFF back to the call's colour, 0x80 a no-op
    private static byte ColorCode(int code, byte fallback) => code == 0xFF ? fallback : (byte)((code & 0x7F) == 0 ? 0xFF : code & 0x7F);

    public int Draw(Canvas canvas, int x, int y, string text, byte color, int right = int.MaxValue) => Draw(canvas, x, y, Encode(text), color, right);

    public int DrawCharacter(Canvas canvas, int x, int y, byte c, byte color)
    {
        var advance = Advance(c);
        if (advance != 0)
        {
            DrawGlyph(canvas, x, y, c, color, advance, TextMode.Copy);
        }
        return advance;
    }

    // orig: the foreground loop of drv+0x0FF3: per row the 16-bit word (bytes swapped), shifted left a bit per column, a set bit plots the colour
    //   with the instruction drv+0x0FFE / drv+0x0218 patched in at drv+0x10A9: mov, and, or or xor by bits 8..9 of the colour word
    private void DrawGlyph(Canvas canvas, int x, int y, byte c, byte color, int columns, TextMode mode)
    {
        for (var row = 0; row < font.CharacterHeight; row++)
        {
            var bytes = font.GetGlyphRow(c, row);
            var bits = bytes[0] << 8 | (bytes.Length > 1 ? bytes[1] : 0);
            for (var column = 0; column < columns && column < 16; column++, bits <<= 1)
            {
                if ((bits & 0x8000) != 0)
                {
                    var under = canvas[x + column, y + row];
                    if (mode == TextMode.Copy)
                    {
                        canvas[x + column, y + row] = color;
                    }
                    else
                    {
                        // a recolour of what is there: the pixel keeps its owner (Screen.Owners)
                        canvas.Repaint(x + column, y + row, mode switch
                        {
                            TextMode.And => (byte)(under & color),
                            TextMode.Or => (byte)(under | color),
                            _ => (byte)(under ^ color),
                        });
                    }
                }
            }
        }
    }

    /// <summary>Draws a text with colour codes (see <see cref="Draw(Canvas, int, int, ReadOnlySpan{byte}, byte, int)"/>).</summary>
    // orig: root 0265:0004 draft (bytes match) and drv+0x0FDC: the colour codes as the driver reads them (the map row of DrawCoded)
    public int DrawCoded(Canvas canvas, int x, int y, string text, byte color) => Draw(canvas, x, y, Encode(text), color);

    /// <summary>
    /// Encodes text as the game's bytes: Latin-1 when every character fits in a byte, so a byte with the top
    /// bit set stays a colour code (that is how the menus draw their hotkey letters, and how the screens
    /// build coloured option lines), and UTF-8 otherwise, which is how Chinese text reaches the canvas.
    /// </summary>
    public static byte[] Encode(string text)
    {
        foreach (var c in text)
        {
            if (c > 0xFF)
            {
                return System.Text.Encoding.UTF8.GetBytes(text);
            }
        }
        return System.Text.Encoding.Latin1.GetBytes(text);
    }
}

/// <summary>How a text pixel combines with the screen: bits 8..9 of the window's colour word (0x380 = colour 0x80 by xor).</summary>
// orig: drv+0x0218 (root thunk 047C:05E6, driver slot 41): the low byte is the colour, (high byte & 3) picks an opcode from the table
//   drv+0x0214 = {mov 0x88, and 0x20, or 0x08, xor 0x30}; the text loop drv+0x0FF3 calls it with the window's colour word. checked against asm
public enum TextMode
{
    Copy = 0,
    And = 1,
    Or = 2,
    Xor = 3,
}
