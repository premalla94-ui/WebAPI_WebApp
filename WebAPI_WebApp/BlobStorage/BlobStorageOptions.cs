namespace WebAPI_WebApp.BlobStorage;

public sealed class BlobStorageOptions
{
    public const string SectionName = "BlobStorage";

    public string? ConnectionString { get; init; }
    public string? ServiceUri { get; init; }
    public string ContainerName { get; init; } = "documents";
}

