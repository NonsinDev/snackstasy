using Dapper;
using MySqlConnector;

namespace Backend.Router
{
    public static class BalanceRoutes
    {
        public static void MapBalanceRoutes(this RouteGroupBuilder group, string conn_str)
        {
            group.MapGet("/balance/{user_id}", async (string user_id) =>
            {
                try
                {   
                    const string balance_query =
                        "SELECT balance FROM users WHERE id = @id;";
                    using var conn = new MySqlConnection(conn_str);
                    var balance = await conn.QueryFirstOrDefaultAsync<decimal?>(balance_query, new { id = user_id });

                    if (balance == null)
                        return Results.Problem(detail: "User not found.", statusCode: 404);

                    return Results.Ok(new { balance });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in GET /balance/{{user_id}}: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            group.MapPut("/balance/{ticket_id}/update/{new_balance}", async (string ticket_id, decimal new_balance) =>
            {
                try
                {
                    using var conn = new MySqlConnection(conn_str);
                    const string query =
                        "UPDATE users SET balance = @balance WHERE id = @id;";

                    int rows_affected = await conn.ExecuteAsync(query, new { balance = new_balance, id = ticket_id });

                    if (rows_affected == 0)
                        return Results.NotFound(new { error = "Ticket not found." });

                    return Results.Ok(new { message = $"Successfully updated balance for ticket {ticket_id} to {new_balance}." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in PUT /balance/{{ticket_id}}/update/{{new_balance}}: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            group.MapPut("/balance/{ticket_id}/remove/{amount}", async (string ticket_id, decimal amount) =>
            {
                try
                {
                    using var conn = new MySqlConnection(conn_str);
                    const string query =
                        "UPDATE users SET balance = balance - @amount WHERE id = @id AND balance >= @amount;";

                    int rows_affected = await conn.ExecuteAsync(query, new { amount, id = ticket_id });

                    if (rows_affected == 0)
                    {
                        const string balance_check_query =
                            "SELECT balance FROM users WHERE id = @id;";
                        var exists = await conn.ExecuteScalarAsync<long>(balance_check_query, new { id = ticket_id });
                        if (exists == 0)
                            return Results.NotFound(new { error = "Ticket not found." });
                        
                        return Results.BadRequest(new { error = "Insufficient balance." });
                    }

                    return Results.Ok(new { message = $"Successfully removed {amount} from ticket {ticket_id}." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in PUT /balance/{{ticket_id}}/remove/{{amount}}: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });


            group.MapPut("/balance/{ticket_id}/add/{amount}", async (string ticket_id, decimal amount) =>
            {
                try
                {
                    using var conn = new MySqlConnection(conn_str);
                    const string query =
                        "UPDATE users SET balance = balance + @amount WHERE id = @id;";

                    int rows_affected = await conn.ExecuteAsync(query, new { amount, id = ticket_id });

                    if (rows_affected == 0)
                        return Results.NotFound(new { error = "Ticket not found." });

                    return Results.Ok(new { message = $"Successfully added {amount} to ticket {ticket_id}." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in PUT /balance/{{ticket_id}}/add/{{amount}}: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });
        }
    }
}