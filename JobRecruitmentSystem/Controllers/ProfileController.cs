using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobRecruitmentSystem.Data;
using JobRecruitmentSystem.Models;
using System.Security.Claims;

namespace JobRecruitmentSystem.Controllers
{
    [Authorize(Roles = "JobSeeker")] // Only Job Seekers can access this
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment; // For file uploads

        public ProfileController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Profile/Edit
        public async Task<IActionResult> Edit()
        {
            // 1. Get the current logged-in User ID
            var userId = int.Parse(User.FindFirst("UserId").Value);

            // 2. Try to find an existing profile
            var profile = await _context.JobSeekers.FirstOrDefaultAsync(j => j.UserId == userId);

            // 3. If no profile exists, create a blank one for the View
            if (profile == null)
            {
                profile = new JobSeeker { UserId = userId };
            }

            return View(profile);
        }

        // POST: Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(JobSeeker model, IFormFile resumeFile)
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            model.UserId = userId; // Ensure we don't accidentally save for someone else

            // Handle File Upload (Resume)
            if (resumeFile != null && resumeFile.Length > 0)
            {
                // Create "uploads" folder if it doesn't exist
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                // Create unique filename
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + resumeFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await resumeFile.CopyToAsync(fileStream);
                }

                model.ResumeFilePath = "/uploads/" + uniqueFileName;
            }
            else
            {
                // Keep old resume if no new one uploaded
                var existingProfile = await _context.JobSeekers.AsNoTracking().FirstOrDefaultAsync(j => j.JobSeekerId == model.JobSeekerId);
                if (existingProfile != null) model.ResumeFilePath = existingProfile.ResumeFilePath;
            }

            if (ModelState.IsValid)
            {
                if (model.JobSeekerId == 0)
                {
                    _context.Add(model); // Create new
                }
                else
                {
                    _context.Update(model); // Update existing
                }
                await _context.SaveChangesAsync();
                TempData["Success"] = "Profile saved successfully!";
                return RedirectToAction("Edit");
            }
            return View(model);
        }
    }
}