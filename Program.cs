using Microsoft.EntityFrameworkCore;
using QuanLyChoThuePhongTro.Data;
using QuanLyChoThuePhongTro.Repositories;
using QuanLyChoThuePhongTro.Services;

// Cho phép Npgsql tự xử lý DateTime không có Kind (tránh lỗi 'not a valid timestamp with time zone')
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Configure form/file upload limits
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 52428800; // 50 MB
});
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 52428800; // 50 MB
});

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Add DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add repositories
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IRentalContractRepository, RentalContractRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Add services
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// Add memory cache
builder.Services.AddMemoryCache();

// Add session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Create database and apply migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();

    // Seed Admin User
    try 
    {
        var authService = services.GetRequiredService<IAuthenticationService>();
        
        // Check if admin user exists by username
        var existingAdmin = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == "admin");
        
        if (existingAdmin == null)
        {
            // Create new admin user
            var adminUser = await authService.RegisterAsync("admin", "admin@example.com", "admin123", "Administrator", "Admin");
            if (adminUser != null)
            {
                Console.WriteLine("Admin user created successfully.");
            }
            else
            {
                Console.WriteLine("Failed to create admin user (RegisterAsync returned null).");
            }
        }
        else if (existingAdmin.Role != "Admin")
        {
            // Promote existing user to admin
            existingAdmin.Role = "Admin";
            await dbContext.SaveChangesAsync();
            Console.WriteLine("Existing 'admin' user promoted to Admin role.");
        }
        else
        {
            Console.WriteLine("Admin user already exists.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error seeding admin user: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
        }
    }
}

await app.RunAsync();
