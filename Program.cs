using Contracts.V1;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tweeting_book.Data;
using Tweeting_book.Installers;
using Tweeting_book.Migrations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

// Run all installers
var installers = typeof(IInstaller).Assembly.ExportedTypes
    .Where(x => typeof(IInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
    .Select(Activator.CreateInstance)
    .Cast<IInstaller>()
    .ToList();

foreach (var installer in installers)
{
    installer.InstallServices(builder.Services, builder.Configuration);
}

// Register DataContext
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Identity (if not already in an installer)
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TagViewer", policy =>
        policy.RequireClaim("tags.view", "true"));
});



var app = builder.Build();

// Seed roles on startup
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

    // Apply migrations
    await dbContext.Database.MigrateAsync();

    // Seed Admin role
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // Seed Poster role
    if (!await roleManager.RoleExistsAsync("Poster"))
    {
        await roleManager.CreateAsync(new IdentityRole("Poster"));
    }
}

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();