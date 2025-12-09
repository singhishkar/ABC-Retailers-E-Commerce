using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using ABCRetail.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<TableStorageService>(provider => {
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("AzureStorage");  // Ensure this name matches your actual connection string key in appsettings.json
    return new TableStorageService(connectionString);


});


builder.Services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Make the session cookie essential
});



builder.Services.AddSingleton(provider => {
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new BlobService(configuration.GetConnectionString("AzureStorage"));
});


// Register UserService
builder.Services.AddScoped<UserService>(provider => {
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new UserService(configuration.GetConnectionString("AzureStorage"));
});

builder.Services.AddSingleton(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new QueueService(configuration.GetConnectionString("AzureStorage"), "orders-queue");
});

builder.Services.AddSingleton(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("AzureStorage");
    var fileShareName = "product-share"; // Replace with your actual file share name

    return new AzureFileShareService(connectionString, fileShareName);
});


// Add services to the container.
builder.Services.AddControllersWithViews();





var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Ensure this is placed before app.UseEndpoints

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
