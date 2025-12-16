using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobRecruitmentSystem.Models
{
    public class CompanyPhoto
    {
        [Key]
        public int PhotoId { get; set; }

        public string FilePath { get; set; }

        // Link to the Employer (Company)
        public int EmployerId { get; set; }

        [ForeignKey("EmployerId")]
        public virtual Employer Employer { get; set; }
    }
}