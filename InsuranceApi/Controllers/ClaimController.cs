using InsuranceApi.DTOs;
using InsuranceApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimController : ControllerBase
    {
        private readonly IClaimService service;

        public ClaimController(IClaimService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            List<ClaimDto> claims = await service.GetAll();
            return Ok(claims);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await service.Delete(id);
                return Ok();
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(ClaimDto claim)
        {
            await service.Add(claim);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(ClaimDto claim)
        {
            try
            {
                await service.Update(claim);
                return Ok();
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var found = await service.GetById(id);
                return Ok(found);
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] ClaimStatusUpdateDto updateDto)
        {
            try
            {
                await service.UpdateStatus(id, updateDto.Status, updateDto.DispenseAmount);
                return Ok();
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetClaimCount()
        {
            try
            {
                var count = await service.GetClaimCount();
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }

    public class ClaimStatusUpdateDto
    {
        public string Status { get; set; }
        public decimal DispenseAmount { get; set; }
    }
}