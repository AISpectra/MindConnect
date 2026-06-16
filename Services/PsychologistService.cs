using MindConnect.Models;
using MindConnect.Data;
using Microsoft.EntityFrameworkCore;

namespace MindConnect.Services;

public class PsychologistService(ApplicationDbContext db)
{
    private static readonly IReadOnlyList<Psychologist> Psychologists = new List<Psychologist>
    {
        new("Dr. Emily Carter", "Anxiety and Stress", "Monday, Wednesday, Friday", "09:00", "17:00", "Specializes in practical coping strategies for anxiety and workplace stress."),
        new("Dr. Michael Smith", "Depression and Emotional Support", "Tuesday, Thursday", "10:00", "18:00", "Supports patients through mood concerns, grief, and emotional resilience."),
        new("Dr. Sofia Martinez", "Family and Relationship Therapy", "Monday, Tuesday, Thursday", "08:30", "15:30", "Helps patients improve communication and relationship wellness."),
        new("Dr. David Johnson", "General Mental Health Support", "Wednesday, Friday, Saturday", "11:00", "19:00", "Provides broad mental health support and wellness planning.")
    };

    public async Task<IReadOnlyList<Psychologist>> GetPsychologistsAsync()
    {
        var registeredPsychologists = await db.Users
            .Where(u => u.RolePreference == "Psychologist")
            .Select(u => new Psychologist(
                !string.IsNullOrWhiteSpace(u.FullName) ? u.FullName : u.Email!,
                u.PsychologistSpecialty ?? "General Mental Health Support",
                u.PsychologistAvailableDays ?? "Monday, Tuesday, Wednesday, Thursday, Friday",
                u.PsychologistAvailableFrom,
                u.PsychologistAvailableTo,
                u.PsychologistBio ?? "Registered MindConnect psychologist."))
            .ToListAsync();

        return Psychologists.Concat(registeredPsychologists).ToList();
    }
}
