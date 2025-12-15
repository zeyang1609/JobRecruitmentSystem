using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobRecruitmentSystem.Models
{
    public class JobSeeker
    {
        [Key]
        public int JobSeekerId { get; set; }

        // Foreign Key linking to the User Login Table
        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100)]
        public string FullName { get; set; }

        [StringLength(20)]
        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        // We will store the file path of the uploaded resume
        public string ResumeFilePath { get; set; }

        // Helper to store "Java, C#, Python" as a simple string for now
        public string Skills { get; set; }

        public string Education { get; set; }

        public string Experience { get; set; }
    }
}