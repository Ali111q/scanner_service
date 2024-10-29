using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScannerApi.Services;
using ScannerService;
using System;

var builder = WebApplication.CreateBuilder(args);

// Register ScannerReadService as Singleton for compatibility with HostedService
builder.Services.AddSingleton<ScannerReadService>();

// Register the Worker as a background service
builder.Services.AddHostedService<Worker>();

// Add controllers and CORS for API access
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Build the app with try-catch to log any DI-related errors
try
{
    var app = builder.Build();

    // Apply CORS policy
    app.UseCors("AllowAllOrigins");

    // Configure HTTP request pipeline for development environment
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseRouting();

    app.MapControllers(); // Map controller endpoints

    // Run as a Windows Service
    await app.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine("Exception during Build: " + ex);
    throw;
}