using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MyCuisine.Web;
using MyCuisine.Web.Constants;
using MyCuisine.Web.Data;

var builder = WebApplication.CreateBuilder(args);

var appSettings = builder.Configuration.Get<AppSettings>();
appSettings.FirebaseAdminConfig = File.ReadAllText(Path.Combine(builder.Environment.ContentRootPath, "FirebaseAdmin.json"));
builder.Services.AddSingleton(appSettings);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

    if (builder.Environment.IsDevelopment())
    {
        options.LogTo(Console.Write);
    }
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
    });

builder.Services.AddAuthorization(options =>
    options.AddPolicy(AuthConstants.AdminPolicy,
    policy => policy.RequireClaim(AuthConstants.AdminClaim))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetService<ApplicationContext>();
    dbContext.Database.Migrate();

    if (!dbContext.Users.Any(x => x.IsAdmin))
    {
        dbContext.Users.Add(new MyCuisine.Data.Web.Models.User
        {
            Email = "mycuisine@mvp-stack.com",
            Name = "Admin",
            IsActive = true,
            IsAdmin = true,
            DateCreated = DateTimeOffset.Now,
            DateModified = DateTimeOffset.Now
        });
        dbContext.SaveChanges();
    }
}

app.Run();