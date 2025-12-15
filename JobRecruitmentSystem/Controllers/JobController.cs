using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JobRecruitmentSystem.Data;
using JobRecruitmentSystem.Models;

namespace JobRecruitmentSystem.Controllers
{
    [Authorize(Roles = "Employer")]
    public class JobController : Controller
    {
        private readonly AppDbContext _context;

        public JobController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Job (Manage Jobs)
        public async Task<IActionResult> Index()
        {
            // 1. Get current logged-in user ID
            var userId = int.Parse(User.FindFirst("UserId").Value);

            // 2. Find the Employer Profile ID
            var employer = await _context.Employers.FirstOrDefaultAsync(e => e.UserId == userId);
            if (employer == null) return RedirectToAction("Profile", "Employer");

            // 3. Get jobs for THIS employer only
            var jobs = await _context.JobPosts
                .Include(j => j.JobCategory) // Join with Category table
                .Where(j => j.EmployerId == employer.EmployerId)
                .OrderByDescending(j => j.PostedDate)
                .ToListAsync();

            return View(jobs);
        }

        // GET: Job/Create
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.JobCategories.Where(c => c.IsActive), "Id", "Name");
            return View();
        }

        // POST: Job/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobPost model)
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var employer = await _context.Employers.FirstOrDefaultAsync(e => e.UserId == userId);

            if (employer == null)
            {
                TempData["Error"] = "Please complete your Company Profile first.";
                return RedirectToAction("Profile", "Employer");
            }

            // Manually remove Employer validation because we set it programmatically
            ModelState.Remove("Employer");
            ModelState.Remove("JobCategory");

            if (ModelState.IsValid)
            {
                model.EmployerId = employer.EmployerId;
                model.PostedDate = DateTime.Now;

                _context.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Job created successfully!";
                return RedirectToAction(nameof(Index));
            }

            // Reload dropdown if validation fails
            ViewBag.Categories = new SelectList(_context.JobCategories.Where(c => c.IsActive), "Id", "Name");
            return View(model);
        }

        // GET: Job/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var job = await _context.JobPosts.FindAsync(id);
            if (job == null) return NotFound();

            // Security Check: Ensure this job belongs to the current employer
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var employer = await _context.Employers.FirstOrDefaultAsync(e => e.UserId == userId);
            if (job.EmployerId != employer.EmployerId) return Unauthorized();

            ViewBag.Categories = new SelectList(_context.JobCategories.Where(c => c.IsActive), "Id", "Name", job.JobCategoryId);
            return View(job);
        }

        // POST: Job/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, JobPost model)
        {
            if (id != model.JobPostId) return NotFound();

            ModelState.Remove("Employer");
            ModelState.Remove("JobCategory");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Job updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.JobPosts.Any(e => e.JobPostId == model.JobPostId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(_context.JobCategories.Where(c => c.IsActive), "Id", "Name", model.JobCategoryId);
            return View(model);
        }

        // POST: Job/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var job = await _context.JobPosts.FindAsync(id);
            if (job != null)
            {
                _context.JobPosts.Remove(job);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Job deleted.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}