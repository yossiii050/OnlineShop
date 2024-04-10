// Startup.cs

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineShop.Models;
using OnlineShop.Models.BrainTree;
using Stripe;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        Console.WriteLine($"fdsahf Environment::");
        services.AddTransient<IBraintreeService, BraintreeService>();
        services.AddDbContext<DBProjectContext>(options =>
                                                options.UseSqlServer(
                                                Configuration.GetConnectionString("conn")));

        services.AddTransient<IEmailSender,IEmailSender>();
        
        var aesSettings = Configuration.GetSection("AES").Get<AESSettings>();
        services.AddSingleton(aesSettings);

        services.AddRazorPages();

        services.AddMvc();
        services.AddControllersWithViews();
        services.AddSession();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSession();
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
