using AlignHR.Data;
using AlignHR.Repositories;
using AlignHR.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.ComponentModel;

var builder = WebApplication.CreateBuilder(args);

// -----------------------
// Add services to the container
// -----------------------
var mvc = builder.Services.AddControllersWithViews();
if (builder.Environment.IsDevelopment())
{
    mvc.AddRazorRuntimeCompilation();
}

// Register DMS Workflow Services
builder.Services.AddScoped<IFlowResolver, FlowResolver>();
builder.Services.AddScoped<IWorkflowEngine, WorkflowEngine>();

// Register Payroll Lock Service (centralized GL lock enforcement)
builder.Services.AddScoped<IPayrollLockService, PayrollLockService>();


// Register IHttpContextAccessor (needed by TagHelpers)
builder.Services.AddHttpContextAccessor();

// Register Leave  Services
builder.Services.AddScoped<ILeaveWorkflowService, LeaveWorkflowService>();
builder.Services.AddScoped<ILeaveAccountingService, LeaveAccountingService>();

// Register Recruitment Services
builder.Services.AddScoped<IHrRequisitionRepository, HrRequisitionRepository>();
builder.Services.AddScoped<IHrRequisitionService, HrRequisitionService>();
builder.Services.AddScoped<IHrWorkflowRepository, HrWorkflowRepository>();
builder.Services.AddScoped<IHrRequisitionWorkflowService, HrRequisitionWorkflowService>();
builder.Services.AddScoped<IHrAssignmentRepository, HrAssignmentRepository>();
builder.Services.AddScoped<IHrAssignmentService, HrAssignmentService>();
builder.Services.AddScoped<IHrJobPostingRepository, HrJobPostingRepository>();
builder.Services.AddScoped<IHrJobPostingService, HrJobPostingService>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped<IHrCandidateRepository, HrCandidateRepository>();
builder.Services.AddScoped<IHrCandidateService, HrCandidateService>();
builder.Services.AddScoped<IHrApplicationRepository, HrApplicationRepository>();
builder.Services.AddScoped<IHrApplicationService, HrApplicationService>();
builder.Services.AddScoped<IHrInterviewRepository, HrInterviewRepository>();
builder.Services.AddScoped<IHrInterviewService, HrInterviewService>();
builder.Services.AddScoped<IHrOfferRepository, HrOfferRepository>();
builder.Services.AddScoped<IHrOfferService, HrOfferService>();
builder.Services.AddScoped<IHrOnboardingRepository, HrOnboardingRepository>();
builder.Services.AddScoped<IHrOnboardingService, HrOnboardingService>();
builder.Services.AddScoped<IEmployeeCreationService, HrEmployeeCreationService>();

// Register DbContext
// UseCompatibilityLevel(120) prevents EF Core 8 from generating WITH (CTE) clauses
// that cause "Incorrect syntax near 'WITH'" errors on older SQL Server versions
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.UseCompatibilityLevel(120)));

// -----------------------
// Configure Session
// -----------------------
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// -----------------------
// Configure Authentication
// -----------------------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login";
    });

// -----------------------
// Configure Authorization
// -----------------------
builder.Services.AddAuthorization();


// -----------------------
// Build the app
// -----------------------
var app = builder.Build();

// -----------------------
// Configure the HTTP request pipeline
// -----------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable session
app.UseSession();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();



