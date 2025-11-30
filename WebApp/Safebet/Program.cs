using SafeBet.Services;
using SafeBet.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<SafeBet.Services.SafeAdvisorService>(c =>
{
    c.BaseAddress = new Uri("https://thesafeadvisor-e3bbcuacdfg7geb8.canadacentral-01.azurewebsites.net");
});

builder.Services.AddDbContext<SafeBetContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SafeBetDB")));

builder.Services.AddControllersWithViews();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
    });


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();