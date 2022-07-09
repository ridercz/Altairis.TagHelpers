// Register services
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
// Uncomment the following section to use custom date formatting
//builder.Services.Configure<TimeTagHelperOptions>(options => {
//    options.GeneralDateFormatter = d => string.Format("{0:d}", d);
//});

// Add middleware
var app = builder.Build();
app.UseStaticFiles();
app.MapRazorPages();

// Run application
await app.RunAsync();