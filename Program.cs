using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystemApp.Components;
using StudentManagementSystemApp.Components.Account;
using StudentManagementSystemApp.Data;
using StudentManagementSystemApp.Helpers;
using StudentManagementSystemApp.Interfaces;
using StudentManagementSystemApp.Models;
using StudentManagementSystemApp.Repositories;
using StudentManagementSystemApp.Services;

var builder = WebApplication.CreateBuilder(args);

#region Database

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<DapperContext>();

#endregion


builder.Services.AddHttpContextAccessor();


builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));


#region Repositories

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookIssueRepository, BookIssueRepository>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<IUserManagementRepository, UserManagementRepository>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IAuditRepository, AuditRepository>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IAuditComparer, AuditComparer>();
builder.Services.AddScoped<IDashboardDtoRepository, DashboardDtoRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IStudentNotificationRepository, StudentNotificationRepository>();

builder.Services.AddScoped<DepartmentService>();
builder.Services.AddScoped<SubjectService>();
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<BookIssueService>();
builder.Services.AddScoped<TeacherService>();
builder.Services.AddScoped<UserManagementService>();
builder.Services.AddScoped<DashboardDtoService>();
builder.Services.AddHostedService<OverdueReminderService>();
builder.Services.AddScoped<NotificationService>();



#endregion

#region Controllers

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion

#region Razor Components

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

#endregion

#region Identity

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<IdentityRedirectManager>();

builder.Services.AddScoped<AuthenticationStateProvider,
    IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddIdentityCookies();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;

    options.User.RequireUniqueEmail = true;

    options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders();



builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>,
    IdentityNoOpEmailSender>();

#endregion

var app = builder.Build();

#region Seed Roles

using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedRolesAsync(scope.ServiceProvider);
}

#endregion

#region Middleware

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

    app.UseSwagger();

    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStatusCodePagesWithReExecute(
    "/not-found",
    createScopeForStatusCodePages: true);

app.UseAuthentication();

app.UseAuthorization();

app.UseAntiforgery();

#endregion

#region Static Files

app.MapStaticAssets();

#endregion

#region Controllers

app.MapControllers();

#endregion

#region Blazor

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();

#endregion

app.Run();