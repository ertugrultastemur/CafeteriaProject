using Business.Abstract;
using Entity.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public ActionResult Get(int id)
        {
            var result = _productService.GetById(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("getall")]
        public ActionResult GetAll()
        {
            var result = _productService.GetAll();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("add")]
        public ActionResult Add([FromForm] ProductRequestDto productRequestDto, [FromForm] IFormFile image)
        {
            var result = _productService.Add(productRequestDto, image);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("update")]
        public ActionResult Update([FromForm] ProductRequestDto productRequestDto, [FromForm] IFormFile image)
        {
            var result = _productService.Update(productRequestDto, image);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpDelete("delete")]
        public ActionResult Delete(int id)
        {
            var result = _productService.Delete(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
    }
}
