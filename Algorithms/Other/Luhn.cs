namespace Algorithms.Other;

/// <summary>
///     Luhn algorithm is a simple
///     checksum formula used to validate
///     a variety of identification numbers,
///     such as credit card numbers.
///     More information on the link:
///     https://en.wikipedia.org/wiki/Luhn_algorithm.
/// </summary>
public static class Luhn
{
    // Checking the validity of a sequence of numbers.
    public static bool Validate(string number) => GetSum(number) % 10 == 0;

    // Finds one missing digit. In place of the unknown digit, put "x".
    public static int GetLostNum(string number)
    {
        var missingDigitIndex = number.Length - 1 - number.LastIndexOf('x');
        var checkDigit = GetSum(number.Replace("x", "0")) * 9 % 10;

        return missingDigitIndex % 2 == 0
            ? checkDigit
            : Validate(number.Replace("x", (checkDigit / 2).ToString()))
                ? checkDigit / 2
                : (checkDigit + 9) / 2;
    }

    // Computes the sum found by the Luhn algorithm, optimized with Span.
    private static int GetSum(string number)
    {
        var sum = 0;
        var span = number.AsSpan();
        for (var i = 0; i < span.Length; i++)
        {
            var c = span[i];
            if (c is < '0' or > '9') continue;
            var digit = c - '0';
            digit = (i + span.Length) % 2 == 0 ? 2 * digit : digit;
            if (digit > 9) digit -= 9;
            sum += digit;
        }
        return sum;
    }
}
