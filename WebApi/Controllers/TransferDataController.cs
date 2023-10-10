using HtmlAgilityPack;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

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
        public IActionResult GetWebSiteData()
        {
            var web = new HtmlWeb();

            var document = web.Load("https://scrapeme.live/shop/");

            var productHTMLElements = document.DocumentNode.QuerySelectorAll("li.product");

            var products = new List<Product>();
            // iterating over the list of product elements 
            foreach (var productHTMLElement in productHTMLElements)
            {
                // scraping the interesting data from the current HTML element 
                var url = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("a").Attributes["href"].Value);
                var image = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("img").Attributes["src"].Value);
                var name = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("h2").InnerText);
                var price = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector(".price").InnerText);
                // instancing a new product object 
                var product = new Product{ Url = url, Image = image, Name = name, Price = price };
                // adding the object containing the scraped data to the list 
                products.Add(product);
            }

            // initializing the CSV output file 
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath,FolderType.Other, "products.csv");
            using (var writer = new StreamWriter(filePath))
            // initializing the CSV writer 
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                // populating the CSV file 
                csv.WriteRecords(products);
            }
            return Ok();
        }

    }
}

