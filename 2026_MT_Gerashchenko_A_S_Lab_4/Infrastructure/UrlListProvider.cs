using System.Text.RegularExpressions;

namespace Infrastructure;
public static class UrlListProvider
{
    public static async Task<IReadOnlyList<Uri>> LoadAsync(string filePath, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(filePath);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("URL list file not found: " + filePath);
        }

        var lines = await File.ReadAllLinesAsync(filePath, ct).ConfigureAwait(false);

        return lines
            .Select(l => l.Trim())
            .Where(l => l.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                        l.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            .Distinct()
            .Select(l => new Uri(l, UriKind.Absolute))
            .ToList();
    }
}