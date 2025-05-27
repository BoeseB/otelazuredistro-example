var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Registriere HttpClient für die Kommunikation mit der WeatherAPI
builder.Services.AddHttpClient<WeatherFrontend.Services.WeatherService>(client =>
{
    var apiSettings = builder.Configuration.GetSection("WeatherApi");
    var baseUrl = apiSettings["BaseUrl"] ?? "http://localhost:5031";
    client.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
