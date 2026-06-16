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
        await EnsureApplicationUserProfileColumnsAsync(db);

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        const string email = "sample@mindconnect.local";
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true, FullName = "Sample Patient", RolePreference = "Patient" };
            await userManager.CreateAsync(user, "Password1");
        }

        if (!await db.Appointments.AnyAsync())
        {
            db.Appointments.AddRange(
                new Appointment { PatientName = "Sample Patient", PsychologistName = "Dr. Emily Carter", AppointmentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)), AppointmentTime = new TimeOnly(10, 0), Reason = "Anxiety and stress check-in", Status = AppointmentStatus.Scheduled, Notes = "Prepare questions about coping strategies.", UserId = user.Id },
                new Appointment { PatientName = "Sample Patient", PsychologistName = "Dr. Michael Smith", AppointmentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)), AppointmentTime = new TimeOnly(14, 30), Reason = "Emotional support follow-up", Status = AppointmentStatus.Completed, Notes = "Completed appointment.", UserId = user.Id },
                new Appointment { PatientName = "Sample Patient", PsychologistName = "Dr. Sofia Martinez", AppointmentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-4)), AppointmentTime = new TimeOnly(9, 30), Reason = "Relationship counseling", Status = AppointmentStatus.Cancelled, Notes = "Cancelled due to schedule conflict.", UserId = user.Id });
            await db.SaveChangesAsync();
        }
    }

    private static async Task EnsureApplicationUserProfileColumnsAsync(ApplicationDbContext db)
    {
        var connection = db.Database.GetDbConnection();
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA table_info('AspNetUsers')";
        var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        await using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                columns.Add(reader.GetString(1));
            }
        }

        foreach (var (column, definition) in new[]
        {
            ("PsychologistSpecialty", "TEXT NULL"),
            ("PsychologistBio", "TEXT NULL"),
            ("PsychologistAvailableDays", "TEXT NULL"),
            ("PsychologistAvailableFrom", "TEXT NOT NULL DEFAULT '09:00'"),
            ("PsychologistAvailableTo", "TEXT NOT NULL DEFAULT '17:00'")
        })
        {
            if (!columns.Contains(column))
            {
                await using var alterCommand = connection.CreateCommand();
                alterCommand.CommandText = $"""ALTER TABLE "AspNetUsers" ADD COLUMN "{column}" {definition}""";
                await alterCommand.ExecuteNonQueryAsync();
            }
        }
    }
}
