using Dapper;
using MySqlConnector;

namespace Backend.Router
{
    public static class TicketRoutes
    {
        public class BookRequest
        {
            public string first_name { get; set; } = "";
            public string last_name { get; set; } = "";
        }

        public class Users
        {
            public long Id { get; set; }
            public string FirstName { get; set; } = "";
            public string LastName { get; set; } = "";
            public decimal Balance { get; set; }
        }
        
        public static void MapTicketRoutes(this WebApplication app, string connStr)
        {
            app.MapGet("/tickets/all", async () =>
            {
                try
                {
                    using var conn = new MySqlConnection(connStr);
                    var tickets = await conn.QueryAsync<Users>(
                        "SELECT id AS Id, first_name AS FirstName, last_name AS LastName, balance AS Balance FROM users;");

                    var result = tickets.Select(t => new {
                        ticket_id = t.Id.ToString("D6"),
                        first_name = t.FirstName,
                        last_name = t.LastName,
                        balance = t.Balance
                    });

                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in GET /tickets/all: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            app.MapPost("/tickets/book", async (BookRequest req) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(req.first_name) || string.IsNullOrWhiteSpace(req.last_name))
                        return Results.BadRequest(new { error = "First name and last name are required." });

                    using var conn = new MySqlConnection(connStr);
                    const string query =
                        "INSERT INTO users (first_name, last_name, balance) VALUES (@fn, @ln, 0); SELECT LAST_INSERT_ID();";

                    long id = await conn.QueryFirstAsync<long>(query, new { fn = req.first_name, ln = req.last_name});

                    return Results.Ok(new
                    {
                        ticket_id = id.ToString("D6"),
                        first_name = req.first_name,
                        last_name = req.last_name
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
