using Backend.Models;
using Dapper;
using MySqlConnector;

namespace Backend.Router
{
    public static class TicketRoutes
    {
        public static void MapTicketRoutes(this RouteGroupBuilder group, string conn_str)
        {
            group.MapGet("/tickets/all", async () =>
            {
                try
                {
                    using var conn = new MySqlConnection(conn_str);
                    var users = await conn.QueryAsync<User>(
                        "SELECT user_id, first_name, last_name, balance FROM users;");

                    return Results.Ok(users);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in GET /tickets/all: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            group.MapPost("/tickets/book", async (BookRequest req) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(req.first_name) || string.IsNullOrWhiteSpace(req.last_name))
                        return Results.BadRequest(new { error = "First name and last name are required." });

                    using var conn = new MySqlConnection(conn_str);
                    const string query =
                        "INSERT INTO users (first_name, last_name, balance) VALUES (@fn, @ln, 0); SELECT LAST_INSERT_ID();";

                    int user_id = await conn.QueryFirstAsync<int>(query, new { fn = req.first_name, ln = req.last_name});

                    return Results.Ok(new
                    {
                        user_id,
                        req.first_name,
                        req.last_name
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in POST /tickets/book: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });
        }
    }
}
