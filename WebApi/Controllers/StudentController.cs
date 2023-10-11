/*using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Text.Json;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileService _fileService;
        public StudentController(IWebHostEnvironment webHostEnvironment, IFileService fileService)
        {
            _webHostEnvironment = webHostEnvironment;
            _fileService = fileService;
        }
        [HttpGet("GetAllStudents_1")]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = new List<ProductDto>
            {
                new ProductDto{Id=1,FirstName = "Olimjon",LastName="Tajiev",Age =22},
                new ProductDto{Id=2,FirstName = "Ali",LastName="Aliev",Age =22},
                new ProductDto{Id=3,FirstName = "Azam",LastName="Azamov",Age =22},
                new ProductDto{FirstName = "Anvar",LastName="Nazarzoda",Age =22},
                new ProductDto{FirstName = "Salah",LastName="Soliev",Age =22}
            };
            var fileName = "Student.json";
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, FolderType.JsonObject, fileName);
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            var resolver = new CamelCasePropertyNamesContractResolver();
            var settings = new JsonSerializerSettings { ContractResolver = resolver };
            var newStudents = JsonConvert.DeserializeObject<List<ProductDto>>(filePath);
            if(newStudents.Count==0 && students.Count!=0)
            {
                students = students.Take(1).ToList();
                var json1 = JsonConvert.SerializeObject(students, Formatting.Indented, settings);
                await System.IO.File.WriteAllTextAsync(filePath, json1);
                students = students.Skip(1).ToList();
            }
            foreach (var item in students)
            {
                var json = JsonConvert.SerializeObject(item, Formatting.Indented, settings);
                await System.IO.File.WriteAllTextAsync(filePath, json);
            }
            
            newStudents = JsonConvert.DeserializeObject<List<ProductDto>>(filePath);

            return Ok(newStudents);
        }
        [HttpGet("GetAllStudents_2")]
        public async Task<IActionResult> GetAllStudents_2()
        {
            var students = new List<ProductDto>
            {
                new ProductDto{Id=1,FirstName = "Olimjon",LastName="Tajiev",Age =22},
                new ProductDto{Id=2,FirstName = "Ali",LastName="Aliev",Age =22},
                new ProductDto{Id=3,FirstName = "Azam",LastName="Azamov",Age =22},
                new ProductDto{FirstName = "Anvar",LastName="Nazarzoda",Age =22},
                new ProductDto{FirstName = "Salah",LastName="Soliev",Age =22}
            };
            var fileName = "Student.json";
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, FolderType.JsonObject, fileName);
            
            JArray jsonArray = new JArray(filePath);
            foreach (var student in students)
            {
                var json = JsonConvert.SerializeObject(student);
                var tok = JToken.Parse(json);
                jsonArray.Add(tok);
            }
            var convertedJson = JsonConvert.SerializeObject(jsonArray,Formatting.Indented);
            await System.IO.File.WriteAllTextAsync(filePath, convertedJson);

            return Ok();
        }
        [HttpGet("GetAllStudents_3")]
        public async Task<IActionResult> GetAllStudents_3()
        {
            var students = new List<ProductDto>
            {
                new ProductDto{Id=1,FirstName = "Olimjon",LastName="Tajiev",Age =22},
                new ProductDto{Id=2,FirstName = "Ali",LastName="Aliev",Age =22},
                new ProductDto{Id=3,FirstName = "Azam",LastName="Azamov",Age =22},
                new ProductDto{FirstName = "Anvar",LastName="Nazarzoda",Age =22},
                new ProductDto{FirstName = "Salah",LastName="Soliev",Age =22}
            };

            var fileName = "Student.json";
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, FolderType.JsonObject, fileName);
            using FileStream create = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read);
            
            if (create.Length>6)
            {
                await create.DisposeAsync();
                using FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate);
                var models = System.Text.Json.JsonSerializer.Deserialize<List<ProductDto>>(stream);
                models.AddRange(students);
                await stream.DisposeAsync();

                using FileStream createStream = new FileStream(filePath, FileMode.OpenOrCreate);
                var options1 = new JsonSerializerOptions() { WriteIndented = true };
                await System.Text.Json.JsonSerializer.SerializeAsync(createStream, models, options1);
                await createStream.DisposeAsync();
            }
            else
            {
                await create.DisposeAsync();
                using FileStream createStream =new FileStream(filePath,FileMode.OpenOrCreate,FileAccess.Write);
                var options = new JsonSerializerOptions() { WriteIndented = true };
                await System.Text.Json.JsonSerializer.SerializeAsync(createStream, students, options);
                await createStream.DisposeAsync();
            }

            return Ok();
        }
    }
}
*/