using Jarvis;

var builder = WebApplication.CreateBuilder(args);

var launcher = new Launcher();
await launcher.InitAsync(builder.Services, builder.Configuration);

var app = builder.Build();

_ = launcher.StartAsync(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
