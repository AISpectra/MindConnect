using System.ComponentModel.DataAnnotations;

namespace MindConnect.Models;

public class Appointment : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Patient name is required.")]
    [StringLength(100)]
    public string PatientName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Psychologist name is required.")]
    [StringLength(100)]
    public string PsychologistName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Appointment date is required.")]
    [DataType(DataType.Date)]
    public DateOnly AppointmentDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required(ErrorMessage = "Appointment time is required.")]
    [DataType(DataType.Time)]
    public TimeOnly AppointmentTime { get; set; } = new(9, 0);

    [Required(ErrorMessage = "Reason is required.")]
    [StringLength(500)]
    public string Reason { get; set; } = string.Empty;

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

    [StringLength(1000)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Status == AppointmentStatus.Scheduled && AppointmentDate < DateOnly.FromDateTime(DateTime.Today))
        {
            yield return new ValidationResult("Scheduled appointments cannot be created in the past.", new[] { nameof(AppointmentDate) });
        }
    }
}
