using System.IO;
using System.Threading.Tasks;
using AppLocalizationUtil.Data.Loaders;
using AppLocalizationUtil.Entities;

namespace AppLocalizationUtil.Data.Sources
{
    public class RemoteFileSource : ISource
    {
        private readonly IFileDownloader _fileDownloader;
        private readonly IFileDocumentReader _documentReader;

        public RemoteFileSource(IFileDownloader fileDownloader, IFileDocumentReader documentReader)
        {
            _fileDownloader = fileDownloader;
            _documentReader = documentReader;
        }

        public void Dispose()
        {
            try
            {
                FileInfo fi = new FileInfo(_fileDownloader.FileName);
                if (fi.Exists)
                {
                    fi.Delete();
                }
            }
            catch
            {
            }
        }

        public async Task<Document> LoadAsync()
        {
            await _fileDownloader.DownloadAsync();
            _documentReader.FileName = _fileDownloader.FileName;
            return await _documentReader.ReadAsync();
        }
    }
}