namespace WebApi
{
    public interface IFileService
    {
        public Task<string> DownloadFileAsync(string folderName,string fileUrlAddress);
    }
}

