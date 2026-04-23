using Backend.Models;
using Dapper;
using MySqlConnector;

namespace Backend.Router
{
    public static class ItemRoutes
    {
        public static void MapItemRoutes(this RouteGroupBuilder group, string conn_str)
        {
            // Get all items
            group.MapGet("/items/all", async () =>
            {
                try
                {
                    using var conn = new MySqlConnection(conn_str);
                    var items = await conn.QueryAsync<Item>(
                        "SELECT item_id, stand_id, name, price, stock FROM items;");

                    return Results.Ok(items);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in GET /items: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            // Get items by stand
            group.MapGet("/stands/{stand_id}/items", async (int stand_id) =>
            {
                try
                {
                    using var conn = new MySqlConnection(conn_str);
                    var items = await conn.QueryAsync<Item>(
                        "SELECT item_id, stand_id, name, price, stock FROM items WHERE stand_id = @stand_id;",
                        new { stand_id });

                    return Results.Ok(items);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in GET /stands/{stand_id}/items: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            // Get item by ID
            group.MapGet("/items/{item_id}", async (int item_id) =>
            {
                try
                {
                    using var conn = new MySqlConnection(conn_str);
                    var item = await conn.QueryFirstOrDefaultAsync<Item>(
                        "SELECT item_id, stand_id, name, price, stock FROM items WHERE item_id = @id;",
                        new { id = item_id });

                    if (item == null)
                        return Results.NotFound(new { error = "Item not found." });

                    return Results.Ok(item);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in GET /items/{item_id}: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            // Create new item
            group.MapPost("/items/create", async (CreateItemRequest req) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(req.name))
                        return Results.BadRequest(new { error = "Item name is required." });

                    if (req.stand_id <= 0)
                        return Results.BadRequest(new { error = "Valid stand_id is required." });

                    if (req.price < 0)
                        return Results.BadRequest(new { error = "Price cannot be negative." });

                    if (req.stock < 0)
                        return Results.BadRequest(new { error = "Stock cannot be negative." });

                    using var conn = new MySqlConnection(conn_str);

                    // Verify stand exists
                    var stand_exists = await conn.QueryFirstOrDefaultAsync<int>(
                        "SELECT COUNT(*) FROM stands WHERE stand_id = @id;",
                        new { id = req.stand_id });

                    if (stand_exists == 0)
                        return Results.BadRequest(new { error = "Stand not found." });

                    const string query = @"
                        INSERT INTO items (stand_id, name, price, stock) 
                        VALUES (@stand_id, @name, @price, @stock);
                        SELECT LAST_INSERT_ID();";

                    int id = await conn.QueryFirstAsync<int>(query, new
                    {
                        req.stand_id,
                        req.name,
                        req.price,
                        req.stock
                    });

                    return Results.Ok(new
                    {
                        item_id = id,
                        req.stand_id,
                        req.name,
                        req.price,
                        req.stock
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in POST /items: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            // Update item
            group.MapPut("/items/{item_id}", async (int item_id, UpdateItemRequest req) =>
            {
                try
                {
                    using var conn = new MySqlConnection(conn_str);

                    List<string> updates = new List<string>();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("id", item_id);

                    if (!string.IsNullOrEmpty(req.name))
                    {
                        updates.Add("name = @name");
                        parameters.Add("name", req.name);
                    }
                    if (req.price.HasValue)
                    {
                        if (req.price < 0)
                            return Results.BadRequest(new { error = "Price cannot be negative." });
                        updates.Add("price = @price");
                        parameters.Add("price", req.price);
                    }
                    if (req.stock.HasValue)
                    {
                        if (req.stock < 0)
                            return Results.BadRequest(new { error = "Stock cannot be negative." });
                        updates.Add("stock = @stock");
                        parameters.Add("stock", req.stock);
                    }

                    if (updates.Count == 0)
                        return Results.BadRequest(new { error = "No fields to update." });

                    string query = $"UPDATE items SET {string.Join(", ", updates)} WHERE item_id = @id;";
                    int rows_affected = await conn.ExecuteAsync(query, parameters);

                    if (rows_affected == 0)
                        return Results.NotFound(new { error = "Item not found." });

                    return Results.Ok(new { message = $"Item {item_id} updated successfully." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in PUT /items/{item_id}: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            // Delete item
            group.MapDelete("/items/{item_id}", async (int item_id) =>
            {
                try
                {
                    using var conn = new MySqlConnection(conn_str);

                    int rows_affected = await conn.ExecuteAsync(
                        "DELETE FROM items WHERE item_id = @id;",
                        new { id = item_id });

                    if (rows_affected == 0)
                        return Results.NotFound(new { error = "Item not found." });

                    return Results.Ok(new { message = $"Item {item_id} deleted successfully." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in DELETE /items/{item_id}: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            // Update stock
            group.MapPatch("/items/{item_id}/stock", async (int item_id, int adjustment) =>
            {
                try
                {
                    using var conn = new MySqlConnection(conn_str);

                    var item = await conn.QueryFirstOrDefaultAsync<Item>(
                        "SELECT item_id, stock FROM items WHERE item_id = @id;",
                        new { id = item_id });

                    if (item == null)
                        return Results.NotFound(new { error = "Item not found." });

                    int new_stock = item.stock + adjustment;
                    if (new_stock < 0)
                        return Results.BadRequest(new { error = "Stock cannot be negative." });

                    await conn.ExecuteAsync(
                        "UPDATE items SET stock = @stock WHERE item_id = @id;",
                        new { id = item_id, stock = new_stock });

                    return Results.Ok(new
                    {
                        item_id = item_id,
                        old_stock = item.stock,
                        new_stock = new_stock,
                        adjustment = adjustment
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in PATCH /items/{item_id}/stock: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });
        }
    }
}
