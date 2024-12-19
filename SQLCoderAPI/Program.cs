using SQLCoderAPI.Extensions;
using SQLCoderAPI.Interfaces;
using SQLCoderAPI.Services;

namespace SQLCoderAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();

        // Register Semantic Kernel
        builder.AddSemanticKernelServices();
        builder.Services.AddTransient<IDBService, DBService>();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        // Add CORS services
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                builder => builder.WithOrigins("http://127.0.0.1:5500")
                                  .AllowAnyHeader()
                                  .AllowAnyMethod());
        });

        builder.Services.AddHttpClient("ollamaclient", c =>
        {
            c.BaseAddress = new Uri("http://localhost:11434");
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        // Enable CORS
        app.UseCors("AllowSpecificOrigin");

        app.MapControllers();

        app.Run();
    }
}
