# MindConnect

MindConnect is my .NET 8 Blazor Server project for managing mental health appointments between patients and psychologists.

The main idea is simple: patients can find a psychologist, book an appointment during that psychologist's available hours, and manage their appointments. Psychologists can create a profile with their availability and see the appointments that patients have booked with them.

## What the app does

- Users can register, log in, and log out.
- A user can register as a patient or as a psychologist.
- Psychologists can add their specialty, short bio, available days, and available hours.
- Patients can browse available psychologists.
- Patients can create, view, edit, cancel, and delete appointments.
- Psychologists can view appointments booked with them and cancel them if needed.
- The dashboard changes depending on whether the user is a patient or psychologist.
- The app checks for invalid appointment dates, unavailable psychologist times, and duplicate appointment slots.
- The layout is built with Bootstrap and works on different screen sizes.

## Technologies

- .NET 8
- Blazor Server
- ASP.NET Core Identity
- Entity Framework Core
- SQLite
- Bootstrap 5

## Running the project locally

From the project folder:

```bash
dotnet restore
dotnet run
```

Then open the local URL shown in the terminal. It is usually:

```text
http://localhost:5000
```

## Test account

The app creates a sample patient account when it starts:

```text
Email: sample@mindconnect.local
Password: Password1
```

This account has a few sample appointments so the dashboard, appointment list, and history pages are not empty during testing.

## Patient workflow

A patient can:

1. Register or log in.
2. View the dashboard.
3. Open the psychologist directory.
4. Create an appointment with an available psychologist.
5. Edit, cancel, delete, or review appointments.

## Psychologist workflow

A psychologist can:

1. Register with a specialty, bio, days, and hours.
2. Appear in the psychologist directory.
3. View appointments that patients booked with them.
4. Cancel scheduled appointments when needed.

## Data storage

The app uses SQLite through Entity Framework Core. The connection string is in `appsettings.json`:

```json
"DefaultConnection": "Data Source=mindconnect.db"
```

The database file is created locally when the app runs.

## Deployment

The project includes a `Dockerfile` for deployment on Render. The container runs the published Blazor app and binds to the port provided by Render.

For a real production version, I would replace the local SQLite setup with a managed database such as PostgreSQL, or configure persistent disk storage.

## Possible future improvements

- Calendar-style appointment selection
- Email reminders
- Appointment search
- Admin dashboard
- More detailed authorization rules
