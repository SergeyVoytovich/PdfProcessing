namespace PdfProcessing.Data;

internal static class StringExtensions
{
    public static string ToUniqueFileName(this string filename)
        => $"{Path.GetFileNameWithoutExtension(filename)}_{Guid.NewGuid():N}{Path.GetExtension(filename)}";
}
