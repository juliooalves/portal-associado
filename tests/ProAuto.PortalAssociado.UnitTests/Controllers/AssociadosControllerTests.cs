using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using ProAuto.PortalAssociado.Web.Contracts;
using ProAuto.PortalAssociado.Web.Controllers.Api;
using ProAuto.PortalAssociado.Web.Domain;
using ProAuto.PortalAssociado.Web.Services;

namespace ProAuto.PortalAssociado.UnitTests.Controllers;

public class AssociadosControllerTests
{
    private static readonly Guid AssociadoId = Guid.NewGuid();

    private readonly IAssociadoService _service = Substitute.For<IAssociadoService>();
    private readonly AssociadosController _controller;

    public AssociadosControllerTests()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, AssociadoId.ToString()) },
            authenticationType: "Test"));

        _controller = new AssociadosController(_service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            }
        };
    }

    private static Associado CriarAssociado() =>
        new(
            AssociadoId,
            "Maria Oliveira Santos",
            new Cpf("11144477735"),
            new Placa("ABC1234"),
            "(31) 99876-5432",
            new Endereco("Rua das Acácias", "120", null, "Savassi", "Belo Horizonte", "MG", "30140090"));

    private static AtualizarEnderecoRequest CriarRequest(string logradouro = "Avenida Paulista") =>
        new()
        {
            Logradouro = logradouro,
            Numero = "1578",
            Bairro = "Bela Vista",
            Cidade = "São Paulo",
            Uf = "SP",
            Cep = "01310-200"
        };

    [Fact]
    public async Task Me_WithExistingAssociado_ReturnsData()
    {
        _service.ObterPorIdAsync(AssociadoId, Arg.Any<CancellationToken>()).Returns(CriarAssociado());

        var result = await _controller.Me(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<AssociadoResponse>(ok.Value);
        Assert.Equal("Maria Oliveira Santos", response.Nome);
        Assert.Equal("ABC1234", response.Placa);
    }

    [Fact]
    public async Task Me_WithUnknownAssociado_ReturnsNotFound()
    {
        _service.ObterPorIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).ReturnsNull();

        var result = await _controller.Me(CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task AtualizarEndereco_WithValidRequest_UpdatesUsingIdFromClaim()
    {
        var associado = CriarAssociado();
        Endereco? enderecoRecebido = null;
        _service.AtualizarEnderecoAsync(AssociadoId, Arg.Do<Endereco>(e => enderecoRecebido = e), Arg.Any<CancellationToken>())
            .Returns(associado);

        var result = await _controller.AtualizarEndereco(CriarRequest(), CancellationToken.None);

        Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(enderecoRecebido);
        Assert.Equal("Avenida Paulista", enderecoRecebido!.Logradouro);
    }

    [Fact]
    public async Task AtualizarEndereco_WithUnknownAssociado_ReturnsNotFound()
    {
        _service.AtualizarEnderecoAsync(Arg.Any<Guid>(), Arg.Any<Endereco>(), Arg.Any<CancellationToken>())
            .ReturnsNull();

        var result = await _controller.AtualizarEndereco(CriarRequest(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task AtualizarEndereco_WithEnderecoRejectedByDomain_ReturnsValidationProblem()
    {
        var result = await _controller.AtualizarEndereco(CriarRequest(logradouro: "   "), CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var problem = Assert.IsType<ValidationProblemDetails>(badRequest.Value);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
        await _service.DidNotReceive().AtualizarEnderecoAsync(
            Arg.Any<Guid>(), Arg.Any<Endereco>(), Arg.Any<CancellationToken>());
    }
}
