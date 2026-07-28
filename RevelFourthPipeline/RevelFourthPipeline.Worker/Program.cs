using RevelFourthPipeline.Worker;
using RevelFourthPipeline.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "Revel Fourth ProductMix Pipeline";
});
builder.Services.AddRevelFourthPipeline(builder.Configuration);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
