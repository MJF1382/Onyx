using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Onyx.Models.Database;
using Onyx.Models.Identity.Entities;
using Onyx.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<MailSettings>();
builder.Services.AddTransient<IMailSender, MailSender>();
builder.Services.AddDbContext<OnyxDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
builder.Services
    .AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<OnyxDBContext>()
    .AddDefaultTokenProviders();
builder.Services
    .AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = "666755732744-efuc1a7kgu1dihvagutbdirjrom6d4j3.apps.googleusercontent.com";
        options.ClientSecret = "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";
    });
    //.AddFacebook(options =>
    //{
    //    options.AppId = "dgdg";
    //    options.AppSecret = "dgdg";
    //});
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireNonAlphanumeric  = false;
    options.Password.RequiredLength = 8;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(5);
    options.Lockout.MaxFailedAccessAttempts = 3;
});
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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

app.Run();
