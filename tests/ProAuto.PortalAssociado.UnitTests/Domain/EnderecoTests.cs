using ProAuto.PortalAssociado.Web.Domain;

namespace ProAuto.PortalAssociado.UnitTests.Domain;

public class EnderecoTests
{
    private static Endereco Valid(
        string logradouro = "Rua das Flores",
        string numero = "100",
        string? complemento = "Apto 12",
        string bairro = "Centro",
        string cidade = "São Paulo",
        string uf = "SP",
        string cep = "01310-100") =>
        new(logradouro, numero, complemento, bairro, cidade, uf, cep);

    [Fact]
    public void Constructor_WithValidData_NormalizesCepAndUf()
    {
        var endereco = Valid(uf: "sp", cep: "01310-100");

        Assert.Equal("SP", endereco.Uf);
        Assert.Equal("01310100", endereco.Cep);
    }

    [Fact]
    public void Constructor_WithBlankComplemento_StoresNull()
    {
        var endereco = Valid(complemento: "  ");

        Assert.Null(endereco.Complemento);
    }

    [Theory]
    [InlineData("")]
    [InlineData("S")]
    [InlineData("SPX")]
    [InlineData("S1")]
    [InlineData("ZZ")]
    [InlineData("XX")]
    public void Constructor_WithInvalidUf_Throws(string uf)
    {
        Assert.Throws<ArgumentException>(() => Valid(uf: uf));
    }

    [Theory]
    [InlineData("1234567")]
    [InlineData("123456789")]
    [InlineData("")]
    [InlineData("０１３１０１００")]
    public void Constructor_WithInvalidCep_Throws(string cep)
    {
        Assert.Throws<ArgumentException>(() => Valid(cep: cep));
    }

    [Theory]
    [InlineData("", "100", "Centro", "São Paulo")]
    [InlineData("Rua A", "", "Centro", "São Paulo")]
    [InlineData("Rua A", "100", "", "São Paulo")]
    [InlineData("Rua A", "100", "Centro", "")]
    public void Constructor_WithMissingRequiredField_Throws(
        string logradouro, string numero, string bairro, string cidade)
    {
        Assert.Throws<ArgumentException>(
            () => Valid(logradouro: logradouro, numero: numero, bairro: bairro, cidade: cidade));
    }
}
