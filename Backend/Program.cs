using Dapper;
using MySqlConnector;
using Backend.Router;
using Backend.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();

// Swagger/OpenAPI configuration
builder.Services.AddSwaggerServices();

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
string db_port = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306";
string db_name = Environment.GetEnvironmentVariable("DB_NAME") ?? "essens-bestellungs-tool";
string db_user = Environment.GetEnvironmentVariable("DB_USER") ?? "appuser";
string db_pass = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "apppass";

string conn_str = $"Server={db_host};Port={db_port};Database={db_name};User={db_user};Password={db_pass};";

// Wait for database to be ready
await WaitForDatabaseAsync(conn_str);

// perform a quick sanity query on tickets table to surface errors early
try
{
    using var conn = new MySqlConnection(conn_str);
    var count = await conn.QueryFirstAsync<long>("SELECT COUNT(*) FROM tickets;");
    Console.WriteLine($"Tickets table contains {count} row(s)");
}
catch (Exception ex)
{
    Console.WriteLine($"Startup query failed: {ex}");
}

// CORS enabling
app.UseCors(builder => builder
    .SetIsOriginAllowed(_ => true)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

// Enable Swagger UI
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Snackstasy API v1");
    options.RoutePrefix = "swagger";
});

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


