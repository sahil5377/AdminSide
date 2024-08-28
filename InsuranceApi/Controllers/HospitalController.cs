using InsuranceApi.Data;
using InsuranceApi.DTOs;
using InsuranceApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApi.Controllers
{
    public interface IHospitalController
    {
        Task<IActionResult> Add(HospitalDto hospital);
        Task<IActionResult> Delete(int id);
        Task<IActionResult> GetAll();
        Task<IActionResult> GetById(int id);
        Task<IActionResult> Update(HospitalDto hospital);
        Task<ActionResult<int>> GetHospitalCount();
    }

    [Route("api/[controller]")]
    [ApiController]
    public class HospitalController : ControllerBase, IHospitalController
    {
        private readonly IHospitalService service;
        public HospitalController(IHospitalService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            List<HospitalDto> hospitals = await service.GetAll();
            return Ok(hospitals);
        }

        [HttpDelete]
        [Route("{id}")]
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
        public async Task<IActionResult> Add(HospitalDto hospital)
        {
            await service.Add(hospital);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(HospitalDto hospital)
        {
            try
            {
                await service.Update(hospital);
                return Ok();
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("{id}")]
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
        [HttpGet("count")]
        public async Task<ActionResult<int>>GetHospitalCount()
        {
            try
            {
                var count = await service.GetHospitalCount();
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string term)
        {
            try
            {
                var hospitals = await service.SearchHospitalAsync(term);
                return Ok(hospitals);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}