using Darklands.Presentation.Graphics;

namespace Darklands.Rendering.Tests;

/// <summary>
/// A hotkey prefix is colour codes, and colour codes are single bytes: 0x8C switches to colour 0x0C, 0xFF
/// restores. <see cref="TextPainter.Encode"/> keeps one byte per character only while every code point fits in
/// a byte; a single Chinese character switches the whole string to UTF-8, which turns those two bytes into
/// U+008C and U+00FF. The highlight is lost and glyphs are drawn where the codes were, so a translated label
/// has to be drawn as its own call rather than appended to the hotkey. ScreenDrawing.Hotkey builds exactly the
/// prefix asserted here, but it is internal to the presentation assembly.
/// </summary>
public sealed class HotkeyTextTests
{
    private const string HotkeyE = "\u008C E\u00FF";

    /// <summary>The hotkey prefix on its own stays one byte per character, so its colour codes survive.</summary>
    [Fact]
    public void HotkeyPrefixEncodesToOneBytePerCharacter()
    {
        Assert.Equal(new byte[] { 0x8C, 0x20, 0x45, 0xFF }, TextPainter.Encode(HotkeyE));
    }

    /// <summary>Appending a translated label to it breaks the colour codes: 0x8C becomes the UTF-8 pair C2 8C.</summary>
    [Fact]
    public void AppendingTranslatedTextBreaksTheColourCodes()
    {
        var bytes = TextPainter.Encode(HotkeyE + "赚点小钱");

        Assert.Equal(new byte[] { 0xC2, 0x8C }, bytes[..2]);
        Assert.DoesNotContain((byte)0x8C, bytes[2..]);
    }

    /// <summary>
    /// The prefix decodes back to two colour changes around the Latin key, which is what the painter needs.
    /// A colour code comes back as the negated byte value.
    /// </summary>
    [Fact]
    public void HotkeyPrefixDecodesToColourChanges()
    {
        var decoded = TextDecoder.Decode(TextPainter.Encode(HotkeyE)).ToArray();

        Assert.Equal(new[] { -0x8C, 0x20, 0x45, -0xFF }, decoded);
    }
}
