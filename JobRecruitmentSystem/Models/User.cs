using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobRecruitmentSystem.Models
{
    public class User
    {
        // Primary Key
        [Key]
        public int UserId { get; set; }

        // Basic Login Info
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        // Note: In a real app, we store Hashed passwords, not plain text. 
        // We will handle hashing in the Controller later.
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(20)]
        // Role stores: "Admin", "Employer", or "JobSeeker"
        public string Role { get; set; }

        // Status for "Block/Unblock" features [cite: 196]
        public bool IsActive { get; set; } = true;

        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}