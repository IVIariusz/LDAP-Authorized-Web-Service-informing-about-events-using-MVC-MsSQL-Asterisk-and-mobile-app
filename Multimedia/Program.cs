using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Multimedia.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Dodanie kontekstu bazy danych
builder.Services.AddDbContext<SchoolEventsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolEventsDB")));
builder.Services.Configure<LdapConfig>(builder.Configuration.GetSection("Ldap"));

// Dodanie kontrolerów i widoków
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication("CookieAuthentication")
    .AddCookie("CookieAuthentication", options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
    });


// Dodaj konfiguracjê LDAP

var app = builder.Build();

// Konfiguracja pipeline'a HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
