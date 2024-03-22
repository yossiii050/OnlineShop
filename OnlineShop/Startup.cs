// Startup.cs

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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



        services.Configure<BrainTreeSettings>(Configuration.GetSection("BrainTree"));
        //services.AddSingleton<IBrainTreeGate, BrainTreeGate>();
        services.AddScoped<IBrainTreeGate, BrainTreeGate>();

        var brainTreeSettings = Configuration.GetSection("BrainTree").Get<BrainTreeSettings>();
        Console.WriteLine($"BrainTree Environment: {brainTreeSettings.Environment}");
        Console.WriteLine($"BrainTree MerchantId: {brainTreeSettings.MerchantId}");
        Console.WriteLine($"BrainTree PublicKey: {brainTreeSettings.PublicKey}");
        Console.WriteLine($"BrainTree PrivateKey: {brainTreeSettings.PrivateKey}");

        // Add BrainTree configuration
        services.Configure<BrainTreeSettings>(Configuration.GetSection("BrainTree"));
        services.AddScoped<IBrainTreeGate, BrainTreeGate>();

        //aes
        var aesSettings = Configuration.GetSection("AES").Get<AESSettings>();
        services.AddSingleton(aesSettings);

        // Add framework services.
        services.AddRazorPages();

        // Add other services
        services.AddMvc();
        services.AddControllersWithViews();
        services.AddSession();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSession();
        // Configure the app
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
