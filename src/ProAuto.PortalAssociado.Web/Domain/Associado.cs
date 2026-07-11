namespace ProAuto.PortalAssociado.Web.Domain;

public sealed class Associado
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public Cpf Cpf { get; private set; }
    public Placa Placa { get; private set; }
    public string Telefone { get; private set; }
    public Endereco Endereco { get; private set; }

    public Associado(Guid id, string nome, Cpf cpf, Placa placa, string telefone, Endereco endereco)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("Nome é obrigatório.", nameof(nome));
        }

        if (string.IsNullOrWhiteSpace(telefone))
        {
            throw new ArgumentException("Telefone é obrigatório.", nameof(telefone));
        }

        Id = id;
        Nome = nome.Trim();
        Cpf = cpf ?? throw new ArgumentNullException(nameof(cpf));
        Placa = placa ?? throw new ArgumentNullException(nameof(placa));
        Telefone = telefone.Trim();
        Endereco = endereco ?? throw new ArgumentNullException(nameof(endereco));
    }

#pragma warning disable CS8618
    private Associado()
    {
    }
#pragma warning restore CS8618

    public void AtualizarEndereco(Endereco novoEndereco)
    {
        Endereco = novoEndereco ?? throw new ArgumentNullException(nameof(novoEndereco));
    }
}
