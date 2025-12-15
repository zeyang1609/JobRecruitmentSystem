using JobRecruitmentSystem.Data;
using JobRecruitmentSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace JobRecruitmentSystem.Controllers
{
    public class JobListingController : Controller
    {
        private readonly AppDbContext _context;

        public JobListingController(AppDbContext context)
        {
            _context = context;
        }

        // GET: JobListing (Search Page)
        public async Task<IActionResult> Index(string keyword, int? categoryId, string location)
        {
            // 1. Start with ALL Active jobs
            var query = _context.JobPosts
                .Include(j => j.Employer)       // Get Company Name
                .Include(j => j.JobCategory)    // Get Category Name
                .Where(j => j.Status == "Active")
                .AsQueryable();

            // 2. Apply Filters if provided
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(j => j.JobTitle.Contains(keyword) || j.JobDescription.Contains(keyword));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(j => j.JobCategoryId == categoryId);
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                query = query.Where(j => j.JobLocation.Contains(location));
            }

            // 3. Prepare ViewModel
            var model = new JobSearchViewModel
            {
                Keyword = keyword,
                CategoryId = categoryId,
                Location = location,
                Categories = new SelectList(await _context.JobCategories.Where(c => c.IsActive).ToListAsync(), "Id", "Name"),
                Results = await query.OrderByDescending(j => j.PostedDate).ToListAsync()
            };

            return View(model);
        }
        public async Task<IActionResult> FilterJobs(string keyword, int? categoryId, string location)
        {
            // Reuse the same query logic
            var query = _context.JobPosts
                .Include(j => j.Employer)
                .Include(j => j.JobCategory)
                .Where(j => j.Status == "Active")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(j => j.JobTitle.Contains(keyword) || j.JobDescription.Contains(keyword));

            if (categoryId.HasValue)
                query = query.Where(j => j.JobCategoryId == categoryId);

            if (!string.IsNullOrWhiteSpace(location))
                query = query.Where(j => j.JobLocation.Contains(location));

            var results = await query.OrderByDescending(j => j.PostedDate).ToListAsync();

            // RETURN PARTIAL VIEW instead of full View
            return PartialView("_JobPostList", results);
        }

        // GET: JobListing/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var job = await _context.JobPosts
                .Include(j => j.Employer)
                .Include(j => j.JobCategory)
                .FirstOrDefaultAsync(m => m.JobPostId == id);

            if (job == null) return NotFound();

            return View(job);
        }
    }
}