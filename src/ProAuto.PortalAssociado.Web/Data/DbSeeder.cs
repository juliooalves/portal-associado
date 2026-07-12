using Microsoft.EntityFrameworkCore;
using ProAuto.PortalAssociado.Web.Domain;

namespace ProAuto.PortalAssociado.Web.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext)
    {
        if (await dbContext.Associados.AnyAsync())
        {
            return;
        }

        dbContext.Associados.AddRange(
            new Associado(
                Guid.Parse("6f1cd2a4-9b7e-4c1a-8f3d-1a2b3c4d5e6f"),
                "Maria Oliveira Santos",
                new Cpf("11144477735"),
                new Placa("ABC1234"),
                "(31) 99876-5432",
                new Endereco(
                    "Rua das Acácias",
                    "120",
                    "Apto 302",
                    "Savassi",
                    "Belo Horizonte",
                    "MG",
                    "30140-090")),
            new Associado(
                Guid.Parse("2b8e5f7c-3d4a-4e9b-a1c6-7d8e9f0a1b2c"),
                "João Pedro Ferreira",
                new Cpf("52998224725"),
                new Placa("BRA2E19"),
                "(11) 98765-4321",
                new Endereco(
                    "Avenida Paulista",
                    "1578",
                    null,
                    "Bela Vista",
                    "São Paulo",
                    "SP",
                    "01310-200")),
            new Associado(
                Guid.Parse("9c4d7e1f-5a6b-4c8d-b2e3-4f5a6b7c8d9e"),
                "Ana Carolina Lima",
                new Cpf("39053344705"),
                new Placa("XYZ9A87"),
                "(21) 97654-3210",
                new Endereco(
                    "Rua Barata Ribeiro",
                    "45",
                    "Bloco B",
                    "Copacabana",
                    "Rio de Janeiro",
                    "RJ",
                    "22040-000")));

        await dbContext.SaveChangesAsync();
    }
}
