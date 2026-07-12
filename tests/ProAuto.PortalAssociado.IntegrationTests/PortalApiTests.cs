using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using ProAuto.PortalAssociado.Web.Contracts;

namespace ProAuto.PortalAssociado.IntegrationTests;

public sealed class PortalApiTests : IClassFixture<PortalWebApplicationFactory>
{
    private readonly PortalWebApplicationFactory _factory;

    public PortalApiTests(PortalWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private static async Task<HttpResponseMessage> LoginAsync(HttpClient client, string cpf, string placa) =>
        await client.PostAsJsonAsync("/api/auth/login", new { cpf, placa });

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsAssociado()
    {
        var client = _factory.CreateHttpsClient();

        var response = await LoginAsync(client, "111.444.777-35", "ABC1234");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var associado = await response.Content.ReadFromJsonAsync<AssociadoResponse>();
        Assert.NotNull(associado);
        Assert.Equal("Maria Oliveira Santos", associado.Nome);
        Assert.Equal("111.444.777-35", associado.Cpf);
    }

    [Fact]
    public async Task Login_WithWrongPlaca_ReturnsGenericUnauthorized()
    {
        var client = _factory.CreateHttpsClient();

        var response = await LoginAsync(client, "111.444.777-35", "ZZZ9999");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal("CPF ou placa inválidos.", problem.Title);
    }

    [Fact]
    public async Task Login_WithMalformedCpf_ReturnsSameGenericUnauthorized()
    {
        var client = _factory.CreateHttpsClient();

        var response = await LoginAsync(client, "123", "ABC1234");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal("CPF ou placa inválidos.", problem.Title);
    }

    [Fact]
    public async Task Me_WithoutAuthentication_ReturnsUnauthorized()
    {
        var client = _factory.CreateHttpsClient();

        var response = await client.GetAsync("/api/associados/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AtualizarEndereco_WithoutAuthentication_ReturnsUnauthorized()
    {
        var client = _factory.CreateHttpsClient();

        var response = await client.PutAsJsonAsync("/api/associados/me/endereco", new
        {
            logradouro = "Rua Nova",
            numero = "1",
            bairro = "Centro",
            cidade = "Curitiba",
            uf = "PR",
            cep = "80010-000"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task MeusDados_WithoutAuthentication_RedirectsToLogin()
    {
        var client = _factory.CreateHttpsClient(allowAutoRedirect: false);

        var response = await client.GetAsync("/meus-dados");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("https://localhost/login", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task FullFlow_LoginGetMeUpdateEndereco_PersistsNewEndereco()
    {
        var client = _factory.CreateHttpsClient();
        await LoginAsync(client, "11144477735", "abc1234");

        var me = await client.GetFromJsonAsync<AssociadoResponse>("/api/associados/me");
        Assert.NotNull(me);
        Assert.Equal("Maria Oliveira Santos", me.Nome);

        var putResponse = await client.PutAsJsonAsync("/api/associados/me/endereco", new
        {
            logradouro = "Rua Atualizada",
            numero = "999",
            complemento = "Casa 2",
            bairro = "Funcionários",
            cidade = "Belo Horizonte",
            uf = "MG",
            cep = "30130-100"
        });

        Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);
        var updated = await putResponse.Content.ReadFromJsonAsync<AssociadoResponse>();
        Assert.NotNull(updated);
        Assert.Equal("Rua Atualizada", updated.Endereco.Logradouro);

        var meAgain = await client.GetFromJsonAsync<AssociadoResponse>("/api/associados/me");
        Assert.NotNull(meAgain);
        Assert.Equal("Rua Atualizada", meAgain.Endereco.Logradouro);
        Assert.Equal("999", meAgain.Endereco.Numero);
        Assert.Equal("Maria Oliveira Santos", meAgain.Nome);
    }

    [Fact]
    public async Task AtualizarEndereco_WithExtraFieldsInPayload_IgnoresThem()
    {
        var client = _factory.CreateHttpsClient();
        await LoginAsync(client, "390.533.447-05", "XYZ9A87");

        var response = await client.PutAsJsonAsync("/api/associados/me/endereco", new
        {
            nome = "Nome Hackeado",
            cpf = "000.000.000-00",
            placa = "HAK1234",
            logradouro = "Rua Legítima",
            numero = "10",
            bairro = "Copacabana",
            cidade = "Rio de Janeiro",
            uf = "RJ",
            cep = "22040-000"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var associado = await response.Content.ReadFromJsonAsync<AssociadoResponse>();
        Assert.NotNull(associado);
        Assert.Equal("Ana Carolina Lima", associado.Nome);
        Assert.Equal("390.533.447-05", associado.Cpf);
        Assert.Equal("XYZ9A87", associado.Placa);
        Assert.Equal("Rua Legítima", associado.Endereco.Logradouro);
    }

    [Fact]
    public async Task AtualizarEndereco_WithInvalidUf_ReturnsBadRequest()
    {
        var client = _factory.CreateHttpsClient();
        await LoginAsync(client, "390.533.447-05", "XYZ9A87");

        var response = await client.PutAsJsonAsync("/api/associados/me/endereco", new
        {
            logradouro = "Rua Qualquer",
            numero = "10",
            bairro = "Centro",
            cidade = "Vitória",
            uf = "XYZ",
            cep = "29010-000"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AtualizarEndereco_OnlyAffectsAuthenticatedAssociado()
    {
        var mariaClient = _factory.CreateHttpsClient();
        await LoginAsync(mariaClient, "111.444.777-35", "ABC1234");
        await mariaClient.PutAsJsonAsync("/api/associados/me/endereco", new
        {
            logradouro = "Rua Alterada Pela Maria",
            numero = "77",
            bairro = "Savassi",
            cidade = "Belo Horizonte",
            uf = "MG",
            cep = "30140-090"
        });

        var joaoClient = _factory.CreateHttpsClient();
        await LoginAsync(joaoClient, "529.982.247-25", "BRA2E19");
        var joao = await joaoClient.GetFromJsonAsync<AssociadoResponse>("/api/associados/me");

        Assert.NotNull(joao);
        Assert.Equal("João Pedro Ferreira", joao.Nome);
        Assert.Equal("Avenida Paulista", joao.Endereco.Logradouro);
        Assert.NotEqual("Rua Alterada Pela Maria", joao.Endereco.Logradouro);
    }
}
