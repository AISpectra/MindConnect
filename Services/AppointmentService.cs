using Microsoft.EntityFrameworkCore;
using MindConnect.Data;
using MindConnect.Models;

namespace MindConnect.Services;

public class AppointmentService(ApplicationDbContext db)
{
    public async Task<List<Appointment>> GetForUserAsync(string userId) =>
        await db.Appointments.Where(a => a.UserId == userId).OrderByDescending(a => a.AppointmentDate).ThenBy(a => a.AppointmentTime).ToListAsync();

    public async Task<List<Appointment>> GetForPsychologistAsync(string psychologistName) =>
        await db.Appointments
            .Where(a => a.PsychologistName == psychologistName)
            .OrderByDescending(a => a.AppointmentDate)
            .ThenBy(a => a.AppointmentTime)
            .ToListAsync();

    public async Task<Appointment?> GetByIdAsync(int id, string userId) =>
        await db.Appointments.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

    public async Task<Appointment?> GetForParticipantAsync(int id, string userId, string? psychologistName) =>
        await db.Appointments.FirstOrDefaultAsync(a =>
            a.Id == id &&
            (a.UserId == userId || (!string.IsNullOrWhiteSpace(psychologistName) && a.PsychologistName == psychologistName)));

    public async Task<bool> HasPsychologistConflictAsync(Appointment appointment) =>
        await db.Appointments.AnyAsync(a =>
            a.Id != appointment.Id &&
            a.Status == AppointmentStatus.Scheduled &&
            a.PsychologistName == appointment.PsychologistName &&
            a.AppointmentDate == appointment.AppointmentDate &&
            a.AppointmentTime == appointment.AppointmentTime);

    public async Task AddAsync(Appointment appointment)
    {
        db.Appointments.Add(appointment);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Appointment appointment)
    {
        db.Appointments.Update(appointment);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Appointment appointment)
    {
        db.Appointments.Remove(appointment);
        await db.SaveChangesAsync();
    }
}
