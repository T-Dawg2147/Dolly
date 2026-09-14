namespace Dolly.Application.Models;

// TODO: Shold this be a static class or just make it bespoke to whatevers asking for it?
public static class FlagValue
{
    public static bool ToBool(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        var v = value.Trim().ToLowerInvariant();

        return v is "y" or "yes" or "true" or "1" or "t";
    }

    public static string FromBool(bool value, string trueText = "Yes", string falseText = "No")
        => value ? trueText : falseText;
}