using Microsoft.AspNetCore.Mvc.Rendering;

namespace JobRecruitmentSystem.ViewModels
{
    public class JobSearchViewModel
    {
        // Search Inputs
        public string Keyword { get; set; }
        public int? CategoryId { get; set; }
        public string Location { get; set; }

        // Dropdown Data
        public SelectList Categories { get; set; }

        // Results
        public IEnumerable<Models.JobPost> Results { get; set; }
    }
}