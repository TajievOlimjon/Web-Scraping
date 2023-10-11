using System.Text.Json;

namespace WebApi.JsonHalpers
{
    public static class ConvertToObject
    {
        public static async Task<List<T>> JsonFileConvertToObjects<T>(IWebHostEnvironment _webHostEnvironment, string fileName, string folderName)
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, folderName, fileName);
            using FileStream fileStream = new FileStream(filePath, FileMode.Open);
            var allItems =  await JsonSerializer.DeserializeAsync<List<T>>(fileStream)??new();
            await fileStream.DisposeAsync();
            return allItems;
        }
        public static async Task<T?> JsonFileConvertToObject<T>(IWebHostEnvironment _webHostEnvironment, string fileName, string folderName)
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, folderName, fileName);
            using FileStream fileStream = new FileStream(filePath, FileMode.Open);
            var item = await JsonSerializer.DeserializeAsync<T>(fileStream);
            await fileStream.DisposeAsync();
            return item;
        }
    }
}
