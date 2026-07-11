using ProAuto.PortalAssociado.Web.Domain;

namespace ProAuto.PortalAssociado.UnitTests.Domain;

public class PlacaTests
{
    [Theory]
    [InlineData("ABC1234", "ABC1234")] // old format
    [InlineData("abc1234", "ABC1234")] // normalizes casing
    [InlineData("ABC-1234", "ABC1234")] // strips hyphen
    [InlineData("ABC1D23", "ABC1D23")] // Mercosul
    [InlineData("abc1d23", "ABC1D23")]
    public void Constructor_WithValidPlaca_Normalizes(string input, string expected)
    {
        var placa = new Placa(input);

        Assert.Equal(expected, placa.Value);
    }

    [Theory]
    [InlineData("AB1234")] // too short
    [InlineData("ABCD1234")] // too many letters
    [InlineData("1234ABC")] // inverted
    [InlineData("ABC12D3")] // letter in wrong Mercosul position
    [InlineData("")]
    [InlineData("ABC 12 34X")]
    public void Constructor_WithInvalidPlaca_Throws(string input)
    {
        Assert.Throws<ArgumentException>(() => new Placa(input));
    }

    [Fact]
    public void TryParse_WithInvalidPlaca_ReturnsFalse()
    {
        var result = Placa.TryParse("1234", out var placa);

        Assert.False(result);
        Assert.Null(placa);
    }

    [Fact]
    public void Equality_SameValueDifferentFormatting_AreEqual()
    {
        Assert.Equal(new Placa("abc-1234"), new Placa("ABC1234"));
    }
}
