using ITI.gRPC.API.Protos;
using ITI.gRPC.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace ITI.gRPC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        public ApiProductService _ProductService;
        public ProductController(ApiProductService productService)
        {
            _ProductService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductById(int id) {
            confirmMessage respon = await _ProductService.GetProductByIdAsync(id);
           return Ok(respon);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(productForAdd product)
        {
            var respon = await _ProductService.AddProductAsync(product);
            return Ok(respon);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct(product product)
        {
            var respon = await _ProductService.UpdateProductAsync(product);
            return Ok(respon);
        }

        [HttpPost("TaskMethod")]
        public async Task<IActionResult> IfExUpdateProduct(product product)
        {
            confirmMessage respon = await _ProductService.GetProductByIdAsync(product.Id);
            if (respon.Status)
            {
                var Updaterespon = await _ProductService.UpdateProductAsync(product);
                return Ok(Updaterespon);
            }
            else
            {
                productForAdd productForAdd = new productForAdd() { Name = product.Name, Price = product.Price };
                var addRespon = await _ProductService.AddProductAsync(productForAdd);
                return Created("",addRespon);
            }

           
        }

        [HttpPost("AddManyProducts")]
        public async Task<IActionResult> AddManyProducts(List<productForAdd> products)
        {
            NumberOfInsertedProducts respon = await _ProductService.AddManyProducts(products);
            return Created("", respon.NumOfInsertedRows);
        }



        [HttpGet("GetFilterdSortedProducts")]
        public async Task<IActionResult> GetFilterdSortedProducts(ProductCriteriaMsg Criteria)
        {
            List<product> respon = await _ProductService.GetFilterdSortedProducts(Criteria);
            return Ok(respon);
        }

    }
}
