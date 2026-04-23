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
            group.MapPost("/orders", async (HttpContext context, CreateOrderRequest req) =>
            {
                if (!SessionHelper.IsUserSessionValid(context, out var ticketId))
                    return Results.Unauthorized();

                try
                {
                    using var conn = new MySqlConnection(conn_str);
                    await conn.OpenAsync();
                    using var transaction = await conn.BeginTransactionAsync();

                    var user = await conn.QueryFirstOrDefaultAsync<User>(
                        "SELECT user_id, balance FROM users WHERE ticket_id = @ticket_id",
                        new { ticket_id = ticketId }, transaction);

                    if (user == null)
                        return Results.NotFound(new { error = "User not found." });

                    var itemIds = req.items.Select(i => i.item_id).ToArray();
                    var dbItems = (await conn.QueryAsync<Item>(
                        "SELECT item_id, price, stock FROM items WHERE item_id IN @ids AND stand_id = @stand_id",
                        new { ids = itemIds, stand_id = req.stand_id }, transaction))
                        .ToDictionary(i => i.item_id);

                    if (dbItems.Count != req.items.Count)
                        return Results.BadRequest(new { error = "One or more items not found for this stand." });

                    decimal totalPrice = 0;
                    foreach (var orderItem in req.items)
                    {
                        var dbItem = dbItems[orderItem.item_id];
                        if (dbItem.stock < orderItem.quantity)
                            return Results.BadRequest(new { error = $"Insufficient stock for item {orderItem.item_id}." });

                        totalPrice += dbItem.price * orderItem.quantity;
                    }

                    if (user.balance < (float)totalPrice)
                        return Results.BadRequest(new { error = "Insufficient balance." });

                    var orderId = await conn.QuerySingleAsync<int>(
                        "INSERT INTO orders (user_id, stand_id, total_price, status) VALUES (@user_id, @stand_id, @total_price, 'pending'); SELECT LAST_INSERT_ID();",
                        new { user_id = user.user_id, stand_id = req.stand_id, total_price = totalPrice }, transaction);

                    foreach (var orderItem in req.items)
                    {
                        var dbItem = dbItems[orderItem.item_id];
                        await conn.ExecuteAsync(
                            "INSERT INTO order_items (order_id, item_id, quantity, price) VALUES (@order_id, @item_id, @quantity, @price)",
                            new { order_id = orderId, item_id = orderItem.item_id, quantity = orderItem.quantity, price = dbItem.price },
                            transaction);

                        await conn.ExecuteAsync(
                            "UPDATE items SET stock = stock - @quantity WHERE item_id = @item_id",
                            new { quantity = orderItem.quantity, item_id = orderItem.item_id },
                            transaction);
                    }

                    await conn.ExecuteAsync(
                        "UPDATE users SET balance = balance - @amount WHERE user_id = @user_id",
                        new { amount = totalPrice, user_id = user.user_id }, transaction);

                    await transaction.CommitAsync();

                    return Results.Created($"/v1/orders/{orderId}", new { order_id = orderId, total_price = totalPrice });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in POST /orders: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            // GET /orders/{order_id} - Get order with items (requires user session)
            group.MapGet("/orders/{order_id}", async (HttpContext context, int order_id) =>
            {
                if (!SessionHelper.IsUserSessionValid(context, out var ticketId))
                    return Results.Unauthorized();

                try
                {
                    using var conn = new MySqlConnection(conn_str);

                    var order = await conn.QueryFirstOrDefaultAsync<Order>(
                        @"SELECT o.order_id, o.user_id, o.stand_id, o.total_price, o.status, o.created_at
                          FROM orders o
                          INNER JOIN users u ON u.user_id = o.user_id
                          WHERE o.order_id = @order_id AND u.ticket_id = @ticket_id",
                        new { order_id, ticket_id = ticketId });

                    if (order == null)
                        return Results.NotFound(new { error = "Order not found." });

                    var items = await conn.QueryAsync<OrderItem>(
                        "SELECT order_item_id, order_id, item_id, is_collected, quantity, price FROM order_items WHERE order_id = @order_id",
                        new { order_id });

                    return Results.Ok(new { order, items });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in GET /orders/{{order_id}}: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            // PATCH /orders/items/{order_item_id} - Update is_collected
            group.MapPatch("/orders/items/{order_item_id}", async (HttpContext context, int order_item_id, UpdateOrderItemCollectedRequest req) =>
            {
                if (!SessionHelper.IsEmployeeSessionValid(context, out _))
                    return Results.Unauthorized();

                try
                {
                    using var conn = new MySqlConnection(conn_str);

                    var rows = await conn.ExecuteAsync(
                        "UPDATE order_items SET is_collected = @is_collected WHERE order_item_id = @order_item_id",
                        new { req.is_collected, order_item_id });

                    if (rows == 0)
                        return Results.NotFound(new { error = "Order item not found." });

                    return Results.Ok(new { updated = true });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in PATCH /orders/items/{{order_item_id}}: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });
        }
    }
}
