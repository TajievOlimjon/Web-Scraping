using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using WebApi.Entities;
using WebApi.JsonHalpers;

namespace WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferProductController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _dbContext;
        public TransferProductController(
            IFileService fileService,
            IWebHostEnvironment webHostEnvironment,
            ApplicationDbContext dbContext)
        {
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
            _dbContext = dbContext;
        }
        [HttpGet("Start-transfer-products-from-website-to-DB")]
        public async Task<IActionResult> StartTransferProducts()
        {
            try
            {
                var urlAddresses = await _dbContext.UrlAddresses.Where(x => x.VisitedAddress == false).ToListAsync();
                var newUrlAddresses = new List<UrlAddress>();
                foreach (var urlAddress in urlAddresses)
                {
                    newUrlAddresses.Add(urlAddress);
                    var web = new HtmlWeb();
                    HtmlDocument document = web.Load(urlAddress.Url);


                    var headersElements = document.DocumentNode.QuerySelectorAll("ul.c-breadcrumbs").SelectMany(x => x.ChildNodes).SkipLast(1).ToList();
                    var categoryName = headersElements.LastOrDefault()?.InnerText.TrimStart().TrimEnd();

                    int categoryId = 0;
                    var category = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Name == categoryName);
                    if (category == null)
                    {
                        var newCategory = new Category
                        {
                            Name = categoryName
                        };
                        await _dbContext.Categories.AddAsync(newCategory);
                        await _dbContext.SaveChangesAsync();
                        categoryId = newCategory.Id;
                    }
                    else { categoryId = category.Id; }

                    var productHTMLElement = document.DocumentNode.QuerySelector("div.v-card");
                    var productName = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("h1.title").InnerText.TrimStart().TrimEnd());
                    var productImageUrl = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("img.v-img").Attributes["src"].Value);

                    int productId = 0;
                    var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Name == productName && x.CategoryId == categoryId);
                    if (product == null)
                    {

                        var newProduct = new Product
                        {
                            CategoryId = categoryId,
                            Name = productName
                        };
                        await _dbContext.Products.AddAsync(newProduct);
                        await _dbContext.SaveChangesAsync();
                        productId = newProduct.Id;
                    }
                    else { productId = product.Id; }


                    var htmlAdditionalImageElements = productHTMLElement.QuerySelector("div.v-slider").ChildNodes.ToList();
                    foreach (var htmlAdditionalImageElement in htmlAdditionalImageElements)
                    {
                        var productAdditionalImageUrl = htmlAdditionalImageElement.QuerySelector("img.v-img").Attributes["src"].Value;

                        var additionalProductImage= new ProductImage
                        {
                            FileName = productAdditionalImageUrl,
                            ProductId = productId
                        };
                        await _dbContext.ProductImages.AddAsync(additionalProductImage);
                        await _dbContext.SaveChangesAsync();
                    }

                    // attributes
                    var htmlAttributeElements = document.DocumentNode.QuerySelectorAll("li.c-product-attrs-item");
                    int attributeId = 0;
                    foreach (var htmlAttributeElement in htmlAttributeElements)
                    {
                        var attributeName = HtmlEntity.DeEntitize(htmlAttributeElement.QuerySelector("div.title").InnerText.TrimStart().TrimEnd());

                        var attribute = await _dbContext.Attributes.FirstOrDefaultAsync(x => x.Name == attributeName);
                        if (attribute == null)
                        {
                            var newAttribute = new Attribute
                            {
                                Name = attributeName
                            };
                            await _dbContext.Attributes.AddAsync(newAttribute);
                            await _dbContext.SaveChangesAsync();
                            attributeId = newAttribute.Id;
                        }
                        else { attributeId = attribute.Id; }

                        var htmlProductAttributeDetail = htmlAttributeElement.QuerySelector("ul.c-product-attrs-list");
                        var htmlAttributeDetails = htmlProductAttributeDetail.QuerySelectorAll("li.item");
                        int attributeDetailId = 0;
                        foreach (var htmlAttributeDetail in htmlAttributeDetails)
                        {
                            var productAttributeDetailName = HtmlEntity.DeEntitize(htmlAttributeDetail.QuerySelector("div.label").InnerText.TrimStart().TrimEnd());

                            var value = HtmlEntity.DeEntitize(htmlAttributeDetail.QuerySelector("div.value").InnerText.TrimStart().TrimEnd());
                            /*productAttributeDetailValue = Regex.Replace(productAttributeDetailValue, @"\s", "");*/
                            var attributeDetail = await _dbContext.AttributeDetails.FirstOrDefaultAsync(x => x.Name == productAttributeDetailName && x.AttributeId==attributeId);
                            if (attributeDetail == null)
                            {
                                var newAttributeDetail = new AttributeDetail
                                {
                                    AttributeId = attributeId,
                                    Name = productAttributeDetailName
                                };

                                await _dbContext.AttributeDetails.AddAsync(newAttributeDetail);
                                await _dbContext.SaveChangesAsync();
                                attributeDetailId = newAttributeDetail.Id;
                            }
                            else { attributeDetailId = attributeDetail.Id; }


                            var productAttributeDetail = await _dbContext.ProductAttributeDetails.FirstOrDefaultAsync(x =>
                                x.AttributeDetailId == attributeDetailId &&
                                  x.ProductId == productId &&
                                    x.Value == value);

                            if (productAttributeDetail == null)
                            {
                                var newProductAttributeDetail = new ProductAttributeDetail
                                {
                                    AttributeDetailId = attributeDetailId,
                                    ProductId = productId,
                                    Value = value
                                };

                                await _dbContext.ProductAttributeDetails.AddAsync(newProductAttributeDetail);
                                await _dbContext.SaveChangesAsync();
                            }
                        }
                    }
                }
                await UpdateUrlAddress(newUrlAddresses);

                return StatusCode(200,"Success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        [HttpPost("AddPageUrlAddresses")]
        public async Task<IActionResult> AddPageUrlAddresses(string pageUrlAddress)
        {
            try
            {
                var pageAddress = _dbContext.PageUrlAddresses.FirstOrDefault(x => x.PageUrl == pageUrlAddress.TrimStart().TrimEnd());
                if (pageAddress == null)
                {
                    var newPageUrlAddress = new PageUrlAddress
                    {
                        PageUrl = pageUrlAddress.TrimStart().TrimEnd(),
                        VisitedAddress = false
                    };
                    await _dbContext.PageUrlAddresses.AddAsync(newPageUrlAddress);
                    await _dbContext.SaveChangesAsync();

                    return StatusCode(200, "Successfully added");
                }
                return StatusCode(403, "PageUrl address already exists !");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        private async Task UpdateUrlAddress(List<UrlAddress> urlAddresses)
        {
            try
            {
                foreach (var urlAddress in urlAddresses)
                {
                    var url = await _dbContext.UrlAddresses.FirstOrDefaultAsync(x=>x.Id==urlAddress.Id);
                    if (url != null)
                    {
                        url.VisitedAddress = true;
                        await _dbContext.SaveChangesAsync();
                    }
                    //_dbContext.UpdateRange(urlAddresses);
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
        }
        [HttpGet("TransferUrlAddress")]
        public async Task<IActionResult> TransferUrlAddress()
        {
            try
            {
                var pageUrlAddresses = await _dbContext.PageUrlAddresses.Where(x=>x.VisitedAddress==false).ToListAsync();
                foreach (var page in pageUrlAddresses)
                {
                    var pageAddress = await _dbContext.PageUrlAddresses.FirstOrDefaultAsync(x=>x.PageUrl==page.PageUrl);
                    if (pageAddress == null) continue;
                    
                    pageAddress.VisitedAddress = true;
                    await _dbContext.SaveChangesAsync();

                    var web = new HtmlWeb();
                    HtmlDocument document = web.Load(pageAddress.PageUrl);

                    var htmlUrlAddressElements = document.DocumentNode.QuerySelectorAll("li.c-grid-item").SelectMany(x => x.ChildNodes).ToList();

                    foreach (var html in htmlUrlAddressElements)
                    {

                        var urlAddress = html.QuerySelector("a.picture").Attributes["href"].Value;

                        //var urlAddress = HtmlEntity.DeEntitize(html.QuerySelector("h1.title").InnerText.TrimStart().TrimEnd());

                        var url = await _dbContext.UrlAddresses.FirstOrDefaultAsync(x=>x.Url==urlAddress);
                        if (url == null)
                        {
                            var newUrlAddress = new UrlAddress
                            {
                                Url = "https://shantui.com.ru"+urlAddress,
                                VisitedAddress = false
                            };

                            await _dbContext.UrlAddresses.AddAsync(newUrlAddress);
                            await _dbContext.SaveChangesAsync();
                        }
                    }
                }
                return StatusCode(200, "Success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("Download-Additional-Product-Images")]
        public async Task<IActionResult> DownloadProductImages()
        {
            var imagesURLs = await _dbContext.ProductImages.AsNoTracking().ToListAsync();
            foreach (var imageUrl in imagesURLs)
            {
               var fileName = await _fileService.DownloadFileAsync(FolderType.Image,imageUrl.FileName);

                imageUrl.FileName = fileName;
                _dbContext.Update(imageUrl);
                await _dbContext.SaveChangesAsync();
            }

            return Ok("Success");
        }
       /* [HttpGet("Download-Product-Image")]
        public async Task<IActionResult> DownloadProductImage()
        {
            var imagesURLs = await _dbContext.Products.AsNoTracking().ToListAsync();
            foreach (var imageUrl in imagesURLs)
            {
                var fileName = await _fileService.DownloadFileAsync(FolderType.Image, imageUrl.FileName);

                 imageUrl.FileName = fileName;
                _dbContext.Update(imageUrl);
                await _dbContext.SaveChangesAsync();
            }

            return Ok("Success");
        }*/
    }
}
