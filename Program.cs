using UniDesc.Web.Services;
using UniDesc.Web.Models;
using Microsoft.EntityFrameworkCore;
using UniDesc.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ITicketService, InMemoryTicketService>();

builder.Services.AddProblemDetails();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Ciąg połączenia nie może być pusty.");
}

builder.Services.AddDbContext<UniDeskDbContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

app.UseStaticFiles();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();