using ProAuto.PortalAssociado.Web.Domain;

namespace ProAuto.PortalAssociado.UnitTests.Domain;

public class CpfTests
{
    [Theory]
    [InlineData("529.982.247-25")]
    [InlineData("52998224725")]
    [InlineData("111.444.777-35")]
    public void Constructor_WithValidCpf_StoresDigitsOnly(string input)
    {
        var cpf = new Cpf(input);

        Assert.Equal(11, cpf.Value.Length);
        Assert.All(cpf.Value, c => Assert.True(char.IsDigit(c)));
    }

    [Theory]
    [InlineData("529.982.247-26")] // wrong second check digit
    [InlineData("529.982.247-15")] // wrong first check digit
    [InlineData("111.111.111-11")] // repeated digits
    [InlineData("000.000.000-00")]
    [InlineData("123")]
    [InlineData("")]
    [InlineData("abc.def.ghi-jk")]
    public void Constructor_WithInvalidCpf_Throws(string input)
    {
        Assert.Throws<ArgumentException>(() => new Cpf(input));
    }

    [Fact]
    public void Formatted_ReturnsMaskedCpf()
    {
        var cpf = new Cpf("52998224725");

        Assert.Equal("529.982.247-25", cpf.Formatted);
    }

    [Fact]
    public void TryParse_WithInvalidCpf_ReturnsFalse()
    {
        var result = Cpf.TryParse("111.111.111-11", out var cpf);

        Assert.False(result);
        Assert.Null(cpf);
    }

    [Fact]
    public void Equality_SameDigitsDifferentFormatting_AreEqual()
    {
        Assert.Equal(new Cpf("529.982.247-25"), new Cpf("52998224725"));
    }
}
