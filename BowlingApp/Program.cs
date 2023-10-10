using Microsoft.Net.Http.Headers;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy => {
            policy.WithOrigins("https://localhost:44408",
                "https://localhost:7156")
            .AllowAnyHeader()
            //.WithHeaders(HeaderNames.ContentType, "application/json")
            .AllowAnyMethod()
            .AllowCredentials();
            });
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.UseCors();
app.UseAuthorization();

app.Run();
