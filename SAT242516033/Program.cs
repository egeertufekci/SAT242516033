using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SAT242516033.Components;
using SAT242516033.Components.Account;
using SAT242516033.Logging;
using SAT242516033.Models.DbContexts;
using SAT242516033.Models.MyDbModels;
using SAT242516033.Models.Providers;
using SAT242516033.Models.UnitOfWorks;
using SAT242516033.Data;

var builder = WebApplication.CreateBuilder(args);

// --- 1. LOGLAMA AYARLARI ---
var logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);

var compositeLoggerProvider = new CompositeLoggerProvider()
    .AddProvider(new AsyncFileLoggerProvider(Path.Combine("Logs", "app-log.txt")));

builder.Logging.ClearProviders();
builder.Logging.AddProvider(compositeLoggerProvider);

builder.Services.AddSingleton(new LogService(
    filePath: Path.Combine("Logs", "app-log.txt")
));

// --- 2. BLAZOR TEMEL AYARLARI ---
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState(); // ÞART!
builder.Services.AddHttpContextAccessor();

// --- 3. SESSION VE AUTH AYARLARI ---
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();


// --- 4. VERÝTABANI BAÐLANTILARI ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContext<MyDbModel_DbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<StatsService>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity ayarlarý
builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddRoles<ApplicationRole>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// --- 5. UNIT OF WORK & PROVIDERS ---
// Generic UnitOfWork
builder.Services.AddScoped<IMyDbModel_UnitOfWork, MyDbModel_UnitOfWork<MyDbModel_DbContext>>();
// ApplicationDbContext kullanan UnitOfWork (Bunu da eklemiþsin, kalsýn)
builder.Services.AddScoped<IMyDbModel_UnitOfWork, MyDbModel_UnitOfWork<ApplicationDbContext>>();

// Generic Models
builder.Services.AddScoped(typeof(IMyDbModel<>), typeof(MyDbModel<>));

// Providers
builder.Services.AddScoped<IMyDbModel_Provider, MyDbModel_Provider>();

// --- 6. LOCALIZATION (DÝL) AYARLARI ---
builder.Services.AddLocalization(options => options.ResourcesPath = Path.Combine("Models", "MyResources"));
builder.Services.AddScoped(typeof(LocalizerService<>));

// Controller Localization desteði
builder.Services.AddControllers()
    .AddDataAnnotationsLocalization(options => {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(SAT242516033.Loc));
    });

var supportedCultures = new[] { "tr", "en", "de" };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("tr");
    options.SupportedCultures = supportedCultures.Select(c => new System.Globalization.CultureInfo(c)).ToList();
    options.SupportedUICultures = supportedCultures.Select(c => new System.Globalization.CultureInfo(c)).ToList();
    options.RequestCultureProviders.Insert(0, new Microsoft.AspNetCore.Localization.QueryStringRequestCultureProvider());
    options.RequestCultureProviders.Insert(1, new Microsoft.AspNetCore.Localization.CookieRequestCultureProvider());
    options.RequestCultureProviders.Insert(2, new Microsoft.AspNetCore.Localization.AcceptLanguageHeaderRequestCultureProvider());
});

// --- EMAIL SENDER (Identity Hatasý Vermesin Diye) ---
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// --- APP BAÞLIYOR ---
var app = builder.Build();

// --- 7. DATABASE MIGRATION TRIGGER ---
using (var scope = app.Services.CreateScope())
{
    try
    {
        Console.WriteLine("Veritabaný baðlantýsý kontrol ediliyor...");
        // Baðlantýyý tetiklemek için ufak bir check
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        // db.Database.Migrate(); // Entity Framework kullanýyorsan açabilirsin
        Console.WriteLine("Sistem Hazýr.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Baþlatma hatasý: " + ex.Message);
    }
}

// --- 8. MIDDLEWARE AYARLARI ---
var locOptions = app.Services.GetRequiredService<Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOptions.Value);

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Identity endpointlerini ekle (Kayýt ol vs. çalýþsýn diye)
app.MapAdditionalIdentityEndpoints();

// --- ADMIN SEED ---
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    const string adminRoleName = "Admin";
    const string adminUserName = "admin";
    const string adminPassword = "admin123";

    if (!await roleManager.RoleExistsAsync(adminRoleName))
    {
        await roleManager.CreateAsync(new ApplicationRole { Name = adminRoleName });
    }

    var adminUser = await userManager.FindByNameAsync(adminUserName);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminUserName,
            Email = "admin@test.com",
            EmailConfirmed = true
        };
        await userManager.CreateAsync(adminUser, adminPassword);
    }
    else
    {
        adminUser.EmailConfirmed = true;
        await userManager.UpdateAsync(adminUser);
    }

    if (!await userManager.CheckPasswordAsync(adminUser, adminPassword))
    {
        var resetToken = await userManager.GeneratePasswordResetTokenAsync(adminUser);
        await userManager.ResetPasswordAsync(adminUser, resetToken, adminPassword);
    }

    if (!await userManager.IsInRoleAsync(adminUser, adminRoleName))
    {
        await userManager.AddToRoleAsync(adminUser, adminRoleName);
    }
}

app.Run();
