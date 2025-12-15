using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobRecruitmentSystem.Models
{
    public class JobPost
    {
        [Key]
        public int JobPostId { get; set; }

        [Required]
        public int EmployerId { get; set; }

        [ForeignKey("EmployerId")]
        public virtual Employer Employer { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        public int JobCategoryId { get; set; }

        [ForeignKey("JobCategoryId")]
        public virtual JobCategory JobCategory { get; set; }

        [Required(ErrorMessage = "Job Title is required")]
        [StringLength(100)]
        public string JobTitle { get; set; }

        [Required]
        public string JobDescription { get; set; }

        [Required]
        public string JobLocation { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Salary must be positive")]
        [Column(TypeName = "decimal(18, 2)")] // Stores 18 digits total, 2 decimal places
        public decimal Salary { get; set; }

        [Required]
        public string EmploymentType { get; set; } // Full-time, Part-time, Contract

        [Required]
        public string Status { get; set; } = "Draft"; // Draft, Active, Closed

        public DateTime PostedDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime ClosingDate { get; set; }
    }
}