using Backend.Models;
using Dapper;
using MySqlConnector;

namespace Backend.Router
{
    public static class StandRoutes
    {
        public static void MapStandRoutes(this RouteGroupBuilder group, string conn_str)
        {
            // Get all stands
            group.MapGet("/stands/all", async () =>
            {
                try
                {
                    using var conn = new MySqlConnection(conn_str);
                    var stands = await conn.QueryAsync<Stand>(
                        "SELECT stand_id, name, pickup_id, tablet_id FROM stands;");

                    return Results.Ok(stands);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in GET /stands: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            // Get stand by ID
            group.MapGet("/stands/{stand_id}", async (int stand_id) =>
            {
                try
                {
                    using var conn = new MySqlConnection(conn_str);
                    var stand = await conn.QueryFirstOrDefaultAsync<Stand>(
                        "SELECT stand_id, name, pickup_id, tablet_id FROM stands WHERE stand_id = @id;",
                        new { id = stand_id });

                    if (stand == null)
                        return Results.NotFound(new { error = "Stand not found." });

                    return Results.Ok(stand);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in GET /stands/{stand_id}: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            // Create new stand (Manager/Admin only)
            group.MapPost("/stands/create", async (CreateStandRequest req) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(req.name))
                        return Results.BadRequest(new { error = "Stand name is required." });

                    using var conn = new MySqlConnection(conn_str);
                    const string query = @"
                        INSERT INTO stands (name, pickup_id, tablet_id) 
                        VALUES (@name, @pickup_id, @tablet_id);
                        SELECT LAST_INSERT_ID();";

                    int id = await conn.QueryFirstAsync<int>(query, new
                    {
                        name = req.name,
                        pickup_id = req.pickup_id ?? "",
                        tablet_id = req.tablet_id ?? ""
                    });

                    return Results.Ok(new
                    {
                        stand_id = id,
                        req.name,
                        req.pickup_id,
                        req.tablet_id
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in POST /stands: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            // Update stand (Manager/Admin only)
            group.MapPut("/stands/{stand_id}", async (int stand_id, UpdateStandRequest req) =>
            {
                try
                {
                    using var conn = new MySqlConnection(conn_str);
                    
                    var updates = new List<string>();
                    var parameters = new DynamicParameters();
                    parameters.Add("id", stand_id);

                    if (!string.IsNullOrEmpty(req.name))
                    {
                        updates.Add("name = @name");
                        parameters.Add("name", req.name);
                    }
                    if (!string.IsNullOrEmpty(req.pickup_id))
                    {
                        updates.Add("pickup_id = @pickup_id");
                        parameters.Add("pickup_id", req.pickup_id);
                    }
                    if (!string.IsNullOrEmpty(req.tablet_id))
                    {
                        updates.Add("tablet_id = @tablet_id");
                        parameters.Add("tablet_id", req.tablet_id);
                    }

                    if (updates.Count == 0)
                        return Results.BadRequest(new { error = "No fields to update." });

                    string query = $"UPDATE stands SET {string.Join(", ", updates)} WHERE stand_id = @id;";
                    int rows_affected = await conn.ExecuteAsync(query, parameters);

                    if (rows_affected == 0)
                        return Results.NotFound(new { error = "Stand not found." });

                    return Results.Ok(new { message = $"Stand {stand_id} updated successfully." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in PUT /stands/{stand_id}: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            // Delete stand (Admin only)
            group.MapDelete("/stands/{stand_id}", async (int stand_id) =>
            {
                try
                {
                    using var conn = new MySqlConnection(conn_str);
                    
                    // Check if stand has items
                    var item_count = await conn.QueryFirstAsync<int>(
                        "SELECT COUNT(*) FROM items WHERE stand_id = @id;",
                        new { id = stand_id });

                    if (item_count > 0)
                        return Results.BadRequest(new { error = $"Cannot delete stand with {item_count} items. Remove items first." });

                    int rows_affected = await conn.ExecuteAsync(
                        "DELETE FROM stands WHERE stand_id = @id;",
                        new { id = stand_id });

                    if (rows_affected == 0)
                        return Results.NotFound(new { error = "Stand not found." });

                    return Results.Ok(new { message = $"Stand {stand_id} deleted successfully." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in DELETE /stands/{stand_id}: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });
        }
    }
}
