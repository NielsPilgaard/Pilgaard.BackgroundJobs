using BackgroundJobs.Configuration;
using Pilgaard.BackgroundJobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBackgroundJobs()
    .AddJob<ReloadableCronJob>(nameof(ReloadableCronJob));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
