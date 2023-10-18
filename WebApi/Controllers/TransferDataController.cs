/*using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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

            var products = new List<ProductDto>();
            foreach (var productHTMLElement in productHTMLElements)
            {
                var url = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("a").Attributes["href"].Value);
                var image = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("img").Attributes["src"].Value);
                var name = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("h2").InnerText);
                var price = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector(".price").InnerText);

                var fileName = await _fileService.DownloadFileAsync(FolderType.Image,image);
                var product = new ProductDto{ Url = url, Image = image, Name = name, Price = price };
                products.Add(product);
            }

            var jsonFileName = "Product.json";
            await ConvertToJsonFile.ObjectsConvertToJsonFile(products, _webHostEnvironment, jsonFileName, FolderType.JsonObject);

            return Ok();
        }
        [HttpGet("AllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            var jsonFileName = "Product.json";
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, FolderType.JsonObject, jsonFileName);
            using FileStream fileStream = new FileStream(filePath, FileMode.Open);
            var products = await JsonSerializer.DeserializeAsync<List<ProductDto>>(fileStream);

            return Ok(products);
        }
        *//*[HttpGet("Start-transfer-products-from-website-to-json")]*/
        /* public async Task<IActionResult> StartTransfer()
         {
             try
             {
                 var productJsonFileName = "Product.json";
                 var allProducts = await ConvertToObject.JsonFileConvertToObjects<Category>(_webHostEnvironment, productJsonFileName, FolderType.JsonObject);
                 var count = allProducts.SelectMany(x=>x.Products).OrderBy(x => x.Id).ToList().Count;
                 int productId =count!=0?count:0;

                 var categoryJsonFileName = "Category.json";
                 var allCategories = await ConvertToObject.JsonFileConvertToObjects<Category>(_webHostEnvironment, categoryJsonFileName, FolderType.JsonObject);
                 var allCategoriesCount = allCategories.OrderBy(x => x.Id).ToList().Count;
                 int categoryId = allCategoriesCount != 0 ? allCategoriesCount : 0;


                 var web = new HtmlWeb();
                 HtmlDocument document;
                 var urlAddressJsonFileName = "UrlAddress.json";
                 var allUrlAddresses = await ConvertToObject.JsonFileConvertToObjects<UrlAddress>(_webHostEnvironment, urlAddressJsonFileName, FolderType.JsonObject);
                 var updateUrlAddresses = new List<UrlAddress>();
                 foreach (var urlAddress in allUrlAddresses)
                 {
                     if (urlAddress.VisitedAddress == true) continue;
                     document = web.Load(urlAddress.Url);
                     urlAddress.VisitedAddress = true;
                     updateUrlAddresses.Add(urlAddress);

                     var headersElements = document.DocumentNode.QuerySelectorAll("ul.c-breadcrumbs").SelectMany(x => x.ChildNodes).ToList();
                     var categoryName = headersElements.Skip(2).FirstOrDefault()?.InnerText.TrimStart().TrimEnd();

                     var productHTMLElement = document.DocumentNode.QuerySelector("div.v-card");
                     var productName = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("h1.title").InnerText.TrimStart().TrimEnd());
                     var productImage = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("img.v-img").Attributes["src"].Value);


                     var category = allCategories.OrderBy(x => x.Id).FirstOrDefault(x => x.Name == categoryName);
                     if (category == null)
                     {
                         category = new Category
                         {
                             Id = ++categoryId,
                             Name = categoryName
                         };
                         var categories = new List<Category>() { category };
                         await ConvertToJsonFile.ObjectsConvertToJsonFile(categories, _webHostEnvironment, categoryJsonFileName, FolderType.JsonObject);
                     }
                     var product = new Product
                     {
                         Id = ++productId,
                         Name = productName,
                         FileName = productImage,
                         CategoryId = category.Id
                     };

                     var products = new List<Product>();
                     var attributes = new List<Attribute>();
                     var attributeDetails = new List<AttributeDetail>();

                     var htmlAttributeElements = document.DocumentNode.QuerySelectorAll("li.c-product-attrs-item");
                     int newAttributeId = 0;
                     Attribute newAttribute;
                     foreach (var htmlAttributeElement in htmlAttributeElements)
                     {
                         var attributeName = HtmlEntity.DeEntitize(htmlAttributeElement.QuerySelector("div.title").InnerText.TrimStart().TrimEnd());

                         newAttribute = new Attribute
                         {
                             Id = ++newAttributeId,
                             ProductId = product.Id,
                             AttributeName = attributeName
                         };
                         var htmlProductAttributeDetail = htmlAttributeElement.QuerySelector("ul.c-product-attrs-list");
                         var htmlAttributeDetails = htmlProductAttributeDetail.QuerySelectorAll("li.item");
                         int newAttributeDetailId = 0;
                         AttributeDetail newAttributeDetail;
                         foreach (var htmlAttributeDetail in htmlAttributeDetails)
                         {
                             var productAttributeDetailName = HtmlEntity.DeEntitize(htmlAttributeDetail.QuerySelector("div.label").InnerText.TrimStart().TrimEnd());

                             var productAttributeDetailValue = HtmlEntity.DeEntitize(htmlAttributeDetail.QuerySelector("div.value").InnerText.TrimStart().TrimEnd());

                             newAttributeDetail = new AttributeDetail
                             {
                                 Id = ++newAttributeDetailId,
                                 AttributeId = newAttribute.Id,
                                 ProductAttributeDetailName = new ProductAttributeDetailName
                                 {
                                     Name = productAttributeDetailName,
                                     NameValue = Regex.Replace(productAttributeDetailValue.Trim(), @"\s", "")
                                 }
                             };
                             attributeDetails.Add(newAttributeDetail);
                         }

                         newAttribute.AttributeDetails = attributeDetails;
                         attributes.Add(newAttribute);
                         attributeDetails = new();
                     }

                     product.Attributes = attributes;
                     products.Add(product);

                     await ConvertToJsonFile.ObjectsConvertToJsonFile(products, _webHostEnvironment, productJsonFileName, FolderType.JsonObject);
                     //var fileName = await _fileService.DownloadFileAsync(FolderType.Image, image);
                 }

                 if (updateUrlAddresses.Count != 0)
                 {
                     await ConvertToJsonFile.UpdateJsonFile(updateUrlAddresses, _webHostEnvironment, urlAddressJsonFileName, FolderType.JsonObject);
                 }

                 return StatusCode(200, "Success");
             }
             catch (Exception ex)
             {
                 return StatusCode(500, ex.Message);
             }
         }*//*
    }

}

*/