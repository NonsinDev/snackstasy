using Dapper;
using MySqlConnector;
using Backend.Models;


namespace Backend.Router
{
    public static class BalanceRoutes
    {
        public static void MapBalanceRoutes(this RouteGroupBuilder group, string conn_str)
        {

            group.MapPut("/balance/{user_id}/remove/{amount}", async (BalanceUpdateRequest req) =>
            {
                try
                {
                    using MySqlConnection conn = new MySqlConnection(conn_str);

                    var sql = @"
                        UPDATE users
                        SET balance = balance - @Amount
                        WHERE user_id = @UserId AND balance >= @Amount;
                    ";

                    int rows_affected = await conn.ExecuteAsync(sql, new
                    {
                        Amount = req.amount,
                        UserId = req.user_id
                    });

                    if (rows_affected == 0)
                    {
                        const string balance_check_query =
                            "SELECT balance FROM users WHERE user_id = @UserId;";

                        var balance = await conn.ExecuteScalarAsync<decimal?>(
                            balance_check_query,
                            new { UserId = req.user_id }
                        );

                        if (balance == null)
                            return Results.NotFound(new { error = "User not found." });

                        return Results.BadRequest(new { error = "Insufficient balance." });
                    }

                    return Results.Ok();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in PUT /balance/{{user_id}}/remove/{{amount}}: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });


            group.MapPut("/balance/{user_id}/add/{amount}", async (BalanceUpdateRequest req) =>
            {
                try
                {
                    using var conn = new MySqlConnection(conn_str);
                    const string query =
                        "UPDATE users SET balance = balance + @amount WHERE user_id = @user_id;";

                    int rows_affected = await conn.ExecuteAsync(query, new { req.amount, req.user_id });

                    if (rows_affected == 0)
                        return Results.NotFound(new { error = "User not found." });

                    return Results.Ok();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in PUT /balance/{{user_id}}/add/{{amount}}: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });
        }
    }
}