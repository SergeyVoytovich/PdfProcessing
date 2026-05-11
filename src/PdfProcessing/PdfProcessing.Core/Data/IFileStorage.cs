namespace PdfProcessing.Data;

public interface IFileStorage
{
    Task<string> AddAsync(string fileName, Stream stream, CancellationToken cancellationToken = default);
}
