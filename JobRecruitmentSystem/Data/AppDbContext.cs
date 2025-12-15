using JobRecruitmentSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace JobRecruitmentSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Define your tables here
        public DbSet<User> Users { get; set; }
        public DbSet<JobSeeker> JobSeekers { get; set; }
        public DbSet<Employer> Employers { get; set; }
        public DbSet<JobCategory> JobCategories { get; set; }
        public DbSet<JobPost> JobPosts { get; set; }
        // public DbSet<Application> Applications { get; set; }
    }
}