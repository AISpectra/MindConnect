# MindConnect

MindConnect is a .NET 8 Blazor Server MVP for managing mental health support appointments between patients and psychologists. It was built for a course project submission and focuses on clear CRUD functionality, authentication, validation, and a calm responsive UI.

## Features

- ASP.NET Core Identity registration, login, and logout
- Demo-friendly profile role selector during registration
- Dashboard with total, upcoming, and history appointment summaries
- Appointment CRUD: create, list, details, edit, cancel, and delete
- Appointment history with Completed/Cancelled filtering
- Static psychologist availability page with schedule links
- Friendly validation and error messages around form and database actions
- SQLite persistence through Entity Framework Core
- Responsive Bootstrap-based design
- Documentation/About page for video demonstration
- Seeded demo user and appointments

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
dotnet tool restore # optional, only if your environment uses a tool manifest
dotnet ef database update # optional for migrations; the app also creates the SQLite database automatically on startup
dotnet run
```

Open the displayed local URL, usually `https://localhost:5001` or `http://localhost:5000`.

## Database and Migration Notes

The SQLite connection string is configured in `appsettings.json` as `Data Source=mindconnect.db`. On startup, the app creates the database schema automatically with EF Core so the course demo works without extra setup.

If you want to manage migrations explicitly, install the EF tool and create/apply a migration:

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Demo User

A demo account is seeded when the app starts:

- Email: `demo@mindconnect.local`
- Password: `Password1`

The demo account includes scheduled, completed, and cancelled appointments so the Dashboard, My Appointments, and History pages are easy to demonstrate.

## CRUD Explanation

The `Appointment` model includes patient name, psychologist name, date, time, reason, status, notes, creation timestamp, and the authenticated user's ID. Users can:

1. Create an appointment from Schedule Appointment.
2. Read appointments in My Appointments and Appointment Details.
3. Update appointment details from Edit Appointment.
4. Cancel or delete appointments from the details page.
5. Review completed/cancelled appointments from Appointment History.

## Authentication Explanation

MindConnect uses ASP.NET Core Identity with a custom `ApplicationUser` class. Registration captures full name and a simple role preference for demo purposes. Appointment queries are scoped to the authenticated user's ID so users only see their own appointments.

## Deployment Notes

For deployment, publish the app with:

```bash
dotnet publish -c Release
```

Configure a persistent SQLite file location or replace SQLite with a managed production database. Set production secrets and HTTPS settings through the hosting provider.

## Future Improvements

- Real psychologist accounts and role-based authorization
- Calendar availability conflict checks
- Email or SMS reminders
- Appointment search and pagination
- Admin reporting dashboard
- Production deployment pipeline

## GitHub Commit Commands

```bash
git add .
git commit -m "Build MindConnect Blazor MVP"
git push origin HEAD
```
