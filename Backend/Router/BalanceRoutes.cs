using Dapper;
using MySqlConnector;

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

                    int rows_affected = await conn.ExecuteAsync($"UPDATE users SET balance = balance - {req.amount} WHERE user_id = {req.user_id} AND balance >= {req.amount};");

                    if (rows_affected == 0)
                    {
                        const string balance_check_query =
                            "SELECT balance FROM users WHERE user_id = @user_id;";
                        var exists = await conn.ExecuteScalarAsync<long>(balance_check_query, new { user_id });
                        if (exists == 0)
                            return Results.NotFound(new { error = "User not found." });
                        
                        return Results.BadRequest(new { error = "Insufficient balance." });
                    }

                    return Results.Ok(new { message = $"Successfully removed {amount} from user {user_id}." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in PUT /balance/{{user_id}}/remove/{{amount}}: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });


            group.MapPut("/balance/{user_id}/add/{amount}", async (string user_id, decimal amount) =>
            {
                try
                {
                    using var conn = new MySqlConnection(conn_str);
                    const string query =
                        "UPDATE users SET balance = balance + @amount WHERE user_id = @user_id;";

                    int rows_affected = await conn.ExecuteAsync(query, new { amount, user_id });

                    if (rows_affected == 0)
                        return Results.NotFound(new { error = "User not found." });

                    return Results.Ok(new { message = $"Successfully added {amount} to user {user_id}." });
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