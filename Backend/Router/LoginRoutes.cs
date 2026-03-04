using Dapper;
using MySqlConnector;
using static Backend.Router.TicketRoutes;

namespace Backend.Router
{
    public static class LoginRoutes
    {
        public static void MapLoginRoutes(this WebApplication app, string connStr)
        {
            app.MapPost("/login", async (LoginRequest req) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(req.userId) || string.IsNullOrWhiteSpace(req.username))
                        return Results.BadRequest(new { error = "User ID (Ticket ID) and username are required." });

                    using var conn = new MySqlConnection(connStr);
                    
                    // First check if user id exists
                    const string userCheckQuery =
                        "SELECT id AS Id, first_name AS FirstName, last_name AS LastName, balance AS Balance FROM users WHERE id = @id;";
                    var user = await conn.QueryFirstOrDefaultAsync<User>(userCheckQuery, new { id = req.userId });

                    if (user == null)
                        return Results.Problem(detail: "User ID not found.", statusCode: 404);

                    // Then check username matches the user id
                    const string usernameCheckQuery =
                        "SELECT COUNT(*) FROM users WHERE id = @id AND username = @firstname + @last_name;";
                    long usernameMatch = await conn.QueryFirstAsync<long>(usernameCheckQuery, new { id = req.userId, username = req.username });

                    if (usernameMatch == 0)
                        return Results.Problem(detail: "Wrong username.", statusCode: 401);

                    return Results.Ok(new
                    {
                        user_id = user.Id.ToString("D6"),
                        first_name = user.FirstName,
                        last_name = user.LastName,
                        balance = user.Balance
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

    internal class LoginRequest
    {
        public required string userId { get; set; }
        public required string username { get; set; }
    }

    internal record User(long Id, string FirstName, string LastName, decimal Balance);
}