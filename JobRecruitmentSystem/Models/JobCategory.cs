using System.ComponentModel.DataAnnotations;

namespace JobRecruitmentSystem.Models
{
    public class JobCategory
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; }

        public bool IsActive { get; set; } = true;
    }
}