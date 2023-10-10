using System.Text.Json;

namespace WebApi
{
    public static class ConvertToJsonFile
    {
        public static async Task ObjectsConvertToJsonFile<T>(this List<T> models,IWebHostEnvironment _webHostEnvironment, string fileName, string folderName)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                AllowTrailingCommas =true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, folderName, fileName);
            await using FileStream createStream = new FileStream(filePath, FileMode.OpenOrCreate);
            await JsonSerializer.SerializeAsync(createStream, models,options);
        }
        public static async Task ObjectConvertToJsonFile<T>(this T model, IWebHostEnvironment _webHostEnvironment, string fileName, string folderName)
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, folderName, fileName);
            await using FileStream createStream = new FileStream(filePath, FileMode.OpenOrCreate);
            await JsonSerializer.SerializeAsync(createStream, model);
        }
    }
}

