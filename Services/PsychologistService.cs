using MindConnect.Models;

namespace MindConnect.Services;

public class PsychologistService
{
    private static readonly IReadOnlyList<Psychologist> Psychologists = new List<Psychologist>
    {
        new("Dr. Emily Carter", "Anxiety and Stress", "Monday, Wednesday, Friday", "Specializes in practical coping strategies for anxiety and workplace stress."),
        new("Dr. Michael Smith", "Depression and Emotional Support", "Tuesday, Thursday", "Supports patients through mood concerns, grief, and emotional resilience."),
        new("Dr. Sofia Martinez", "Family and Relationship Therapy", "Monday, Tuesday, Thursday", "Helps patients improve communication and relationship wellness."),
        new("Dr. David Johnson", "General Mental Health Support", "Wednesday, Friday, Saturday", "Provides broad mental health support and wellness planning.")
    };

    public IReadOnlyList<Psychologist> GetPsychologists() => Psychologists;
}
