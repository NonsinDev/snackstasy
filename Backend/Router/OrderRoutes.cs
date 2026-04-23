using Backend.Extensions;
using Backend.Models;
using Dapper;
using MySqlConnector;

namespace Backend.Router
{
    public static class OrderRoutes
    {
        public static void MapOrderRoutes(this RouteGroupBuilder group, string conn_str)
        {
            // POST /orders - Create order (requires user session)
            group.MapPost("/orders", async (CreateOrderRequest req) =>
            {
                try
                {
                    using var conn = new MySqlConnection(conn_str);
                    await conn.OpenAsync();
                    using var transaction = await conn.BeginTransactionAsync();

                    // User direkt laden
                    var user = await conn.QueryFirstOrDefaultAsync<User>(
                        "SELECT * FROM users WHERE user_id = @user_id",
                        new { req.user_id }, transaction);

                    if (user == null)
                        return Results.NotFound(new { error = "User not found." });

                    var itemIds = req.items.Select(i => i.item_id).ToArray();

                    var dbItems = (await conn.QueryAsync<Item>(
                        @"SELECT item_id, price, stock
                          FROM items
                          WHERE item_id IN @ids AND stand_id = @stand_id",
                        new { ids = itemIds, stand_id = req.stand_id }, transaction))
                        .ToDictionary(i => i.item_id);

                    if (dbItems.Count != req.items.Count)
                        return Results.BadRequest(new { error = "One or more items not found for this stand." });

                    decimal totalPrice = 0;

                    foreach (var orderItem in req.items)
                    {
                        var dbItem = dbItems[orderItem.item_id];

                        if (dbItem.stock < orderItem.quantity)
                            return Results.BadRequest(new { error = $"Insufficient stock for item {orderItem.item_id}" });

                        totalPrice += dbItem.price * orderItem.quantity;
                    }

                    if (user.balance < totalPrice)
                        return Results.BadRequest(new { error = "Insufficient balance." });

                    // Order erstellen
                    var orderId = await conn.QuerySingleAsync<int>(
                        @"INSERT INTO orders (user_id, stand_id, total_price, status)
                          VALUES (@user_id, @stand_id, @total_price, 'pending');
                          SELECT LAST_INSERT_ID();",
                        new { req.user_id, req.stand_id, total_price = totalPrice }, transaction);

                    // Items speichern + Stock reduzieren
                    foreach (var orderItem in req.items)
                    {
                        var dbItem = dbItems[orderItem.item_id];

                        await conn.ExecuteAsync(
                            @"INSERT INTO order_items (order_id, item_id, quantity, price)
                              VALUES (@order_id, @item_id, @quantity, @price)",
                            new
                            {
                                order_id = orderId,
                                item_id = orderItem.item_id,
                                quantity = orderItem.quantity,
                                price = dbItem.price
                            },
                            transaction);

                        await conn.ExecuteAsync(
                            "UPDATE items SET stock = stock - @quantity WHERE item_id = @item_id",
                            new { orderItem.quantity, orderItem.item_id },
                            transaction);
                    }

                    // Balance abziehen
                    await conn.ExecuteAsync(
                        "UPDATE users SET balance = balance - @amount WHERE user_id = @user_id",
                        new { amount = totalPrice, req.user_id },
                        transaction);

                    await transaction.CommitAsync();

                    return Results.Ok(new
                    {
                        order_id = orderId,
                        total_price = totalPrice
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error POST /orders: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            // GET /orders/{order_id} - Get order with items (requires user session)
            group.MapGet("/orders/{order_id}", async (int order_id) =>
            {
                try
                {
                    using var conn = new MySqlConnection(conn_str);

                    var order = await conn.QueryFirstOrDefaultAsync<Order>(
                        @"SELECT order_id, user_id, stand_id, total_price, status, created_at
                          FROM orders
                          WHERE order_id = @order_id",
                        new { order_id });

                    if (order == null)
                        return Results.NotFound(new { error = "Order not found." });

                    var items = await conn.QueryAsync<OrderItem>(
                        @"SELECT order_item_id, order_id, item_id, is_collected, quantity, price
                          FROM order_items
                          WHERE order_id = @order_id",
                        new { order_id });

                    return Results.Ok(new { order, items });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error GET /orders/{order_id}: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            // PATCH /orders/items/{order_item_id} - Update is_collected
            group.MapPatch("/orders/items/{order_item_id}", async (int order_item_id, UpdateOrderItemCollectedRequest req) =>
            {
                try
                {
                    using var conn = new MySqlConnection(conn_str);

                    var rows = await conn.ExecuteAsync(
                        @"UPDATE order_items
                          SET is_collected = @is_collected
                          WHERE order_item_id = @order_item_id",
                        new { req.is_collected, order_item_id });

                    if (rows == 0)
                        return Results.NotFound(new { error = "Order item not found." });

                    return Results.Ok(new { updated = true });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error PATCH /orders/items/{order_item_id}: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });
        }
    }
}
