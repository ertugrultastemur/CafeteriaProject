using Business.Abstract;
using Entity.Concrete;
using Entity.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private IBranchService _branchService;

        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        [HttpGet]
        public ActionResult Get(int id)
        {
            var result = _branchService.GetById(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("getall")]
        public ActionResult GetAll()
        {
            var result = _branchService.GetAll();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("add")]
        public ActionResult Add(BranchRequestDto branchRequestDto)
        {
            var result = _branchService.Add(branchRequestDto);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("ad")]
        public ActionResult Ad(BranchRequestDto branch)
        {
            var result = _branchService.AddBranch(branch);

                return Ok(result);

        }

        [HttpPost("update")]
        public ActionResult Update(BranchRequestDto branchRequestDto)
        {
            var result = _branchService.Update(branchRequestDto);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpDelete("delete")]
        public ActionResult Delete(int id)
        {
            var result = _branchService.Delete(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
    }
}
