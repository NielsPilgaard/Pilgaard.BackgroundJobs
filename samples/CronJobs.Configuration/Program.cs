using Pilgaard.CronJobs.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCronJobs(typeof(Program));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
