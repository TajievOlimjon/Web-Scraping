using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using WebApi.JsonHalpers;

namespace WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferProductController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public TransferProductController(IFileService fileService, IWebHostEnvironment webHostEnvironment)
        {
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = new List<Product>();
                var jsonFileName = "Product.json";
                var allProducts = await ConvertToObject.JsonFileConvertToObjects<Product>(_webHostEnvironment, jsonFileName, FolderType.JsonObject);
                var count = allProducts.OrderBy(x => x.Id).ToList().Count;
                int productId =count!=0?count:0;

                var web = new HtmlWeb();
                var document = web.Load("https://shantui.com.ru/product/tandemnyi-katok-shantui-srd08");

                var productHTMLElement = document.DocumentNode.QuerySelector("div.v-card");
               

                var image = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("img.v-img").Attributes["src"].Value);
                var productName = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("h1.title").InnerText.TrimStart().TrimEnd());
                var productAboutName = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("h1.head-title").InnerText.TrimStart().TrimEnd());

                var product = new Product
                {
                    Id = ++productId,
                    Name = productName,
                    FileName = image,
                    CategoryId = 1
                };
                var productAbouts = new List<ProductAbout>();
                var newProductAbout = new ProductAbout
                {
                    Id = 1,
                    ProductId = ++productId,
                    ProductAboutName = productAboutName
                };

                var attributes = new List<Attribute>();
                var attributeDetails = new List<AttributeDetail>();

                var htmlAttributeElements = document.DocumentNode.QuerySelectorAll("li.c-product-attrs-item");
                foreach (var html in htmlAttributeElements)
                {
                    var attributeName = HtmlEntity.DeEntitize(html.QuerySelector("div.title").InnerText.TrimStart().TrimEnd());

                    var newAttribute = new Attribute
                    {
                        AttributeName = attributeName
                    };
                    var htmlAttributeDetails = document.DocumentNode.QuerySelectorAll("ul.c-product-attrs-list");
                    foreach (var htmlAttributeDetail in htmlAttributeDetails)
                    {
                        var productAttributeDetailName = HtmlEntity.DeEntitize(html.QuerySelector("div.label").InnerText.TrimStart().TrimEnd());

                        var productAttributeDetailValue = HtmlEntity.DeEntitize(html.QuerySelector("div.value").InnerText.TrimStart().TrimEnd());

                        var newAttributeDetail = new AttributeDetail
                        {
                            ProductAttributeDetail = new ProductAttributeDetail
                            {
                                Name = productAttributeDetailName,
                                NameValue = productAttributeDetailValue
                            }
                        };

                        attributeDetails.Add(newAttributeDetail);
                    }

                    newAttribute.AttributeDetails = attributeDetails;
                    attributes.Add(newAttribute);
                }
                newProductAbout.Attributes = attributes;
                productAbouts.Add(newProductAbout);
                product.ProductAbouts = productAbouts;
                products.Add(product);

                await ConvertToJsonFile.ObjectsConvertToJsonFile(products, _webHostEnvironment, jsonFileName, FolderType.JsonObject);
                //var fileName = await _fileService.DownloadFileAsync(FolderType.Image, image);

                return StatusCode(200, "Success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> CheckWebSite()
        {
            try
            {
                var jsonFileName = "Product.json";
                var allProducts = await ConvertToObject.JsonFileConvertToObjects<Product>(_webHostEnvironment, jsonFileName, FolderType.JsonObject);
                return StatusCode(200, allProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
