using System.Text.RegularExpressions;

namespace Infrastructure;
public static partial class FileNameSanitizer
{
    private const int MaxFileNameLength = 200;

    public static string Sanitize(Uri url)
    {
        ArgumentNullException.ThrowIfNull(url);
        return SanitizeString(url.ToString());
    }

    [GeneratedRegex(@"[:/\\?&=#%+<>|\""]")]
    private static partial Regex SpecialCharsRegex();

    [GeneratedRegex(@"_{2,}")]
    private static partial Regex MultipleUnderscoresRegex();

    private static string SanitizeString(string raw)
    {
        var sanitized = SpecialCharsRegex().Replace(raw, "_");
        sanitized = MultipleUnderscoresRegex().Replace(sanitized, "_");
        sanitized = sanitized.Trim('_');

        return sanitized.Length > MaxFileNameLength
            ? sanitized[..MaxFileNameLength]
            : sanitized;
    }
}