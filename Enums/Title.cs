using System.ComponentModel.DataAnnotations;

namespace PassiveApi.Models
{
    public enum Title
    {
        [Display(Name = "Other")] Other,
        [Display(Name = "President")] President,
        [Display(Name = "Vice President")] VicePresident,
        [Display(Name = "Treasurer")] Treasurer,
        [Display(Name = "Advisor")] Advisor,
        [Display(Name = "Council Member")] CouncilMember,
        [Display(Name = "Member")] Member,

    }
}
