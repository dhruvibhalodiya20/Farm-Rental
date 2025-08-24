public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // ...existing code...
        services.AddSession();
        // ...existing code...
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // ...existing code...
        app.UseSession();
        // ...existing code...
    }
}