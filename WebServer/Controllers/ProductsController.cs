using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        public ProductsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {

                var allCategories = await (from p in _dbContext.Products
                                           join c in _dbContext.Categories on p.CategoryId equals c.Id
                                           group p by new
                                           {
                                               p.CategoryId,
                                               p.Category.Name,
                                               p.Category.FileName
                                           } into key
                                           select new GetAllCategoriesDto
                                           {
                                               Id = key.Key.CategoryId,
                                               Name = key.Key.Name,
                                               FileName = key.Key.FileName,
                                               Products = key.Select(product => new GetAllProductsDto
                                               {
                                                   Id = product.Id,
                                                   Name = product.Name,
                                                   //FileName = product.FileName,
                                                   CategoryId = product.CategoryId,
                                                   AllAttributes = (from a in _dbContext.AttributeDetails
                                                                     join d in _dbContext.AttributeDetails on a.Id equals d.AttributeId
                                                                     join pad in _dbContext.ProductAttributeDetails on d.Id equals pad.AttributeDetailId
                                                                     where pad.ProductId == product.Id
                                                                     group d by new
                                                                     {
                                                                         d.AttributeId,
                                                                         d.Attribute.Name
                                                                     } into key
                                                                     select new GetAllAttributesDto
                                                                     {
                                                                         Id = key.Key.AttributeId,
                                                                         Name = key.Key.Name,
                                                                         Details = key.Select(d => new GetAllAttributeDetailsDto
                                                                         {
                                                                             Id = d.Id,
                                                                             Name = d.Name,
                                                                             NameValue = d.ProductAttributeDetails.FirstOrDefault(x => x.ProductId == product.Id).Value
                                                                         }).ToList()
                                                                     }).ToList()
                                               }).ToList()
                                           }).ToListAsync();

                return StatusCode(200, allCategories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
