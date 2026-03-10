using Backend.Class;
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
                    User user = await conn.QueryFirstAsync<User>(
                        "SELECT id AS Id, first_name AS FirstName, last_name AS LastName, balance AS Balance FROM users;");

                    return Results.Ok(new
                    {
                        user.user_id,
                        user.first_name,
                        user.last_name,
                        user.balance 
                    });
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

                    int id = await conn.QueryFirstAsync<int>(query, new { fn = req.first_name, ln = req.last_name});

                    return Results.Ok(new
                    {
                        id,
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
