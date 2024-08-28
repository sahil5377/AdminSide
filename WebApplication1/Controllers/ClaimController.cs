using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;
using InsuranceApi.DTOs;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class ClaimController : Controller
    {
        private readonly IClaimService _claimService;

        public ClaimController(IClaimService claimService)
        {
            _claimService = claimService;
        }

        public async Task<IActionResult> ManageClaims()
        {
            var claims = await _claimService.GetAllClaimsAsync();
            var claimCount = await _claimService.GetClaimCountAsync();
            TempData["ClaimCount"] = claimCount;  // Pass the count to the view
            TempData.Keep("ClaimCount");
            return View(claims);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateClaimStatus(int claimId, String status, decimal dispenseAmount)
        {
            await _claimService.UpdateClaimStatusAsync(claimId, status, dispenseAmount);
            return RedirectToAction("ManageClaims");
        }

    }
}