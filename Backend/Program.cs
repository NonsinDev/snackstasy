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

string db_host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
string db_port = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306";
string db_name = Environment.GetEnvironmentVariable("DB_NAME") ?? "essens_bestellungs_tool";
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

// Wait for database to be ready
await WaitForDatabaseAsync(conn_str);

// **Init SQL ausführen**
string backendDir = Directory.GetParent(AppContext.BaseDirectory)   // net8.0
                             .Parent  // Debug
                             .Parent  // bin
                             .FullName; // jetzt im Backend-Ordner

string sqlFilePath = Path.Combine(backendDir, "..", "..", "mysql-init", "init.sql");
await ExecuteSqlFileAsync(conn_str, sqlFilePath);

Console.WriteLine("✅ Database initialized! " + sqlFilePath + " " +  conn_str);

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

app.Run();

async Task WaitForDatabaseAsync(string conn_str)
{
    int max_retries = 30;
    int retry_delay = 2000; // 2 seconds

    for (int i = 0; i < max_retries; i++)
    {
        try
        {
            using var conn = new MySqlConnection(conn_str);
            await conn.OpenAsync();
            conn.Close();
            Console.WriteLine("Database is ready!");
            return;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database not ready yet ({i + 1}/{max_retries}): {ex.Message}");
            if (i < max_retries - 1)
                await Task.Delay(retry_delay);
        }
    }

    throw new Exception("Database failed to become available after 30 retries.");
}

// --------------------
// SQL-Datei ausführen
// --------------------
async Task ExecuteSqlFileAsync(string conn_str, string filePath)
{
    if (!File.Exists(filePath))
    {
        Console.WriteLine($"❌ SQL file not found: {filePath}");
        return;
    }

    string sql = await File.ReadAllTextAsync(filePath);

    using var conn = new MySqlConnection(conn_str);
    await conn.OpenAsync();

    // SQL-Datei in einzelne Statements aufsplitten und ausführen
    foreach (var statement in sql.Split(";", StringSplitOptions.RemoveEmptyEntries))
    {
        if (!string.IsNullOrWhiteSpace(statement))
        {
            await conn.ExecuteAsync(statement);
        }
    }

    Console.WriteLine($"✅ SQL file executed: {filePath}");
}