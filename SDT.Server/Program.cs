using Microsoft.EntityFrameworkCore;
using SDT.LBl;
using SDT.Repositories;
using SDT.Data;
using SDT.Api.Middleware.Logging;
using System.Runtime.InteropServices;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
    );
});

// Настройка логирования с использованием ConsoleFormatterOptions
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = true; // Включаем области логирования
    options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] "; // Формат времени
    options.SingleLine = true; // Однострочный вывод для удобства
});
Console.OutputEncoding = Encoding.UTF8;

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddBlServices();
builder.Services.AddRepositories();

// Add DbContext
var connectionString = builder.Configuration["ConnectionStrings:SmartDocumentTranslation"];
builder.Services.AddDbContext<SDTContext>((serviceProvider, options) =>
{
    options.UseNpgsql(connectionString, o =>
    {
        o.MigrationsAssembly("SDT.LData");
        o.CommandTimeout(180);
        o.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null
        );
        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var buildInfoFile = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
    ? Path.Combine(Directory.GetCurrentDirectory(), "buildinfo.txt")
    : "/tmp/buildinfo";

if (File.Exists(buildInfoFile))
{
    var buildInfo = File.ReadAllLines(buildInfoFile)
                        .FirstOrDefault(line => line.StartsWith("APP_VERSION="));
    if (buildInfo != null)
    {
        var buildDate = buildInfo.Split('=')[1];
        Environment.SetEnvironmentVariable("APP_VERSION", buildDate);
    }
}

Console.WriteLine($"Start Up in {builder.Environment.EnvironmentName} environment... v. {Environment.GetEnvironmentVariable("APP_VERSION")}, port: {Environment.GetEnvironmentVariable("PORT") ?? "8080"}");

var app = builder.Build();

var myDomain = Environment.GetEnvironmentVariable("MY_DOMAIN");
if (!string.IsNullOrEmpty(myDomain))
{
    app.Use(async (context, next) =>
    {
        var forwardedProto = context.Request.Headers["X-Forwarded-Proto"].FirstOrDefault();
        var scheme = !string.IsNullOrEmpty(forwardedProto) ? forwardedProto : context.Request.Scheme;
        var host = context.Request.Host.Value;

        Console.WriteLine("{0} {1} {2}", scheme, host, myDomain);

        // Проверяем, если запрос уже корректен
        if (scheme.Equals("https", StringComparison.OrdinalIgnoreCase) && host.Equals(myDomain, StringComparison.OrdinalIgnoreCase))
        {
            await next();
            return;
        }

        // Формируем новый URL с HTTPS и правильным доменом
        var newUrl = $"https://{myDomain}{context.Request.Path}{context.Request.QueryString}";
        context.Response.Redirect(newUrl, permanent: true);
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SDTContext>();
    dbContext.Database.Migrate();
}

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

await app.RunAsync();
