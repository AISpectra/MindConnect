using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MindConnect.Components;
using MindConnect.Data;
using MindConnect.Models;
using MindConnect.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddScoped<AppointmentService>();
builder.Services.AddScoped<PsychologistService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=mindconnect.db";
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";
    options.AccessDeniedPath = "/account/access-denied";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    await DbInitializer.InitializeAsync(scope.ServiceProvider);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.MapPost("/account/login-submit", async (
    HttpRequest request,
    SignInManager<ApplicationUser> signInManager) =>
{
    var form = await request.ReadFormAsync();
    var email = form["email"].ToString();
    var password = form["password"].ToString();
    var returnUrl = NormalizeReturnUrl(request.Query["returnUrl"].ToString());

    var result = await signInManager.PasswordSignInAsync(email, password, isPersistent: true, lockoutOnFailure: false);
    return result.Succeeded
        ? Results.Redirect(returnUrl)
        : Results.Redirect($"/account/login?error={Uri.EscapeDataString("Invalid email or password.")}&returnUrl={Uri.EscapeDataString(returnUrl)}");
}).DisableAntiforgery();

app.MapPost("/account/register-submit", async (
    HttpRequest request,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) =>
{
    var form = await request.ReadFormAsync();
    var email = form["email"].ToString();
    var password = form["password"].ToString();
    var role = form["role"].ToString();
    var isPsychologist = role == "Psychologist";
    var user = new ApplicationUser
    {
        UserName = email,
        Email = email,
        EmailConfirmed = true,
        FullName = form["fullName"].ToString(),
        RolePreference = isPsychologist ? "Psychologist" : "Patient",
        PsychologistSpecialty = isPsychologist ? form["specialty"].ToString() : null,
        PsychologistBio = isPsychologist ? form["bio"].ToString() : null,
        PsychologistAvailableDays = isPsychologist ? string.Join(", ", form["availableDays"].ToArray()) : null,
        PsychologistAvailableFrom = isPsychologist && !string.IsNullOrWhiteSpace(form["availableFrom"].ToString()) ? form["availableFrom"].ToString() : "09:00",
        PsychologistAvailableTo = isPsychologist && !string.IsNullOrWhiteSpace(form["availableTo"].ToString()) ? form["availableTo"].ToString() : "17:00"
    };

    var result = await userManager.CreateAsync(user, password);
    if (!result.Succeeded)
    {
        var error = string.Join(" ", result.Errors.Select(e => e.Description));
        return Results.Redirect($"/account/register?error={Uri.EscapeDataString(error)}");
    }

    await signInManager.SignInAsync(user, isPersistent: true);
    return Results.Redirect("/dashboard");
}).DisableAntiforgery();

app.MapPost("/appointments/create-submit", async (
    HttpContext context,
    UserManager<ApplicationUser> userManager,
    AppointmentService appointmentService,
    PsychologistService psychologistService) =>
{
    var currentUser = await userManager.GetUserAsync(context.User);
    if (currentUser is null)
    {
        return Results.Redirect("/account/login?returnUrl=/appointments/create");
    }

    if (currentUser.RolePreference == "Psychologist")
    {
        return Results.Redirect($"/appointments/create?error={Uri.EscapeDataString("Psychologist accounts cannot schedule patient appointments.")}");
    }

    var form = await context.Request.ReadFormAsync();
    var psychologistName = form["psychologistName"].ToString();
    var reason = form["reason"].ToString();

    if (!DateOnly.TryParse(form["appointmentDate"].ToString(), out var appointmentDate) ||
        !TimeOnly.TryParse(form["appointmentTime"].ToString(), out var appointmentTime))
    {
        return Results.Redirect($"/appointments/create?error={Uri.EscapeDataString("Select a valid appointment date and time.")}");
    }

    var psychologists = await psychologistService.GetPsychologistsAsync();
    var selectedPsychologist = psychologists.FirstOrDefault(p => p.Name == psychologistName);
    if (selectedPsychologist is null)
    {
        return Results.Redirect($"/appointments/create?error={Uri.EscapeDataString("Select an available psychologist.")}");
    }

    if (appointmentDate < DateOnly.FromDateTime(DateTime.Today))
    {
        return Results.Redirect($"/appointments/create?error={Uri.EscapeDataString("Scheduled appointments cannot be created in the past.")}");
    }

    var selectedDay = appointmentDate.DayOfWeek.ToString();
    if (!selectedPsychologist.AvailableDays.Contains(selectedDay, StringComparison.OrdinalIgnoreCase))
    {
        return Results.Redirect($"/appointments/create?error={Uri.EscapeDataString($"{selectedPsychologist.Name} is not available on {selectedDay}.")}");
    }

    var availableFrom = TimeOnly.Parse(selectedPsychologist.AvailableFrom);
    var availableTo = TimeOnly.Parse(selectedPsychologist.AvailableTo);
    if (appointmentTime < availableFrom || appointmentTime >= availableTo)
    {
        return Results.Redirect($"/appointments/create?error={Uri.EscapeDataString($"{selectedPsychologist.Name} is available from {selectedPsychologist.AvailableFrom} to {selectedPsychologist.AvailableTo}.")}");
    }

    var appointment = new Appointment
    {
        UserId = currentUser.Id,
        PatientName = currentUser.FullName,
        PsychologistName = selectedPsychologist.Name,
        AppointmentDate = appointmentDate,
        AppointmentTime = appointmentTime,
        Reason = reason,
        Notes = form["notes"].ToString(),
        Status = AppointmentStatus.Scheduled,
        CreatedAt = DateTime.UtcNow
    };

    if (await appointmentService.HasPsychologistConflictAsync(appointment))
    {
        return Results.Redirect($"/appointments/create?error={Uri.EscapeDataString("That psychologist already has an appointment at that date and time.")}");
    }

    await appointmentService.AddAsync(appointment);
    return Results.Redirect("/appointments");
}).RequireAuthorization().DisableAntiforgery();

app.MapPost("/account/logout", async (SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/");
}).DisableAntiforgery();

app.Run();

static string NormalizeReturnUrl(string? returnUrl)
{
    if (string.IsNullOrWhiteSpace(returnUrl))
    {
        return "/dashboard";
    }

    if (Uri.TryCreate(returnUrl, UriKind.Absolute, out var absoluteUri))
    {
        return string.IsNullOrWhiteSpace(absoluteUri.PathAndQuery) ? "/dashboard" : absoluteUri.PathAndQuery;
    }

    return returnUrl.StartsWith('/') && !returnUrl.StartsWith("//") ? returnUrl : "/dashboard";
}
