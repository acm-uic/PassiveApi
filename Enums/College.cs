using System.ComponentModel.DataAnnotations;

namespace PassiveApi.Models
{
    public enum College
    {
        [Display(Name = "Other")] Other,
        [Display(Name = "Applied Health Sciences")] AppliedHealthSciences,
        [Display(Name = "Architecture, Design, Arts")] ArchitectureDesignArts,
        [Display(Name = "Business Administration")] BusinessAdministration,
        [Display(Name = "Education")] Education,
        [Display(Name = "Engineering")] Engineering,
        [Display(Name = "Liberal Arts and Sciences")] LiberalArtsandSciences,
        [Display(Name = "Nursing")] Nursing,
        [Display(Name = "Pharmacy")] Pharmacy,
        [Display(Name = "Public Health")] PublicHealth,
        [Display(Name = "Urban Planning and Public Affairs")] UrbanPlanningandPublicAffairs,
    }
}
