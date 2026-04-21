using Northwind.EntityModels;
using Microsoft.Extensions.Caching.Memory;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IMemoryCache>(new MemoryCache(
new MemoryCacheOptions
{
    TrackStatistics = true,
    SizeLimit = 50 // Товары
}));
// Add services to the container.
builder.Services.AddNorthwindContext();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDistributedMemoryCache();
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
