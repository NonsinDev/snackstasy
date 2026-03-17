using Backend.Models;
using Dapper;
using MySqlConnector;

namespace Backend.Router
{
    public static class LoginRoutes
    {
        public static void MapLoginRoutes(this RouteGroupBuilder group, string conn_str)
        {
            // Login-Check: Überprüft ob ein Account mit diesen Daten existiert
            group.MapPost("/login-check", async (LoginRequest req) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(req.user_id) || string.IsNullOrWhiteSpace(req.username))
                        return Results.BadRequest(new { error = "User ID and username are required." });

                    using var conn = new MySqlConnection(conn_str);

                    User user = await conn.QueryFirstAsync<User>($"SELECT user_id, first_name, last_name, balance, ticket_id FROM users WHERE user_id = { req.user_id };");

                    return Results.Ok(new
                    {
                        exists = true,
                        user.user_id,
                        user.first_name,
                        user.last_name,
                        user.balance
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in POST /login-check: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            // Login: Erstellt eine Session für den Benutzer
            group.MapPost("/login", async (HttpContext context, LoginRequest req) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(req.user_id) || string.IsNullOrWhiteSpace(req.username))
                        return Results.BadRequest(new { error = "Ticket ID and username are required." });

                    using var conn = new MySqlConnection(conn_str);
                    
                    const string user_check_query =
                        "SELECT user_id, first_name, last_name, balance, ticket_id FROM users WHERE user_id = @user_id;";
                    User user = await conn.QueryFirstAsync<User>(user_check_query, new { req.user_id });

                    if (user == null)
                        return Results.Problem(detail: "Ticket ID not found.", statusCode: 404);
                        
                    // Session erstellen
                    context.Session.SetString("user_id", req.user_id);
                    context.Session.SetString("username", req.username);
                    context.Session.SetString("first_name", user.first_name ?? "");
                    context.Session.SetString("last_name", user.last_name ?? "");
                    context.Session.SetString("logged_in_at", DateTime.UtcNow.ToString("o"));

                    return Results.Ok(new
                    {
                        message = "Login successful",
                        logged_in = true,
                        user.user_id,
                        user.first_name,
                        user.last_name,
                        user.balance
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in POST /login: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });
        }
    }
}