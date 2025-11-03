using SafeBet.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<SafeBet.Services.SafeAdvisorService>(c =>
{
    c.BaseAddress = new Uri("http://127.0.0.1:8000");
});

builder.Services.AddHttpClient<SafeAdvisorService>(client =>
{
    client.BaseAddress = new Uri("http://127.0.0.1:8000");
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
