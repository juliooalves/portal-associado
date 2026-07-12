using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using ProAuto.PortalAssociado.Web.Contracts;
using ProAuto.PortalAssociado.Web.Controllers.Api;
using ProAuto.PortalAssociado.Web.Domain;
using ProAuto.PortalAssociado.Web.Services;

namespace ProAuto.PortalAssociado.UnitTests.Controllers;

public class AuthControllerTests
{
    private readonly IAssociadoService _service = Substitute.For<IAssociadoService>();
    private readonly IAuthenticationService _authenticationService = Substitute.For<IAuthenticationService>();
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        var services = new ServiceCollection();
        services.AddSingleton(_authenticationService);

        _controller = new AuthController(_service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = services.BuildServiceProvider()
                }
            }
        };
    }

    private static Associado CriarAssociado() =>
        new(
            Guid.NewGuid(),
            "Maria Oliveira Santos",
            new Cpf("11144477735"),
            new Placa("ABC1234"),
            "(31) 99876-5432",
            new Endereco("Rua das Acácias", "120", null, "Savassi", "Belo Horizonte", "MG", "30140090"));

    [Fact]
    public async Task Login_WithValidCredentials_SignsInAndReturnsAssociado()
    {
        var associado = CriarAssociado();
        _service.AutenticarAsync("11144477735", "ABC1234", Arg.Any<CancellationToken>()).Returns(associado);

        var result = await _controller.Login(
            new LoginRequest { Cpf = "11144477735", Placa = "ABC1234" },
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<AssociadoResponse>(ok.Value);
        Assert.Equal("111.444.777-35", response.Cpf);
        await _authenticationService.Received(1).SignInAsync(
            Arg.Any<HttpContext>(),
            Arg.Any<string>(),
            Arg.Is<ClaimsPrincipal>(p =>
                p.FindFirst(ClaimTypes.NameIdentifier)!.Value == associado.Id.ToString()),
            Arg.Any<AuthenticationProperties>());
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsGenericUnauthorizedWithoutSigningIn()
    {
        _service.AutenticarAsync(Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .ReturnsNull();

        var result = await _controller.Login(
            new LoginRequest { Cpf = "11144477735", Placa = "XYZ9A87" },
            CancellationToken.None);

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        var problem = Assert.IsType<ProblemDetails>(unauthorized.Value);
        Assert.Equal("CPF ou placa inválidos.", problem.Title);
        await _authenticationService.DidNotReceive().SignInAsync(
            Arg.Any<HttpContext>(),
            Arg.Any<string>(),
            Arg.Any<ClaimsPrincipal>(),
            Arg.Any<AuthenticationProperties>());
    }

    [Fact]
    public async Task Logout_SignsOutAndReturnsNoContent()
    {
        var result = await _controller.Logout();

        Assert.IsType<NoContentResult>(result);
        await _authenticationService.Received(1).SignOutAsync(
            Arg.Any<HttpContext>(),
            Arg.Any<string>(),
            Arg.Any<AuthenticationProperties>());
    }
}
