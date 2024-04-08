using Microsoft.AspNetCore.Identity;
using Stripe;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using OnlineShop.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.UI.Services;
using OnlineShop.Utillity;
using OnlineShop.Models.BrainTree;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSession();

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddDbContext<DBProjectContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("conn")));

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DBProjectContext>();

builder.Services.AddRazorPages();

builder.Services.Configure<BraintreeService>(builder.Configuration.GetSection("BraintreeGateway"));
builder.Services.AddSingleton<IBraintreeService>(sp => sp.GetRequiredService<IOptions<BraintreeService>>().Value);


builder.Services.Configure<AESSettings>(builder.Configuration.GetSection("AES"));
builder.Services.AddSingleton<IAESSettings>(sp => sp.GetRequiredService<IOptions<AESSettings>>().Value);


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
StripeConfiguration.ApiKey=builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using(var scope=app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Admin", "Regular", "Advanced" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

app.Run();

