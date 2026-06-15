using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MindConnect.Models;

namespace MindConnect.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        await db.Database.EnsureCreatedAsync();

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        const string email = "demo@mindconnect.local";
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true, FullName = "Demo Patient", RolePreference = "Patient" };
            await userManager.CreateAsync(user, "Password1");
        }

        if (!await db.Appointments.AnyAsync())
        {
            db.Appointments.AddRange(
                new Appointment { PatientName = "Demo Patient", PsychologistName = "Dr. Emily Carter", AppointmentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)), AppointmentTime = new TimeOnly(10, 0), Reason = "Anxiety and stress check-in", Status = AppointmentStatus.Scheduled, Notes = "Prepare questions about coping strategies.", UserId = user.Id },
                new Appointment { PatientName = "Demo Patient", PsychologistName = "Dr. Michael Smith", AppointmentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)), AppointmentTime = new TimeOnly(14, 30), Reason = "Emotional support follow-up", Status = AppointmentStatus.Completed, Notes = "Completed demo appointment.", UserId = user.Id },
                new Appointment { PatientName = "Demo Patient", PsychologistName = "Dr. Sofia Martinez", AppointmentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-4)), AppointmentTime = new TimeOnly(9, 30), Reason = "Relationship counseling", Status = AppointmentStatus.Cancelled, Notes = "Cancelled due to schedule conflict.", UserId = user.Id });
            await db.SaveChangesAsync();
        }
    }
}
