using System.Net;

namespace WebApi
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<string> DownloadFileAsync(string folderName, string fileUrlAddress)
        {
            var fileName = fileUrlAddress;
            if (fileUrlAddress.StartsWith("https://"))
            {
                using var client = new WebClient();
                var urlAddress = new Uri(fileUrlAddress);
                fileName = Path.GetFileName(fileUrlAddress);
                fileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, FolderType.Image, fileName);
                await client.DownloadFileTaskAsync(urlAddress, filePath);
                client.Dispose();
            }
            return fileName;
        }
    }
}
