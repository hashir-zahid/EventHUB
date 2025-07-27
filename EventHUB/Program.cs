using EventHUB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc; // Needed for ResponseCacheAttribute
using static EventHUB.Models.ApplicationDbContext;

var builder = WebApplication.CreateBuilder(args);

// ✅ Add global response cache disabling and MVC
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new ResponseCacheAttribute
    {
        NoStore = true,
        Location = ResponseCacheLocation.None
    });
});

// ✅ Add EF Core
builder.Services.AddDbContext<ApplicationDbcontext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("default"))
);

// ✅ Add cookie authentication
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Login/Login";
        options.LogoutPath = "/Login/Logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

var app = builder.Build();

// ✅ Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ✅ Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Guest}/{action=GuestDashboard}/{id?}"
);

app.Run();
