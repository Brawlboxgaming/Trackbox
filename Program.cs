using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Trackbox.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#if DEBUG
var loginString = "LoginStringDebug";
#else
var loginString = "LoginString";
#endif
var connectionString = builder.Configuration.GetConnectionString(loginString) ?? throw new InvalidOperationException($"Connection string '{loginString}' not found.");
builder.Services.AddDbContext<LoginDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<LoginDbContext>();
builder.Services.AddRazorPages();

builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._+";
    options.User.RequireUniqueEmail = true;

    options.SignIn.RequireConfirmedEmail = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Admin", "Moderator", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    var adminUser = await userManager.FindByEmailAsync("admin@brawlbox.co.uk");
    await userManager.AddToRoleAsync(adminUser, "Admin");
}

app.Run();
