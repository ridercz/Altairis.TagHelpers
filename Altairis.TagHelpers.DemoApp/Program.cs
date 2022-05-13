var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();
//builder.Services.Configure<TimeTagHelperOptions>(options => {
//    options.GeneralDateFormatter = d => string.Format("{0:d}", d);
//});

var app = builder.Build();

app.UseStaticFiles();
app.MapRazorPages();

await app.RunAsync();