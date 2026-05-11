using PdfProcessing.Messaging.Messages;
using System.Text;
using System.Text.Json;

namespace PdfProcessing.Messaging;

internal static class MessageExtensions
{
    public static byte[] ToBytes<T>(this T message) where T: IMessage
    {
        var json = JsonSerializer.Serialize(message);
        return Encoding.UTF8.GetBytes(json);
    }

    public static T? ToMessage<T>(this ReadOnlyMemory<byte> body)
    {
        byte[] bytes = body.ToArray();
        string json = Encoding.UTF8.GetString(bytes);
        T? message = JsonSerializer.Deserialize<T>(json);
        return message;
    }
}
