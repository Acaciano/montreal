using System.IO;

namespace Montreal.Core.Domain.Interfaces.Storage
{
    public interface IStorage
    {
        void UploadFile(Stream stream, string container, string filename, bool isPrivate = false);
        Stream GetFile(string container, string filename);
        string GetFileUrl(string container, string filename);
    }
}
