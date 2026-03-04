using Dapper;
using MySqlConnector;

namespace Backend.Router
{
    public static class BalanceRoutes
    {
        public static void MapBalanceRoutes(this WebApplication app, string connStr)
        {
            app.MapGet("/balance/{ticketId}", async (string ticketId) =>
            {
                try
                {   
                    const string balanceQuery =
                        "SELECT balance FROM users WHERE id = @id;";
                    using var conn = new MySqlConnection(connStr);
                    var balance = await conn.QueryFirstOrDefaultAsync<decimal?>(balanceQuery, new { id = ticketId });

                    if (balance == null)
                        return Results.Problem(detail: "Ticket not found.", statusCode: 404);

                    return Results.Ok(new { balance });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in GET /balance/{{ticketId}}: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            app.MapPut("/balance/{ticketId}/update/{newBalance}", async (string ticketId, decimal newBalance) =>
            {
                try
                {
                    using var conn = new MySqlConnection(connStr);
                    const string query =
                        "UPDATE users SET balance = @balance WHERE id = @id;";

                    int rowsAffected = await conn.ExecuteAsync(query, new { balance = newBalance, id = ticketId });

                    if (rowsAffected == 0)
                        return Results.NotFound(new { error = "Ticket not found." });

                    return Results.Ok(new { message = $"Successfully updated balance for ticket {ticketId} to {newBalance}." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in PUT /balance/{{ticketId}}/update/{{newBalance}}: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            app.MapPut("/balance/{ticketId}/remove/{amount}", async (string ticketId, decimal amount) =>
            {
                try
                {
                    using var conn = new MySqlConnection(connStr);
                    const string query =
                        "UPDATE users SET balance = balance - @amount WHERE id = @id AND balance >= @amount;";

                    int rowsAffected = await conn.ExecuteAsync(query, new { amount, id = ticketId });

                    if (rowsAffected == 0)
                    {
                        const string balanceCheckQuery =
                            "SELECT balance FROM users WHERE id = @id;";
                        var exists = await conn.ExecuteScalarAsync<long>(balanceCheckQuery, new { id = ticketId });
                        if (exists == 0)
                            return Results.NotFound(new { error = "Ticket not found." });
                        
                        return Results.BadRequest(new { error = "Insufficient balance." });
                    }

                    return Results.Ok(new { message = $"Successfully removed {amount} from ticket {ticketId}." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in PUT /balance/{{ticketId}}/remove/{{amount}}: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            app.MapPut("/balance/{ticketId}/add/{amount}", async (string ticketId, decimal amount) =>
            {
                try
                {
                    using var conn = new MySqlConnection(connStr);
                    const string query =
                        "UPDATE users SET balance = balance + @amount WHERE id = @id;";

                    int rowsAffected = await conn.ExecuteAsync(query, new { amount, id = ticketId });

                    if (rowsAffected == 0)
                        return Results.NotFound(new { error = "Ticket not found." });

                    return Results.Ok(new { message = $"Successfully added {amount} to ticket {ticketId}." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in PUT /balance/{{ticketId}}/add/{{amount}}: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });
        }
    }

    internal class BalanceUpdateRequest
    {
        public required string TicketId { get; set; }
        public required decimal NewBalance { get; set; }
    }
}