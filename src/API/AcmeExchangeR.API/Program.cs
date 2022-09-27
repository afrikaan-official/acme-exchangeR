using AcmeExchangeR.API.BackgroundServices;
using AcmeExchangeR.Utils.Configurations;
using AcmeExchangeR.Utils.FastForexClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.UseUrls("http://*:5007");

builder.Services.AddHttpClient("fastForex", o =>
{
    o.Timeout = TimeSpan.FromMilliseconds(1000);
    o.BaseAddress = new Uri(builder.Configuration.GetValue<string>("FastForexAPI:Url"));
});
builder.Services.Configure<FastForexConfiguration>(builder.Configuration.GetSection("FastForexAPI"));
builder.Services.AddScoped<IFastForexClient, FastForexClient>();
builder.Services.AddHostedService<ExchangeFetcherBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();