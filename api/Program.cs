/* Dotnet 6 implementation */
var builder = WebApplication.CreateBuilder(args);

// add service to container (equal to ConfigureServices method in Startup.cs)
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddCors();
builder.Services.AddIdentifyServices(builder.Configuration);
builder.Services.AddSignalR();

// Configure the HTTP request pipeline (equal to Configure method in Startup.cs)
var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseCors(policy => policy.AllowAnyHeader()
    .AllowCredentials()
    .AllowAnyMethod()
    .WithOrigins("https://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();
// using static file generated from angular for view
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");
// fix routing issue with generated file
app.MapFallbackToController("Index", "Fallback");

// Other settings (equal to other settings in the .net5.0 Program.cs)
// add set of seed user for testing purpose
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(userManager, roleManager);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An Error occurred during migration");
}

await app.RunAsync();
