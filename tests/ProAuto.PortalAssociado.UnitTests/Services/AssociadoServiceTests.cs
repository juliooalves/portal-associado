using NSubstitute;
using NSubstitute.ReturnsExtensions;
using ProAuto.PortalAssociado.Web.Domain;
using ProAuto.PortalAssociado.Web.Repositories;
using ProAuto.PortalAssociado.Web.Services;

namespace ProAuto.PortalAssociado.UnitTests.Services;

public class AssociadoServiceTests
{
    private readonly IAssociadoRepository _repository = Substitute.For<IAssociadoRepository>();
    private readonly AssociadoService _service;

    public AssociadoServiceTests()
    {
        _service = new AssociadoService(_repository);
    }

    private static Associado CriarAssociado(
        Guid? id = null,
        string cpf = "11144477735",
        string placa = "ABC1234")
    {
        return new Associado(
            id ?? Guid.NewGuid(),
            "Maria Oliveira Santos",
            new Cpf(cpf),
            new Placa(placa),
            "(31) 99876-5432",
            CriarEndereco());
    }

    private static Endereco CriarEndereco(string cidade = "Belo Horizonte") =>
        new("Rua das Acácias", "120", "Apto 302", "Savassi", cidade, "MG", "30140090");

    [Fact]
    public async Task AutenticarAsync_WithValidCredentials_ReturnsAssociado()
    {
        var associado = CriarAssociado();
        _repository.GetByCpfAsync(associado.Cpf, Arg.Any<CancellationToken>()).Returns(associado);

        var result = await _service.AutenticarAsync("111.444.777-35", "abc-1234");

        Assert.Same(associado, result);
    }

    [Fact]
    public async Task AutenticarAsync_WithWrongPlaca_ReturnsNull()
    {
        var associado = CriarAssociado(placa: "ABC1234");
        _repository.GetByCpfAsync(associado.Cpf, Arg.Any<CancellationToken>()).Returns(associado);

        var result = await _service.AutenticarAsync("11144477735", "XYZ9A87");

        Assert.Null(result);
    }

    [Fact]
    public async Task AutenticarAsync_WithUnknownCpf_ReturnsNull()
    {
        _repository.GetByCpfAsync(Arg.Any<Cpf>(), Arg.Any<CancellationToken>()).ReturnsNull();

        var result = await _service.AutenticarAsync("11144477735", "ABC1234");

        Assert.Null(result);
    }

    [Theory]
    [InlineData(null, "ABC1234")]
    [InlineData("", "ABC1234")]
    [InlineData("12345678900", "ABC1234")]
    [InlineData("11144477735", null)]
    [InlineData("11144477735", "")]
    [InlineData("11144477735", "AB12345")]
    public async Task AutenticarAsync_WithMalformedCredentials_ReturnsNullWithoutQuerying(string? cpf, string? placa)
    {
        var result = await _service.AutenticarAsync(cpf, placa);

        Assert.Null(result);
        await _repository.DidNotReceive().GetByCpfAsync(Arg.Any<Cpf>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ObterPorIdAsync_WithExistingId_ReturnsAssociado()
    {
        var associado = CriarAssociado();
        _repository.GetByIdAsync(associado.Id, Arg.Any<CancellationToken>()).Returns(associado);

        var result = await _service.ObterPorIdAsync(associado.Id);

        Assert.Same(associado, result);
    }

    [Fact]
    public async Task ObterPorIdAsync_WithUnknownId_ReturnsNull()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).ReturnsNull();

        var result = await _service.ObterPorIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task AtualizarEnderecoAsync_WithExistingAssociado_UpdatesAndSaves()
    {
        var associado = CriarAssociado();
        _repository.GetByIdAsync(associado.Id, Arg.Any<CancellationToken>()).Returns(associado);
        var novoEndereco = CriarEndereco(cidade: "São Paulo");

        var result = await _service.AtualizarEnderecoAsync(associado.Id, novoEndereco);

        Assert.Same(associado, result);
        Assert.Same(novoEndereco, associado.Endereco);
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AtualizarEnderecoAsync_WithUnknownAssociado_ReturnsNullWithoutSaving()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).ReturnsNull();

        var result = await _service.AtualizarEnderecoAsync(Guid.NewGuid(), CriarEndereco());

        Assert.Null(result);
        await _repository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
