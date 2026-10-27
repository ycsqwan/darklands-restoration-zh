using System.Runtime.InteropServices;
using StbTrueTypeSharp;

namespace Darklands.Presentation.Graphics;

/// <summary>A glyph of <see cref="CjkFont"/>, rasterised once at the font's size.</summary>
public sealed record CjkGlyph(int Advance, int OffsetX, int OffsetY, int Width, int Height, byte[] Coverage);

/// <summary>
/// A TrueType font rasterised onto the game's indexed canvas, for the characters the bundled bitmap fonts
/// cannot draw (Chinese, and anything else above Latin-1).
/// </summary>
// The canvas is indexed and has no alpha blending, so a glyph pixel is either the text colour or nothing and
// the coverage is thresholded at half. That is also what keeps a bitmap-derived face (Fusion Pixel,
// whose outlines are already pixel-aligned) crisp instead of grey.
public sealed unsafe class CjkFont : IDisposable
{
    private readonly GCHandle _pin;
    private readonly StbTrueType.stbtt_fontinfo _info;
    private readonly float _scale;
    private readonly int _ascent;
    private readonly int _baseline;
    private readonly Dictionary<int, CjkGlyph> _glyphs = [];

    private CjkFont(byte[] file, int em, int? lineHeight, int? baseline = null)
    {
        Em = em;
        _pin = GCHandle.Alloc(file, GCHandleType.Pinned);
        var data = (byte*)_pin.AddrOfPinnedObject();
        _info = new StbTrueType.stbtt_fontinfo();
        if (StbTrueType.stbtt_InitFont(_info, data, StbTrueType.stbtt_GetFontOffsetForIndex(data, 0)) == 0)
        {
            _pin.Free();
            throw new InvalidDataException("not a TrueType font");
        }
        // The em square, not the ascent..descent span, is the grid a bitmap-derived face is aligned to.
        // Fusion Pixel 10 px has unitsPerEm == ascent-descent == 1000, so both give 10 px, but a face whose
        // ascent-descent differs from its unitsPerEm (Cubic 11) only lines up on the em square.
        _scale = StbTrueType.stbtt_ScaleForMappingEmToPixels(_info, em);
        int ascent, descent, gap;
        StbTrueType.stbtt_GetFontVMetrics(_info, &ascent, &descent, &gap);
        _ascent = (int)Math.Round(ascent * _scale);
        // The bitmap fonts put their baseline at the bottom of the glyph box (6 px for font 1, 7 for font 3),
        // so a face drawn beside them takes its baseline from there rather than from its own ascent.
        _baseline = baseline ?? _ascent;
        LineHeight = lineHeight ?? (int)Math.Round((ascent - descent + gap) * _scale);
    }

    /// <summary>The em square in pixels the font is rasterised at.</summary>
    public int Em { get; }

    /// <summary>Pixels from the top of a line to the baseline (the font's ascent, or the one it was loaded with).</summary>
    public int Ascent => _baseline;

    /// <summary>Pixels from the baseline to the bottom of a line (positive).</summary>
    public int Descent => (int)Math.Round(-DescentOf(_info, _scale));

    /// <summary>The distance between two baselines.</summary>
    public int LineHeight { get; }

    public bool Has(int codepoint) => StbTrueType.stbtt_FindGlyphIndex(_info, codepoint) != 0;

    /// <summary>Horizontal advance of one codepoint.</summary>
    public int Advance(int codepoint) => Of(codepoint).Advance;

    public CjkGlyph Of(int codepoint)
    {
        if (_glyphs.TryGetValue(codepoint, out var glyph))
        {
            return glyph;
        }
        int advance, bearing, width, height, offsetX, offsetY;
        StbTrueType.stbtt_GetCodepointHMetrics(_info, codepoint, &advance, &bearing);
        var bitmap = StbTrueType.stbtt_GetCodepointBitmap(_info, _scale, _scale, codepoint, &width, &height, &offsetX, &offsetY);
        var coverage = new byte[Math.Max(0, width * height)];
        if (bitmap != null && coverage.Length != 0)
        {
            new ReadOnlySpan<byte>(bitmap, coverage.Length).CopyTo(coverage);
            StbTrueType.stbtt_FreeBitmap(bitmap, null);
        }
        return _glyphs[codepoint] = new CjkGlyph((int)Math.Round(advance * _scale), offsetX, offsetY, width, height, coverage);
    }

