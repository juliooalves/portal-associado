namespace ProAuto.PortalAssociado.Web.Domain;

public sealed record Cpf
{
    public string Value { get; }

    public Cpf(string value)
    {
        var digits = new string((value ?? string.Empty).Where(char.IsAsciiDigit).ToArray());

        if (digits.Length != 11)
        {
            throw new ArgumentException("CPF deve conter 11 dígitos.", nameof(value));
        }

        if (digits.Distinct().Count() == 1)
        {
            throw new ArgumentException("CPF inválido.", nameof(value));
        }

        if (!HasValidCheckDigits(digits))
        {
            throw new ArgumentException("CPF inválido.", nameof(value));
        }

        Value = digits;
    }

    public static bool TryParse(string? value, out Cpf? cpf)
    {
        try
        {
            cpf = new Cpf(value ?? string.Empty);
            return true;
        }
        catch (ArgumentException)
        {
            cpf = null;
            return false;
        }
    }

    public string Formatted =>
        $"{Value[..3]}.{Value[3..6]}.{Value[6..9]}-{Value[9..]}";

    private static bool HasValidCheckDigits(string digits)
    {
        var numbers = digits.Select(c => c - '0').ToArray();

        var firstCheck = ComputeCheckDigit(numbers, length: 9, initialWeight: 10);
        if (numbers[9] != firstCheck)
        {
            return false;
        }

        var secondCheck = ComputeCheckDigit(numbers, length: 10, initialWeight: 11);
        return numbers[10] == secondCheck;
    }

    private static int ComputeCheckDigit(int[] numbers, int length, int initialWeight)
    {
        var sum = 0;
        for (var i = 0; i < length; i++)
        {
            sum += numbers[i] * (initialWeight - i);
        }

        var remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }

    public override string ToString() => Value;
}
