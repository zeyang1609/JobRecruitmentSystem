using JobRecruitmentSystem.Data;
using JobRecruitmentSystem.Models;
using Microsoft.AspNetCore.Authentication.Cookies; // Required for manual auth
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// Program.cs

// 1. Calculate the path to the App_Data folder
string baseDir = AppDomain.CurrentDomain.BaseDirectory;
if (baseDir.Contains("bin"))
{
   
    int index = baseDir.IndexOf("bin");
    baseDir = baseDir.Substring(0, index);
}
string dataDirectory = Path.Combine(baseDir, "App_Data");

// 2. Set the "DataDirectory" variable so connection strings can use it
AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);

var builder = WebApplication.CreateBuilder(args);


// 1. Add Database Service
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Add Manual Cookie Authentication Service [cite: 74]
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Redirect here if user is not logged in
        options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect here if role is unauthorized
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Session timeout
    });

// Add MVC Services
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 3. Enable Authentication & Authorization
app.UseAuthentication(); // Must be before Authorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();