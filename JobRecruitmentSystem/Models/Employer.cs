using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobRecruitmentSystem.Models
{
    public class Employer
    {
        [Key]
        public int EmployerId { get; set; }

        // Link to the Login Account
        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required(ErrorMessage = "Company Name is required")]
        [StringLength(100)]
        public string CompanyName { get; set; }

        public string CompanyDescription { get; set; }

        public string WebsiteUrl { get; set; }

        public string Location { get; set; } // Address for Maps Integration later

        public string LogoPath { get; set; } // Path to uploaded logo image
    }
}