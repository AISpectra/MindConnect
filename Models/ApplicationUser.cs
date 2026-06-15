using Microsoft.AspNetCore.Identity;

namespace MindConnect.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string RolePreference { get; set; } = "Patient";
}
