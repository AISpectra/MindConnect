using Microsoft.EntityFrameworkCore;
using MindConnect.Data;
using MindConnect.Models;

namespace MindConnect.Services;

public class AppointmentService(ApplicationDbContext db)
{
    public async Task<List<Appointment>> GetForUserAsync(string userId) =>
        await db.Appointments.Where(a => a.UserId == userId).OrderByDescending(a => a.AppointmentDate).ThenBy(a => a.AppointmentTime).ToListAsync();

    public async Task<Appointment?> GetByIdAsync(int id, string userId) =>
        await db.Appointments.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

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
