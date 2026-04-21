using System.Net.Http.Headers;
using Polly; // Для метода AddTransientHttpErrorPolicy
using Polly.Contrib.WaitAndRetry; // Для техники Backoff
using Polly.Extensions.Http; // Для пакета HttpPolicyExtensions
using Polly.Retry; // Для политики AsyncRetryPolicy<T>

// Создание пяти джиттерных задержек начиная примерно с 1 секунды
IEnumerable<TimeSpan> delays = Backoff.DecorrelatedJitterBackoffV2(
medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 5);
Console.WriteLine("Jittered delays for Polly retries:");
foreach (TimeSpan item in delays)
{
    Console.WriteLine($" {item.TotalSeconds:N2} seconds.");
}
AsyncRetryPolicy<HttpResponseMessage> retryPolicy = HttpPolicyExtensions
// Обработка сетевых сбоев, кодов ответа 408 и 5xx
.HandleTransientHttpError().WaitAndRetryAsync(delays);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient(name: "Northwind.WebApi.Service", configureClient: options => { 
    options.BaseAddress = new("https://localhost:5091/");
    options.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1.0)); }).AddPolicyHandler(retryPolicy);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
