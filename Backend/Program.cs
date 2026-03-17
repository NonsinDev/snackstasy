using Dapper;
using MySqlConnector;
using Backend.Router;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();

// Session configuration
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

var app = builder.Build();

string db_host = Environment.GetEnvironmentVariable("DB_HOST") ?? "db";
string db_port = Environment.GetEnvironmentVariable("DB_PORT") ?? "3305";
string db_name = Environment.GetEnvironmentVariable("DB_NAME") ?? "snackstasy";
string db_user = Environment.GetEnvironmentVariable("DB_USER") ?? "appuser";
string db_pass = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "apppass";

string conn_str = $"Server={db_host};Port={db_port};Database={db_name};User={db_user};Password={db_pass};";

// Optional: Test der Verbindung vor WaitForDatabaseAsync
Console.WriteLine($"Trying to connect: Server={db_host};Port={db_port};Database={db_name};User={db_user}");
try
{
    using var testConn = new MySqlConnection(conn_str);
    await testConn.OpenAsync();
    Console.WriteLine("✅ Connection successful!");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Connection failed: {ex.Message}");
}

// CORS enabling
app.UseCors(builder => builder
    .SetIsOriginAllowed(_ => true)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

// Enable session middleware
app.UseSession();

// register endpoints from separate router classes with /v1 prefix
RouteGroupBuilder api_v1 = app.MapGroup("/v1");
api_v1.MapTicketRoutes(conn_str);
api_v1.MapLoginRoutes(conn_str);
api_v1.MapBalanceRoutes(conn_str);
api_v1.MapStandRoutes(conn_str);
api_v1.MapItemRoutes(conn_str);

// Port hier explizit setzen (z. B. 5002). "0.0.0.0" erlaubt Zugriff auch von anderen Geräten im Netzwerk.
app.Run("http://0.0.0.0:5002");
