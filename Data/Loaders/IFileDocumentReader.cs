namespace AppLocalizationUtil.Data.Loaders
{
    public interface IFileDocumentReader : IDocumentReader
    {
        string FileName { get; set; }
    }
}