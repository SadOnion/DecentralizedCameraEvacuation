var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppConfig>(builder.Configuration);
builder.Services.AddSingleton<CameraMonitoringService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<CameraMonitoringService>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.MapGet("/Status", (CameraMonitoringService server) =>
{
    return new { TotalPeople = server.TotalPeople, Cameras = server.CameraMonitor?.GetPeopleCountByCamera() };
});

await app.RunAsync();
