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
                        return Results.BadRequest(new { error = "User ID (Ticket ID) and username are required." });

                    using var conn = new MySqlConnection(conn_str);
                    
                    const string user_check_query =
                        "SELECT user_id AS Id, first_name AS FirstName, last_name AS LastName, balance AS Balance FROM users WHERE user_id = @user_id;";
                    User user = await conn.QueryFirstOrDefaultAsync<User>(user_check_query, new { user_id = req.user_id });

                    if (user == null)
                        return Results.Problem(detail: "User ID not found.", statusCode: 404);

                    // Then check username matches the user id
                    const string username_check_query =
                        "SELECT COUNT(*) FROM users WHERE user_id = @user_id AND username = @firstname + @last_name;";
                    long username_match = await conn.QueryFirstAsync<long>(username_check_query, new { user_id = req.user_id, username = req.username });

                    if (username_match == 0)
                        return Results.Problem(detail: "Wrong username.", statusCode: 401);

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
                        return Results.BadRequest(new { error = "User ID (Ticket ID) and username are required." });

                    using var conn = new MySqlConnection(conn_str);
                    
                    const string user_check_query =
                        "SELECT user_id, first_name, last_name, balance FROM users WHERE user_id = @user_id;";
                    User user = await conn.QueryFirstAsync<User>(user_check_query, new { user_id = req.user_id });

                    if (user == null)
                        return Results.Problem(detail: "User ID not found.", statusCode: 404);

                    // Check username matches the user id
                    const string username_check_query =
                        "SELECT COUNT(*) FROM users WHERE user_id = @user_id AND username = @firstname + @last_name;";
                    long username_match = await conn.QueryFirstAsync<long>(username_check_query, new { user_id = req.user_id, username = req.username });

                    if (username_match == 0)
                        return Results.Problem(detail: "Wrong username.", statusCode: 401);

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

            // Logout: Beendet die Session
            group.MapPost("/logout", (HttpContext context) =>
            {
                try
                {
                    var user_id = context.Session.GetString("user_id");
                    
                    if (string.IsNullOrEmpty(user_id))
                        return Results.BadRequest(new { error = "No active session found." });

                    // Session löschen
                    context.Session.Clear();

                    return Results.Ok(new { 
                        message = "Logout successful",
                        logged_in = false
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in POST /logout: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            // Session-Status prüfen
            group.MapGet("/session", (HttpContext context) =>
            {
                try
                {
                    var user_id = context.Session.GetString("user_id");
                    
                    if (string.IsNullOrEmpty(user_id))
                        return Results.Ok(new { logged_in = false });

                    return Results.Ok(new
                    {
                        logged_in = true,
                        user_id = context.Session.GetString("user_id"),
                        username = context.Session.GetString("username"),
                        first_name = context.Session.GetString("first_name"),
                        last_name = context.Session.GetString("last_name"),
                        logged_in_at = context.Session.GetString("logged_in_at")
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in GET /session: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });
        }
    }
}