    /// <summary>
    /// Plots one codepoint with the top of its line at (<paramref name="x"/>, <paramref name="y"/>).
    /// <paramref name="right"/> is the last column to draw, as <see cref="TextPainter.Draw(Canvas, int, int, ReadOnlySpan{byte}, byte, TextMode, int)"/> takes it.
    /// </summary>
    public void Draw(Canvas canvas, int x, int y, int codepoint, byte color, TextMode mode, int right)
    {
        var glyph = Of(codepoint);
        var visible = Math.Min(glyph.Width, right == int.MaxValue ? int.MaxValue : right - x + 1);
        if (visible <= 0)
        {
            return;
        }
        var top = y + _baseline + glyph.OffsetY;
        for (var row = 0; row < glyph.Height; row++)
        {
            for (var column = 0; column < visible; column++)
            {
                if (glyph.Coverage[row * glyph.Width + column] < 128)
                {
                    continue;
                }
                var px = x + glyph.OffsetX + column;
                var py = top + row;
                if (mode == TextMode.Copy)
                {
                    canvas.Plot(px, py, color, 0);
                }
                else
                {
                    // A recolour of what is there, as a bitmap glyph's window mode does: the pixel keeps its owner.
                    canvas.Repaint(px, py, mode switch
                    {
                        TextMode.And => (byte)(canvas[px, py] & color),
                        TextMode.Or => (byte)(canvas[px, py] | color),
                        _ => (byte)(canvas[px, py] ^ color),
                    });
                }
            }
        }
    }

    /// <summary>
    /// Loads <paramref name="path"/> at <paramref name="em"/> pixels per em square, or null when there is
    /// nothing to load. <paramref name="lineHeight"/> overrides the distance between two baselines when the
    /// font's own is too loose for the layout (12 px Chinese wants 13 px lines, not 15).
    /// </summary>
    public static CjkFont? Load(string? path, int em, int? lineHeight = null, Action<string>? report = null, int? baseline = null)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }
        try
        {
            return new CjkFont(File.ReadAllBytes(path), em, lineHeight, baseline);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException or InvalidDataException or IndexOutOfRangeException or ArgumentException)
        {
            report?.Invoke($"Chinese font {path} cannot be used ({e.Message}).");
            return null;
        }
    }

    /// <summary>Loads an embedded font resource (<paramref name="resource"/>); <paramref name="path"/>, when given, wins.</summary>
    public static CjkFont? LoadEmbedded(string? path, int em, int? lineHeight, Action<string>? report, int? baseline = null,
        string resource = "Darklands.Presentation.Resources.ChineseFont.ttf")
    {
        var direct = Load(path, em, lineHeight, report, baseline);
        if (direct is not null)
        {
            return direct;
        }
        if (path is not null)
        {
            return null;
        }
        try
        {
            using var stream = typeof(TextPainter).Assembly.GetManifestResourceStream(resource);
            if (stream is null)
            {
                return null;
            }
            using var memory = new MemoryStream();
            stream.CopyTo(memory);
            return new CjkFont(memory.ToArray(), em, lineHeight, baseline);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException or InvalidDataException or IndexOutOfRangeException or ArgumentException or InvalidOperationException)
        {
            report?.Invoke($"The built-in Chinese font cannot be used ({e.Message}).");
            return null;
        }
    }

    public void Dispose()
    {
        if (_pin.IsAllocated)
        {
            _pin.Free();
        }
    }

    private static float DescentOf(StbTrueType.stbtt_fontinfo info, float scale)
    {
        int _, descent, __;
        StbTrueType.stbtt_GetFontVMetrics(info, &_, &descent, &__);
        return descent * scale;
    }
}
