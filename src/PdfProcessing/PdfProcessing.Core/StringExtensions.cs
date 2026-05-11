namespace PdfProcessing.Data;

public static class StringExtensions
{
    public static string ToUniqueFileName(this string filename)
        => $"{Path.GetFileNameWithoutExtension(filename)}_{Guid.NewGuid():N}{Path.GetExtension(filename)}";

    public static bool IsPdf(this string filename)
        => string.Equals(Path.GetExtension(filename), ".pdf", StringComparison.OrdinalIgnoreCase);
}
