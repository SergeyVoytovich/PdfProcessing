namespace PdfProcessing.Data;

internal class FileStorage : IFileStorage
{
    protected virtual string RootPath { get; }

    public FileStorage(string rootPath)
    {
        RootPath = string.IsNullOrWhiteSpace(rootPath) ? throw new ArgumentNullException(nameof(rootPath)) : rootPath;
        Directory.CreateDirectory(RootPath);
    }

    public async Task<string> AddAsync(string fileName, Stream stream, CancellationToken cancellationToken = default)
    {
        var path = Path.Combine(RootPath, fileName.ToUniqueFileName());

        using var fs = new FileStream(path, FileMode.Create);
        await stream.CopyToAsync(fs, cancellationToken);

        return path;
    }
}
