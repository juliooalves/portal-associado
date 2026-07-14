namespace ProAuto.PortalAssociado.Web.Domain;

public sealed class Endereco
{
    public string Logradouro { get; private set; }
    public string Numero { get; private set; }
    public string? Complemento { get; private set; }
    public string Bairro { get; private set; }
    public string Cidade { get; private set; }
    public string Uf { get; private set; }
    public string Cep { get; private set; }

    public Endereco(
        string logradouro,
        string numero,
        string? complemento,
        string bairro,
        string cidade,
        string uf,
        string cep)
    {
        Logradouro = ValidateRequired(logradouro, nameof(logradouro));
        Numero = ValidateRequired(numero, nameof(numero));
        Complemento = string.IsNullOrWhiteSpace(complemento) ? null : complemento.Trim();
        Bairro = ValidateRequired(bairro, nameof(bairro));
        Cidade = ValidateRequired(cidade, nameof(cidade));
        Uf = ValidateUf(uf);
        Cep = ValidateCep(cep);
    }

#pragma warning disable CS8618
    private Endereco()
    {
    }
#pragma warning restore CS8618

    private static string ValidateRequired(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Campo obrigatório.", paramName);
        }

        return value.Trim();
    }

    private static readonly HashSet<string> UfsValidas = new(StringComparer.Ordinal)
    {
        "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO", "MA", "MT", "MS", "MG",
        "PA", "PB", "PR", "PE", "PI", "RJ", "RN", "RS", "RO", "RR", "SC", "SE", "SP", "TO"
    };

    private static string ValidateUf(string uf)
    {
        var normalized = (uf ?? string.Empty).Trim().ToUpperInvariant();

        if (!UfsValidas.Contains(normalized))
        {
            throw new ArgumentException("UF inválida.", nameof(uf));
        }

        return normalized;
    }

    private static string ValidateCep(string cep)
    {
        var digits = new string((cep ?? string.Empty).Where(char.IsAsciiDigit).ToArray());

        if (digits.Length != 8)
        {
            throw new ArgumentException("CEP deve conter 8 dígitos.", nameof(cep));
        }

        return digits;
    }
}
