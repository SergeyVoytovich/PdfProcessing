namespace PdfProcessing.Api.Configuration;

public static class ConfigurationManagerExtensions
{
    private const string PdfStorageConnectionString = "PdfStorage";

    public static string PdfStorage(this ConfigurationManager configuration)
        => configuration.GetConnectionString(PdfStorageConnectionString) ?? throw new InvalidOperationException();
}
