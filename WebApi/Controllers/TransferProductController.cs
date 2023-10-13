using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
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
        [HttpGet("StartTransfer")]
        public async Task<IActionResult> StartTransfer()
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
        }
        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var productJsonFileName = "Product.json";
                var allProducts = await ConvertToObject.JsonFileConvertToObjects<Product>(_webHostEnvironment, productJsonFileName, FolderType.JsonObject);
                return StatusCode(200, allProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categoryJsonFileName = "Category.json";
                var allProducts = await ConvertToObject.JsonFileConvertToObjects<Category>(_webHostEnvironment, categoryJsonFileName, FolderType.JsonObject);
                return StatusCode(200, allProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("AddUrlAddresses")]
        public async Task<IActionResult> AddUrlAddress(string urlAddress)
        {
            try
            {
                var urlAddressJsonFileName = "UrlAddress.json";
                var allUrlAddresses = await ConvertToObject.JsonFileConvertToObjects<UrlAddress>(_webHostEnvironment, urlAddressJsonFileName, FolderType.JsonObject);

                var address = allUrlAddresses.FirstOrDefault(x => x.Url == urlAddress.TrimStart().TrimEnd());
                if (address == null)
                {
                    var newUrlAddresses = new List<UrlAddress>
                    { 
                        new UrlAddress 
                        {
                            Url = urlAddress.TrimStart().TrimEnd(),
                            VisitedAddress = false
                        } 
                    };
                    await ConvertToJsonFile.ObjectsConvertToJsonFile(newUrlAddresses, _webHostEnvironment, urlAddressJsonFileName, FolderType.JsonObject);

                    return StatusCode(200, "Successfully added");
                }
                return StatusCode(403, "Url address already exists !");
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }
    }
}
