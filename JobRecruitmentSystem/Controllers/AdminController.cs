using JobRecruitmentSystem.Data;
using JobRecruitmentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace JobRecruitmentSystem.Controllers
{
    // 1. Protect this entire Controller. Only "Admin" can access.
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard(string searchString, string sortOrder, string filterRole, int pageNumber = 1)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentRole"] = filterRole; // New Filter

            var users = from u in _context.Users select u;

            // 1. Filter Logic (Search Box)
            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(s => s.Username.Contains(searchString) || s.Email.Contains(searchString));
            }

            // 2. Dropdown Filter Logic (New Requirement)
            if (!string.IsNullOrEmpty(filterRole) && filterRole != "All")
            {
                users = users.Where(u => u.Role == filterRole);
            }

            // 3. Sorting Logic
            switch (sortOrder)
            {
                case "name_desc": users = users.OrderByDescending(s => s.Username); break;
                case "Date": users = users.OrderBy(s => s.CreatedAt); break;
                case "Role": users = users.OrderBy(s => s.Role); break;
                default: users = users.OrderBy(s => s.Username); break;
            }

            // Paging (Simulated for real-time feel)
            var pagedUsers = await users.Take(50).ToListAsync(); // Limit to 50 for speed

            // CRITICAL: Check if this is an AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_UserTable", pagedUsers);
            }

            return View(pagedUsers);
        }

        // POST: Admin/ToggleStatus
        // 3. Block or Unblock a User
        [HttpPost]
        public IActionResult ToggleStatus(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            // Flip the status (True -> False, False -> True)
            user.IsActive = !user.IsActive;
            _context.SaveChanges();

            return RedirectToAction(nameof(Dashboard));
        }

        // GET: Admin/Edit/5
        public IActionResult Edit(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        public IActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                // Retrieve original user to keep PasswordHash intact
                var existingUser = _context.Users.Find(user.UserId);
                if (existingUser == null) return NotFound();

                // Update allowed fields
                existingUser.Username = user.Username;
                existingUser.Email = user.Email;
                existingUser.Role = user.Role; // Admin can change roles

                _context.SaveChanges();
                TempData["Success"] = "User updated successfully!";
                return RedirectToAction(nameof(Dashboard));
            }
            return View(user);
        }

        // --- CATEGORY MAINTENANCE ---

        // GET: Admin/Categories
        public IActionResult Categories()
        {
            var categories = _context.JobCategories.ToList();
            return View(categories);
        }

        // POST: Admin/CreateCategory
        [HttpPost]
        public IActionResult CreateCategory(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                var category = new JobCategory { Name = name, IsActive = true };
                _context.JobCategories.Add(category);
                _context.SaveChanges();
                TempData["Success"] = "Category added successfully!";
            }
            else
            {
                TempData["Error"] = "Category name cannot be empty.";
            }
            return RedirectToAction(nameof(Categories));
        }

        // POST: Admin/DeleteCategory
        [HttpPost]
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.JobCategories.Find(id);
            if (category != null)
            {
                _context.JobCategories.Remove(category);
                _context.SaveChanges();
                TempData["Success"] = "Category deleted.";
            }
            return RedirectToAction(nameof(Categories));
        }

        // POST: Admin/ToggleCategoryStatus (AJAX Example)
        [HttpPost]
        public IActionResult ToggleCategoryStatus(int id)
        {
            var category = _context.JobCategories.Find(id);
            if (category != null)
            {
                category.IsActive = !category.IsActive;
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Categories));
        }
    }
}