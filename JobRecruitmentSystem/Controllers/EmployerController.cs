using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobRecruitmentSystem.Data;
using JobRecruitmentSystem.Models;

namespace JobRecruitmentSystem.Controllers
{
    [Authorize(Roles = "Employer")] // Secure: Only Employers can enter
    public class EmployerController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmployerController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Employer/Profile
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var employer = await _context.Employers.FirstOrDefaultAsync(e => e.UserId == userId);

            if (employer == null)
            {
                // Create a placeholder if it's their first time
                employer = new Employer { UserId = userId };
            }

            return View(employer);
        }

        // POST: Employer/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(Employer model, IFormFile logoFile)
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            model.UserId = userId;

            // Handle Logo Upload
            if (logoFile != null && logoFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "logos");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + logoFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await logoFile.CopyToAsync(fileStream);
                }

                model.LogoPath = "/logos/" + uniqueFileName;
            }
            else
            {
                // Keep existing logo if not changing
                var existing = await _context.Employers.AsNoTracking().FirstOrDefaultAsync(e => e.EmployerId == model.EmployerId);
                if (existing != null) model.LogoPath = existing.LogoPath;
            }

            if (ModelState.IsValid)
            {
                if (model.EmployerId == 0)
                    _context.Add(model);
                else
                    _context.Update(model);

                await _context.SaveChangesAsync();
                TempData["Success"] = "Company Profile updated!";
                return RedirectToAction("Profile");
            }

            return View(model);
        }
    }
}