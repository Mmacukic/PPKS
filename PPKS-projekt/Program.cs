using Auth0.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using PPKS_projekt.Data;
using PPKS_projekt.Hubs;
using PPKS_projekt.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services
    .AddAuth0WebAppAuthentication(options =>
    {
        builder.Configuration.Bind("Auth0", options);
    });
builder.Services.AddScoped<MockShipmentStatusGenerator>();

var app = builder.Build();
app.MapHub<OrderHub>("/orderHub");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await SeedData.InitializeAsync(db);
}
app.Run();