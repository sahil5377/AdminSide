using InsuranceApi.Data;
using InsuranceApi.DTOs;
using InsuranceApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InsuranceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PolicyHolderController : ControllerBase
    {
        private readonly IPolicyHolderService _service;

        public PolicyHolderController(IPolicyHolderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            List<PolicyHolderDto> policyHolderDtos = await _service.GetAll();
            return Ok(policyHolderDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var found = await _service.GetById(id);
                return Ok(found);
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(PolicyHolderDto policyHolderDto)
        {
            await _service.Add(policyHolderDto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(PolicyHolderDto policyHolderDto)
        {
            try
            {
                await _service.Update(policyHolderDto);
                return Ok();
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.Delete(id);
                return Ok();
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] int status)
        {
            try
            {
                await _service.UpdateStatus(id, status);
                return Ok();
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }
    }
}