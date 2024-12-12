using Business.Abstract;
using Entity.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MunicipalityController : ControllerBase
    {

        private IMunicipalityService _municipalityService;

        public MunicipalityController(IMunicipalityService municipalityService)
        {
            _municipalityService = municipalityService;
        }

        [HttpGet]
        public ActionResult Get(int id)
        {
            var result = _municipalityService.GetById(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("getall")]
        public ActionResult GetAll()
        {
            var result = _municipalityService.GetAll();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("add")]
        public ActionResult Add(MunicipalityRequestDto municipalityRequestDto)
        {
            var result = _municipalityService.Add(municipalityRequestDto);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("update")]
        public ActionResult Update(MunicipalityRequestDto municipalityRequestDto)
        {
            var result = _municipalityService.Update(municipalityRequestDto);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpDelete("delete")]
        public ActionResult Delete(int id)
        {
            var result = _municipalityService.Delete(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
    }
}
