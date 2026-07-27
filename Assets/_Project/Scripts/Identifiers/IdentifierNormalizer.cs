namespace LL.Identifiers
{
    internal static class IdentifierNormalizer
    {
        internal static string Normalize(string value) =>
            value?.Trim().ToLowerInvariant() ?? string.Empty;
    }
}