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
            using var client = new WebClient();
            var urlAddress = new Uri(fileUrlAddress);
            var fileName = Path.GetFileName(fileUrlAddress);
            fileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, FolderType.Image, fileName);
            await client.DownloadFileTaskAsync(urlAddress, filePath);
            return fileName;
        }
    }
}
