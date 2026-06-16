using Microsoft.AspNetCore.Identity;

namespace MindConnect.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string RolePreference { get; set; } = "Patient";
    public string? PsychologistSpecialty { get; set; }
    public string? PsychologistBio { get; set; }
    public string? PsychologistAvailableDays { get; set; }
    public string PsychologistAvailableFrom { get; set; } = "09:00";
    public string PsychologistAvailableTo { get; set; } = "17:00";
}
