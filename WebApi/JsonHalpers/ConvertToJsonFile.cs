using System.Text.Json;

namespace WebApi
{
    public static class ConvertToJsonFile
    {
        public static async Task ObjectsConvertToJsonFile<T>(this List<T> models,IWebHostEnvironment _webHostEnvironment, string fileName, string folderName)
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, folderName, fileName);
            var jsonSerializerOptions = new JsonSerializerOptions() { WriteIndented = true };
            using FileStream create = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read);

            if (create.Length > 8)
            {
                await create.DisposeAsync();
                using FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate);
                var perModels = JsonSerializer.Deserialize<List<T>>(stream);
                perModels.AddRange(models);
                await stream.DisposeAsync();

                using FileStream createStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                await JsonSerializer.SerializeAsync(createStream, perModels, jsonSerializerOptions);
                await createStream.DisposeAsync();
            }
            else
            {
                await create.DisposeAsync();
                using FileStream createStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                await JsonSerializer.SerializeAsync(createStream, models, jsonSerializerOptions);
                await createStream.DisposeAsync();
            }
        }
        public static async Task UpdateJsonFile<T>(this List<T> models, IWebHostEnvironment _webHostEnvironment, string fileName, string folderName)
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, folderName, fileName);
            var jsonSerializerOptions = new JsonSerializerOptions() { WriteIndented = true };
            using FileStream createStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            await JsonSerializer.SerializeAsync(createStream, models, jsonSerializerOptions);
            await createStream.DisposeAsync();
        }
        public static async Task ObjectConvertToJsonFile<T>(this T model, IWebHostEnvironment _webHostEnvironment, string fileName, string folderName)
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, folderName, fileName);
            await using (FileStream fileStream = new(filePath, FileMode.OpenOrCreate))
            {
                var jsonSerializerOptions = new JsonSerializerOptions() { WriteIndented = true};
                await JsonSerializer.SerializeAsync(fileStream, model, jsonSerializerOptions);
                await fileStream.DisposeAsync();
            }
        }
    }
}


