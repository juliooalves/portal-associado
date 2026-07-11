using System.Text.RegularExpressions;

namespace ProAuto.PortalAssociado.Web.Domain;

public sealed partial record Placa
{
    public string Value { get; }

    public Placa(string value)
    {
        var normalized = (value ?? string.Empty)
            .Replace("-", string.Empty)
            .Replace(" ", string.Empty)
            .ToUpperInvariant();

        if (!OldFormat().IsMatch(normalized) && !MercosulFormat().IsMatch(normalized))
        {
            throw new ArgumentException("Placa inválida.", nameof(value));
        }

        Value = normalized;
    }

    public static bool TryParse(string? value, out Placa? placa)
    {
        try
        {
            placa = new Placa(value ?? string.Empty);
            return true;
        }
        catch (ArgumentException)
        {
            placa = null;
            return false;
        }
    }

    [GeneratedRegex("^[A-Z]{3}[0-9]{4}$")]
    private static partial Regex OldFormat();

    [GeneratedRegex("^[A-Z]{3}[0-9][A-Z][0-9]{2}$")]
    private static partial Regex MercosulFormat();

    public override string ToString() => Value;
}
