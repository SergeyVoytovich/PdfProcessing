namespace PdfProcessing.Data.Repositories;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException()
    {
    }

    public EntityNotFoundException(string? message) : base(message)
    {
    }

    public EntityNotFoundException(Guid id)
        :this($"Document with id '{id}' not found.")
    {

    }

    public EntityNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
