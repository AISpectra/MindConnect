# MindConnect

MindConnect is a .NET 8 Blazor Server MVP for managing mental health support appointments between patients and psychologists. It was built for a course project submission and focuses on authentication, appointment CRUD, role-based workflows, validation, and a calm responsive UI.

## Features

- ASP.NET Core Identity registration, login, and logout
- Patient and psychologist account roles
- Psychologist registration with specialty, bio, available days, and available hours
- Psychologist directory that combines seeded demo psychologists and registered psychologist accounts
- Patient dashboard with total, upcoming, and history appointment summaries
- Psychologist dashboard showing appointments booked with that psychologist
- Appointment CRUD for patients: create, list, details, edit, cancel, and delete
- Psychologist appointment view: review and cancel patient appointments booked with them
- Appointment history with Completed/Cancelled filtering
- Scheduling validation for psychologist availability, past dates, and duplicate time slots
- Friendly validation and error messages around login, registration, scheduling, and database actions
- SQLite persistence through Entity Framework Core
- Responsive Bootstrap-based design
- Documentation/About page for video demonstration
- Seeded demo patient account and appointments

## Technologies Used

- .NET 8
- Blazor Server interactive components
- ASP.NET Core Identity
- Entity Framework Core
- SQLite
- Bootstrap 5

## How to Run Locally

```bash
dotnet restore
dotnet run
```

Open the displayed local URL, usually `https://localhost:5001` or `http://localhost:5000`.

## Database Notes

The SQLite connection string is configured in `appsettings.json`:

```json
"DefaultConnection": "Data Source=mindconnect.db"
```

On startup, the app creates the SQLite database schema automatically with EF Core so the course demo works without extra setup. The initializer also adds compatibility columns for the psychologist profile fields if an older local database already exists.

For production deployment, use a persistent SQLite file location or replace SQLite with a managed production database such as PostgreSQL.

## Demo User

A demo patient account is seeded when the app starts:

- Email: `demo@mindconnect.local`
- Password: `Password1`

The demo account includes scheduled, completed, and cancelled appointments so the Dashboard, My Appointments, and History pages are easy to demonstrate.

## Role Workflow

### Patient

Patients can:

1. Register or log in.
2. View dashboard appointment summaries.
3. Browse psychologist availability.
4. Schedule appointments only within psychologist availability.
5. View, edit, cancel, delete, and review appointment history.

### Psychologist

Psychologists can:

1. Register with specialty, bio, available days, and available hours.
2. Appear in the psychologist directory.
3. View appointments patients booked with them.
4. Open appointment details.
5. Cancel scheduled appointments when needed.

## CRUD Explanation

The `Appointment` model includes patient name, psychologist name, date, time, reason, status, notes, creation timestamp, and the authenticated patient's user ID.

CRUD is implemented through:

1. **Create**: patients create appointments from Schedule Appointment.
2. **Read**: patients and assigned psychologists view appointments in list and details pages.
3. **Update**: patients edit appointment details from Edit Appointment.
4. **Delete/Cancel**: patients can delete records; patients and assigned psychologists can cancel scheduled appointments.

The application also blocks duplicate appointments for the same psychologist, date, and time.

## Authentication Explanation

MindConnect uses ASP.NET Core Identity with a custom `ApplicationUser` class. Registration captures full name and role preference. Psychologist accounts also store specialty, biography, available days, and available hours.

Login, registration, logout, and appointment creation use HTTP POST endpoints so authentication cookies and redirects work reliably in Blazor Server.

## Deployment Notes

For deployment, publish the app with:

```bash
dotnet publish -c Release
```

Configure production secrets and database storage through the hosting provider. If deploying to Render with SQLite, use persistent disk storage; otherwise use a managed database.

## Future Improvements

- Calendar-style scheduling interface
- Appointment search and pagination
- Email or SMS reminders
- Admin reporting dashboard
- Stronger production database setup with migrations
- More detailed authorization policies for larger deployments

## GitHub Commit Commands

```bash
git add .
git commit -m "Update MindConnect documentation"
git push origin HEAD
```
