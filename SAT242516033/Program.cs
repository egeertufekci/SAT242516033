using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SAT242516033.Components;
using SAT242516033.Components.Account;
using SAT242516033.Data;
using SAT242516033.Logging;
using SAT242516033.Models.DbContexts;
using SAT242516033.Models.MyDbModels;
using SAT242516033.Models.Providers;
using SAT242516033.Models.UnitOfWorks;
using Microsoft.Data.SqlClient;
using SAT242516033.Models.MyServices;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

var builder = WebApplication.CreateBuilder(args);

// --- 1. LOGLAMA AYARLARI ---
var logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);

var compositeLoggerProvider = new CompositeLoggerProvider()
    .AddProvider(new AsyncFileLoggerProvider(Path.Combine("Logs", "app-log.txt")))
    .AddProvider(new AsyncDbLoggerProvider(() =>
        new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

builder.Logging.ClearProviders();
builder.Logging.AddProvider(compositeLoggerProvider);

builder.Services.AddSingleton(new LogService(
    filePath: Path.Combine("Logs", "app-log.txt"),
    connectionFactory: () => new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))
));

// --- 2. BLAZOR TEMEL AYARLARI ---
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState(); // ÞART!
builder.Services.AddHttpContextAccessor();

// --- 3. SESSION VE AUTH AYARLARI (BURASI DÜZELTÝLDÝ) ---
// Session Storage'ý bir kere ekliyoruz
builder.Services.AddScoped<ProtectedSessionStorage>();

// Standart Identity Provider YERÝNE sadece bizimkini ekliyoruz.
// Eski kodda hem IdentityRevalidating hem Custom vardý, çakýþýyordu.
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// AuthService'i ekle
builder.Services.AddScoped<AuthService>();

// Yetkilendirme çekirdeðini ekle
builder.Services.AddAuthorizationCore();
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
builder.Services.AddScoped<DbLogService>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity ayarlarý (Veritabaný iþlemleri için kalsýn ama UI provider'ý bizimki olacak)
builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Identity endpointlerini ekle (Kayýt ol vs. çalýþsýn diye)
app.MapAdditionalIdentityEndpoints();

// --- GEÇÝCÝ SEED KODU (app.Run();'dan önceye yapýþtýr) ---
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var userCheck = await userManager.FindByEmailAsync("admin@test.com");
    if (userCheck == null)
    {
        var user = new ApplicationUser { UserName = "admin", Email = "admin@test.com", EmailConfirmed = true };
        await userManager.CreateAsync(user, "Admin123!"); // Þifre: Admin123!
    }
}
// --- GEÇÝCÝ KOD SONU ---

app.Run();