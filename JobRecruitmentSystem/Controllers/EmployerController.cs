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

        // --- COMPANY PHOTOS MANAGEMENT ---

        [HttpPost]
        public async Task<IActionResult> UploadPhotos(List<IFormFile> photos)
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var employer = _context.Employers.FirstOrDefault(e => e.UserId == userId);

            if (employer != null && photos != null)
            {
                foreach (var file in photos)
                {
                    if (file.Length > 0)
                    {
                        // Save file logic (similar to your Logo logic)
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string path = Path.Combine(_webHostEnvironment.WebRootPath, "company_photos", fileName);

                        // Ensure directory exists
                        Directory.CreateDirectory(Path.GetDirectoryName(path));

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Save to DB
                        var photo = new CompanyPhoto
                        {
                            EmployerId = employer.EmployerId,
                            FilePath = "/company_photos/" + fileName
                        };
                        _context.CompanyPhotos.Add(photo);
                    }
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Profile");
        }

        // --- STAFF MANAGEMENT (Sub-Users) ---

        // GET: Employer/ManageStaff
        public IActionResult ManageStaff()
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            // Find the current employer ID
            var employer = _context.Employers.FirstOrDefault(e => e.UserId == userId);

            if (employer == null) return RedirectToAction("Profile");

            // Get all users linked to this Employer
            var staff = _context.Users.Where(u => u.EmployerId == employer.EmployerId).ToList();
            return View(staff);
        }

        // POST: Employer/CreateStaff
        [HttpPost]
        public IActionResult CreateStaff(User newStaff)
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var employer = _context.Employers.FirstOrDefault(e => e.UserId == userId);

            if (employer != null && ModelState.IsValid)
            {
                // Link new user to this company
                newStaff.EmployerId = employer.EmployerId;
                newStaff.Role = "Employer"; // Or specific role like "HiringManager"
                newStaff.PasswordHash = "12345"; // simplified; use your PasswordHasher here

                _context.Users.Add(newStaff);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageStaff");
        }
    }
}