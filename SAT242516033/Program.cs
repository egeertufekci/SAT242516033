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
using Microsoft.AspNetCore.Localization; // EKLENDİ
using System.Globalization; // EKLENDİ
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// --- LOGGING AYARLARI ---
var logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);

var logpath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);

var compositeLoggerProvider = new CompositeLoggerProvider()
	.AddProvider(new AsyncFileLoggerProvider(Path.Combine("Logs", "app-log.txt")));

builder.Logging.ClearProviders();
builder.Logging.AddProvider(compositeLoggerProvider);

builder.Services.AddSingleton(new LogService(
	filePath: Path.Combine("Logs", "app-log.txt")
));
// ------------------------

// DÜZELTME 1: SignalR mesaj boyutunu 10MB yaptık (Büyük loglar ve PDF donmasın diye)
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents()
	.AddHubOptions(options =>
	{
		options.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10MB
	});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
{
	options.DefaultScheme = IdentityConstants.ApplicationScheme;
	options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
	.AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
	?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
	// 1. KULLANICI ŞİFRE İLKELERİNİN BELİRLENMESİ
	options.SignIn.RequireConfirmedAccount = true;

	options.Password.RequireDigit = true;           // En az bir rakam olsun mu?
	options.Password.RequiredLength = 6;            // En az kaç karakter?
	options.Password.RequireNonAlphanumeric = false; // Sembol (!@#) zorunlu mu?
	options.Password.RequireUppercase = false;       // Büyük harf zorunlu mu?
	options.Password.RequireLowercase = true;        // Küçük harf zorunlu mu?

	// Ekstra: Yanlış şifre girince kilitlenme (Lockout) ayarları
	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
	options.Lockout.MaxFailedAccessAttempts = 5;
})
	.AddRoles<IdentityRole>() // 2. ROL YÖNETİMİ & RoleManager BURADA EKLENİR (Çok Önemli!)
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddSignInManager()
	.AddDefaultTokenProviders();



builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddSignInManager()
	.AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// --- DB SERVİSLERİ ---
builder.Services.AddDbContext<MyDbModel_DbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IMyDbModel_UnitOfWork, MyDbModel_UnitOfWork<MyDbModel_DbContext>>();
builder.Services.AddScoped(typeof(IMyDbModel<>), typeof(MyDbModel<>));
builder.Services.AddScoped<IMyDbModel_UnitOfWork, MyDbModel_UnitOfWork<ApplicationDbContext>>();
builder.Services.AddScoped<IMyDbModel_Provider, MyDbModel_Provider>();

// --- DİĞER SERVİSLER ---
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<IUserService, UserService>();

// DÜZELTME 2: İstatistikler sayfasının çalışması için gerekli servis kaydı
builder.Services.AddScoped<StatsService>();

builder.Services.AddAuthorizationCore();
builder.Services.AddControllers();


// 🌍 LOCALIZATION (DİL) AYARLARI - DÜZELTİLMİŞ HALİ

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddScoped(typeof(SAT242516033.Models.MyServices.LocalizerService<>));

var supportedCultures = new[] { "tr-TR", "en-US", "tr", "en" };

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
	var cultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();

	options.DefaultRequestCulture = new RequestCulture("tr-TR"); // Varsayılan TR olsun
	options.SupportedCultures = cultures;
	options.SupportedUICultures = cultures;

	// Dil seçimi önceliği: URL (?culture=tr-TR) -> Cookie -> Tarayıcı Dili
	options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
});
// =================================================================
builder.Services.AddCascadingAuthenticationState();
QuestPDF.Settings.License = LicenseType.Community;
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
 
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Migration error:");
        Console.WriteLine(ex.ToString());
    }
}

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

// 🌍 DİL AYARINI AKTİF ET (App Run'dan önce!)
var locOptions = app.Services.GetRequiredService<Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOptions.Value);
// ------------------------------------------

app.UseAntiforgery();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();
app.MapControllers();

app.Run();