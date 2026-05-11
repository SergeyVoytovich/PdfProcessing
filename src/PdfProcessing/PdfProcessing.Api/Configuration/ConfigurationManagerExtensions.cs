using PdfProcessing.Messaging.Configuration;

namespace PdfProcessing.Api.Configuration;

public static class ConfigurationManagerExtensions
{
    private const string PdfStorageConnectionString = "PdfStorage";
    private const string MesasgeBusSectionName = "MessageBus";

    public static string PdfStorage(this ConfigurationManager configuration)
        => configuration.GetConnectionString(PdfStorageConnectionString) ?? throw new InvalidOperationException();

    public static MessageBusConfiguration MessagingConfiguration(this ConfigurationManager configuration)
        => configuration.GetSection(MesasgeBusSectionName).Get<MessageBusConfiguration>() ?? throw new InvalidOperationException();
}
