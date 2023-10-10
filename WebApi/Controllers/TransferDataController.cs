using HtmlAgilityPack;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;
using System;

namespace WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferDataController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileService _fileService;
        public TransferDataController(IWebHostEnvironment webHostEnvironment, IFileService fileService)
        {
            _webHostEnvironment = webHostEnvironment;
            _fileService = fileService;
        }
        [HttpGet("GetWebSiteData")]
        public async Task<IActionResult> GetWebSiteData()
        {
            var web = new HtmlWeb();

            var document = web.Load("https://scrapeme.live/shop/page/2/");

            var productHTMLElements = document.DocumentNode.QuerySelectorAll("li.product");

            var products = new List<Product>();
            foreach (var productHTMLElement in productHTMLElements)
            {
                var url = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("a").Attributes["href"].Value);
                var image = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("img").Attributes["src"].Value);
                var name = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("h2").InnerText);
                var price = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector(".price").InnerText);

                var fileName = await _fileService.DownloadFileAsync(FolderType.Image,image);
                var product = new Product{ Url = url, Image = fileName, Name = name, Price = price };
                products.Add(product);
            }

            var jsonFileName = "Product.json";
            await ConvertToJsonFile.ObjectsConvertToJsonFile(products, _webHostEnvironment, jsonFileName, FolderType.Product);
            return Ok();
        }
        [HttpGet("AllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            var jsonFileName = "Product.json";
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, FolderType.Product, jsonFileName);
            using FileStream fileStream = new FileStream(filePath, FileMode.Open);
            var products = await JsonSerializer.DeserializeAsync<List<Product>>(fileStream);

            return Ok(products);
        }
    }

}

