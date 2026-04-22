using Backend.Models;
using Dapper;
using MySqlConnector;

namespace Backend.Router
{
    public static class AdminRoutes
    {
        private static bool IsAdminAuthenticated(HttpContext context)
        {
            var role = context.Session.GetString("admin_role");
            return role == "admin" || role == "manager";
        }

        public static void MapAdminRoutes(this RouteGroupBuilder group, string conn_str)
        {
            // POST /admin/login
            group.MapPost("/admin/login", async (HttpContext context, AdminLoginRequest req) =>
            {
                if (string.IsNullOrWhiteSpace(req.username) || string.IsNullOrWhiteSpace(req.password))
                    return Results.BadRequest(new { error = "Username and password are required." });

                try
                {
                    using var conn = new MySqlConnection(conn_str);
                    var employee = await conn.QueryFirstOrDefaultAsync<Employee>(
                        "SELECT employee_id, username, first_name, last_name, password_hash, role, stand_id, is_active FROM employees WHERE username = @username;",
                        new { username = req.username });

                    if (employee == null || !employee.is_active)
                        return Results.Unauthorized();

                    if (!BCrypt.Net.BCrypt.Verify(req.password, employee.password_hash))
                        return Results.Unauthorized();

                    context.Session.SetString("admin_employee_id", employee.employee_id.ToString());
                    context.Session.SetString("admin_username", employee.username);
                    context.Session.SetString("admin_role", employee.role);
                    context.Session.SetString("admin_first_name", employee.first_name);
                    context.Session.SetString("admin_last_name", employee.last_name);

                    return Results.Ok(new
                    {
                        employee_id = employee.employee_id,
                        username = employee.username,
                        first_name = employee.first_name,
                        last_name = employee.last_name,
                        role = employee.role
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in POST /admin/login: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            // GET /admin/session
            group.MapGet("/admin/session", (HttpContext context) =>
            {
                var role = context.Session.GetString("admin_role");
                if (string.IsNullOrEmpty(role))
                    return Results.Ok(new { logged_in = false });

                return Results.Ok(new
                {
                    logged_in = true,
                    employee_id = context.Session.GetString("admin_employee_id"),
                    username = context.Session.GetString("admin_username"),
                    first_name = context.Session.GetString("admin_first_name"),
                    last_name = context.Session.GetString("admin_last_name"),
                    role
                });
            });

            // POST /admin/logout
            group.MapPost("/admin/logout", (HttpContext context) =>
            {
                context.Session.Clear();
                return Results.Ok(new { message = "Logged out." });
            });

            // POST /admin/change-password
            group.MapPost("/admin/change-password", async (HttpContext context, AdminChangePasswordRequest req) =>
            {
                if (!IsAdminAuthenticated(context))
                    return Results.Unauthorized();

                var employeeIdStr = context.Session.GetString("admin_employee_id");
                if (!int.TryParse(employeeIdStr, out int employeeId))
                    return Results.Unauthorized();

                try
                {
                    using var conn = new MySqlConnection(conn_str);
                    var currentHash = await conn.QueryFirstOrDefaultAsync<string>(
                        "SELECT password_hash FROM employees WHERE employee_id = @id;",
                        new { id = employeeId });

                    if (currentHash == null || !BCrypt.Net.BCrypt.Verify(req.current_password, currentHash))
                        return Results.BadRequest(new { error = "Current password is incorrect." });

                    string newHash = BCrypt.Net.BCrypt.HashPassword(req.new_password, 11);
                    await conn.ExecuteAsync(
                        "UPDATE employees SET password_hash = @hash WHERE employee_id = @id;",
                        new { hash = newHash, id = employeeId });

                    return Results.Ok(new { message = "Password changed successfully." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in POST /admin/change-password: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            // GET /admin/stands
            group.MapGet("/admin/stands", async (HttpContext context) =>
            {
                if (!IsAdminAuthenticated(context))
                    return Results.Unauthorized();

                try
                {
                    using var conn = new MySqlConnection(conn_str);
                    var stands = await conn.QueryAsync<Stand>(
                        "SELECT stand_id, name, pickup_id, tablet_id FROM stands;");
                    return Results.Ok(stands);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in GET /admin/stands: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            // POST /admin/stands/create
            group.MapPost("/admin/stands/create", async (HttpContext context, CreateStandRequest req) =>
            {
                if (!IsAdminAuthenticated(context))
                    return Results.Unauthorized();

                if (string.IsNullOrWhiteSpace(req.name))
                    return Results.BadRequest(new { error = "Stand name is required." });

                try
                {
                    using var conn = new MySqlConnection(conn_str);
                    const string sql = @"
                        INSERT INTO stands (name, pickup_id, tablet_id)
                        VALUES (@Name, @PickupId, @TabletId);
                        SELECT LAST_INSERT_ID();";

                    int id = await conn.QueryFirstAsync<int>(sql, new
                    {
                        Name = req.name,
                        PickupId = req.pickup_id,
                        TabletId = req.tablet_id
                    });

                    return Results.Ok(new { stand_id = id, req.name, req.pickup_id, req.tablet_id });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in POST /admin/stands/create: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            // PUT /admin/stands/{stand_id}
            group.MapPut("/admin/stands/{stand_id}", async (HttpContext context, int stand_id, UpdateStandRequest req) =>
            {
                if (!IsAdminAuthenticated(context))
                    return Results.Unauthorized();

                var updates = new List<string>();
                var parameters = new DynamicParameters();
                parameters.Add("id", stand_id);

                if (!string.IsNullOrWhiteSpace(req.name)) { updates.Add("name = @name"); parameters.Add("name", req.name); }
                if (req.pickup_id.HasValue) { updates.Add("pickup_id = @pickup_id"); parameters.Add("pickup_id", req.pickup_id.Value); }
                if (req.tablet_id.HasValue) { updates.Add("tablet_id = @tablet_id"); parameters.Add("tablet_id", req.tablet_id.Value); }

                if (updates.Count == 0)
                    return Results.BadRequest(new { error = "No fields to update." });

                try
                {
                    using var conn = new MySqlConnection(conn_str);
                    string query = $"UPDATE stands SET {string.Join(", ", updates)} WHERE stand_id = @id;";
                    int rows = await conn.ExecuteAsync(query, parameters);

                    if (rows == 0)
                        return Results.NotFound(new { error = "Stand not found." });

                    return Results.Ok(new { message = $"Stand {stand_id} updated." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in PUT /admin/stands/{stand_id}: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            // DELETE /admin/stands/{stand_id}
            group.MapDelete("/admin/stands/{stand_id}", async (HttpContext context, int stand_id) =>
            {
                if (!IsAdminAuthenticated(context))
                    return Results.Unauthorized();

                try
                {
                    using var conn = new MySqlConnection(conn_str);
                    var item_count = await conn.QueryFirstAsync<int>(
                        "SELECT COUNT(*) FROM items WHERE stand_id = @id;",
                        new { id = stand_id });

                    if (item_count > 0)
                        return Results.BadRequest(new { error = $"Cannot delete stand with {item_count} items. Remove items first." });

                    int rows = await conn.ExecuteAsync(
                        "DELETE FROM stands WHERE stand_id = @id;",
                        new { id = stand_id });

                    if (rows == 0)
                        return Results.NotFound(new { error = "Stand not found." });

                    return Results.Ok(new { message = $"Stand {stand_id} deleted." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in DELETE /admin/stands/{stand_id}: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            // GET /admin/orders
            group.MapGet("/admin/orders", async (HttpContext context) =>
            {
                if (!IsAdminAuthenticated(context))
                    return Results.Unauthorized();

                try
                {
                    using var conn = new MySqlConnection(conn_str);
                    var orders = await conn.QueryAsync(@"
                        SELECT
                            o.order_id,
                            o.user_id,
                            u.first_name,
                            u.last_name,
                            u.ticket_id,
                            o.stand_id,
                            s.name AS stand_name,
                            o.total_amount,
                            o.status,
                            o.created_at
                        FROM orders o
                        JOIN users u ON o.user_id = u.user_id
                        JOIN stands s ON o.stand_id = s.stand_id
                        ORDER BY o.created_at DESC;");

                    return Results.Ok(orders);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in GET /admin/orders: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            // GET /admin/orders/{order_id} - Bestelldetails mit Positionen
            group.MapGet("/admin/orders/{order_id}", async (HttpContext context, int order_id) =>
            {
                if (!IsAdminAuthenticated(context))
                    return Results.Unauthorized();

                try
                {
                    using var conn = new MySqlConnection(conn_str);

                    var order = await conn.QueryFirstOrDefaultAsync(@"
                        SELECT
                            o.order_id, o.user_id, u.first_name, u.last_name, u.ticket_id,
                            o.stand_id, s.name AS stand_name,
                            o.total_amount, o.status, o.created_at
                        FROM orders o
                        JOIN users u ON o.user_id = u.user_id
                        JOIN stands s ON o.stand_id = s.stand_id
                        WHERE o.order_id = @order_id;",
                        new { order_id });

                    if (order == null)
                        return Results.NotFound(new { error = "Order not found." });

                    var items = await conn.QueryAsync(@"
                        SELECT oi.order_item_id, oi.item_id, i.name, oi.quantity, oi.price
                        FROM order_items oi
                        JOIN items i ON oi.item_id = i.item_id
                        WHERE oi.order_id = @order_id;",
                        new { order_id });

                    return Results.Ok(new { order, items });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in GET /admin/orders/{order_id}: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            // PATCH /admin/orders/{order_id}/status
            group.MapPatch("/admin/orders/{order_id}/status", async (HttpContext context, int order_id, UpdateOrderStatusRequest req) =>
            {
                if (!IsAdminAuthenticated(context))
                    return Results.Unauthorized();

                var allowed = new[] { "pending", "preparing", "ready", "completed", "cancelled" };
                if (!allowed.Contains(req.status))
                    return Results.BadRequest(new { error = "Invalid status." });

                try
                {
                    using var conn = new MySqlConnection(conn_str);
                    int rows = await conn.ExecuteAsync(
                        "UPDATE orders SET status = @status WHERE order_id = @id;",
                        new { status = req.status, id = order_id });

                    if (rows == 0)
                        return Results.NotFound(new { error = "Order not found." });

                    return Results.Ok(new { message = $"Order {order_id} status updated to {req.status}." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in PATCH /admin/orders/{order_id}/status: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });
        }
    }
}
