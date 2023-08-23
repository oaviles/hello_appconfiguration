using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using TestAppConfig;
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

// Retrieve the connection string
//string connectionString = builder.Configuration.GetConnectionString("AppConfig");

string connectionString = Environment.GetEnvironmentVariable("APPCONFIG_CS");
string configurationKey = Environment.GetEnvironmentVariable("APPCONFIG_KEY");

// Load configuration from Azure App Configuration
/* 
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(connectionString)
           // Load all keys that start with `TestApp:` and have no label
           .Select("TestApp:*", LabelFilter.Null)
           // Configure to reload configuration if the registered sentinel key is modified
           .ConfigureRefresh(refreshOptions =>
                refreshOptions.Register("TestApp:Settings:Sentinel", refreshAll: true));
});
*/


// Load configuration from Azure App Configuration with Feature Flags support
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(connectionString)
           // Load all keys that start with `TestApp:` and have no label
           .Select($"{configurationKey}:*", LabelFilter.Null)
           // Configure to reload configuration if the registered sentinel key is modified
           .ConfigureRefresh(refreshOptions =>
                refreshOptions.Register($"{configurationKey}:Settings:Sentinel", refreshAll: true));

    // Load all feature flags with no label
    options.UseFeatureFlags();
});

// Add services to the container.
builder.Services.AddRazorPages();

// Add Azure App Configuration middleware to the container of services.
builder.Services.AddAzureAppConfiguration(); /*** App Config Call ***/


// Add feature management to the container of services.
builder.Services.AddFeatureManagement();

// Load configuration from Azure App Configuration with Feature Flags support
/*
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(connectionString)
           // Load all keys that start with `TestApp:` and have no label
           .Select("TestApp:*", LabelFilter.Null)
           // Configure to reload configuration if the registered sentinel key is modified
           .ConfigureRefresh(refreshOptions =>
                refreshOptions.Register("TestApp:Settings:Sentinel", refreshAll: true));

    // Load all feature flags with no label
    options.UseFeatureFlags();
});
*/

// Bind configuration "TestApp:Settings" section to the Settings object
builder.Services.Configure<Settings>(builder.Configuration.GetSection($"{configurationKey}:Settings"));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Use Azure App Configuration middleware for dynamic configuration refresh.
app.UseAzureAppConfiguration();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
