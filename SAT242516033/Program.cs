using DbContexts;
using Microsoft.EntityFrameworkCore;
using MyDbModels;
using Providers;
using SAT242516033.Components;
using SAT242516033.Data;
using SAT242516033.Services;
using UnitOfWorks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(defaultConnection));
builder.Services.AddDbContext<MyDbModel_DbContext>(options =>
    options.UseSqlServer(defaultConnection));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped<IMyDbModel_UnitOfWork, MyDbModel_UnitOfWork<MyDbModel_DbContext>>();
builder.Services.AddScoped(typeof(IMyDbModel<>), typeof(MyDbModel<>));
builder.Services.AddScoped<IMyDbModel_Provider, MyDbModel_Provider>();
builder.Services.AddScoped<KullaniciAuthService>();
builder.Services.AddScoped<KullaniciSession>();

var app = builder.Build();

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

app.Run();
