using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife;
using pkmnWildLife.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var configuration = builder.Configuration;

// set secrets with dotnet user-secrets set "Auth:sjdhfdshf" "secret"
builder.Services.AddAuthentication().AddLichess(options =>
{
    options.ClientId = configuration["Auth:Lichess:ClientID"];
    options.ClientSecret = configuration["Auth:Lichess:ClientSecret"];
}).AddDiscord(
    options =>
    {
        options.ClientId = configuration["Auth:Discord:ClientID"];
        options.ClientSecret = configuration["Auth:Discord:ClientSecret"];
    });

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
    await DBInitializer.InitializeDB(context);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();