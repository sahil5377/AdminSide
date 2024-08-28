using InsuranceApi.DTOs;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [EnableCors("AllowSpecificOrigin")]  // Ensure CORS policy is applied
    public class CustomerController : Controller
    {
        private readonly PolicyHolderService _policyHolderService;

        public CustomerController(PolicyHolderService policyHolderService)
        {
            _policyHolderService = policyHolderService;
        }


        [HttpGet]
        public async Task<IActionResult> ManageCustomers(string searchTerm = "")
        {
            List<PolicyHolderDto> policyHolders;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                // Fetch all policy holders if no search term is provided
                policyHolders = await _policyHolderService.GetPolicyHoldersAsync();
            }
            else
            {
                // Fetch policy holders matching the search term
                policyHolders = await _policyHolderService.SearchPolicyHoldersAsync(searchTerm);
            }

            var customerCount = await _policyHolderService.GetCustomerCountAsync();
            TempData["CustomerCount"] = customerCount;
            TempData.Keep("CustomerCount");

            ViewData["SearchTerm"] = searchTerm; // Pass search term to the view
            return View(policyHolders);
        }

        [HttpPut]
        [Route("PolicyHolder/{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] int status)
        {
            try
            {
                await _policyHolderService.UpdateStatusAsync(id, status);
                return Ok();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the status.");
            }
        }
        [HttpGet]
        public async Task<IActionResult> SearchCustomers(string searchTerm = "")
        {
            var searchCustomers = await _policyHolderService.SearchPolicyHoldersAsync(searchTerm);
            return View(searchCustomers);
        }

    }
}