using Backend.Models;
using Backend.Services;
using Dapper;
using MySqlConnector;


namespace Backend.Router
{
    public static class TicketRoutes
    {
        public static void MapTicketRoutes(this RouteGroupBuilder group, string conn_str)
        {
            group.MapGet("/tickets/{ticket_id}", async (string ticket_id) =>
            {
                try
                {
                    using var conn = new MySqlConnection(conn_str);

                    var user = await conn.QueryFirstOrDefaultAsync<User>(
                        "SELECT user_id, first_name, last_name, balance, ticket_id FROM users WHERE ticket_id = @ticket_id;",
                        new { ticket_id });

                    if (user == null)
                        return Results.NotFound(new { error = "User not found for this ticket_id" });

                    return Results.Ok(user);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in GET /tickets/{{ticket_id}}: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });
            group.MapPost("/tickets/book", async (BookRequest req) =>
            {
               try
                {
                    if (string.IsNullOrWhiteSpace(req.first_name) || string.IsNullOrWhiteSpace(req.last_name))
                        return Results.BadRequest(new { error = "First name and last name are required." });

                    string ticketId = TicketService.GenerateTicketId(req.first_name, req.last_name);

                    using var conn = new MySqlConnection(conn_str);

                    const string query =
                        @"INSERT INTO users (first_name, last_name, balance, ticket_id)
                         VALUES (@fn, @ln, 0, @ticketId);
                          SELECT LAST_INSERT_ID();";

                    int user_id = await conn.QueryFirstAsync<int>(query,
                        new { fn = req.first_name, ln = req.last_name, ticketId });

                    return Results.Ok(new
                    {
                        user_id,
                        ticketId,
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
