using JeanEdwardTask.API.Integrations;
using JeanEdwardTask.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

  builder.Services.AddHttpClient<ImdbClientIntegration>();

builder.Services.AddMemoryCache();  //
builder.Services.AddSingleton<CacheLatest>();

builder.Services.Configure<OMDb>(builder.Configuration.GetSection("OMDb"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
