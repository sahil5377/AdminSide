using InsuranceApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class HospitalController : Controller
    {
        private readonly IHospitalService _hospitalService;

        public HospitalController(IHospitalService hospitalService)
        {
            _hospitalService = hospitalService;
        }

        [HttpGet]
        public async Task<IActionResult> ManageHospitals(string searchTerm = "")
        {
            List<HospitalDto> hospitals;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                // Fetch all hospitals if no search term is provided
                hospitals = await _hospitalService.GetAllHospitalsAsync();
            }
            else
            {
                // Fetch hospitals matching the search term
                hospitals = await _hospitalService.SearchHospitalsAsync(searchTerm);
            }

            var hospitalCount = await _hospitalService.GetHospitalCountAsync();
            TempData["HospitalCount"] = hospitalCount;
            TempData.Keep("HospitalCount");

            ViewData["SearchTerm"] = searchTerm; // Pass search term to the view
            return View(hospitals);
        }

        [HttpPost]
        public async Task<IActionResult> AddHospital(HospitalDto hospital)
        {
            if (ModelState.IsValid)
            {
                await _hospitalService.AddHospitalAsync(hospital);
                return RedirectToAction("ManageHospitals");
            }
            return View("ManageHospitals", await _hospitalService.GetAllHospitalsAsync());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateHospital(HospitalDto hospital)
        {
            if (ModelState.IsValid)
            {
                await _hospitalService.UpdateHospitalAsync(hospital);
                return RedirectToAction("ManageHospitals");
            }
            return View("ManageHospitals", await _hospitalService.GetAllHospitalsAsync());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteHospital(int hospitalId)
        {
            await _hospitalService.DeleteHospitalAsync(hospitalId);
            return RedirectToAction("ManageHospitals");
        }
    }
}