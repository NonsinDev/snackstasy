using Backend.Models;
using Dapper;
using MySqlConnector;

namespace Backend.Router
{
    public static class EmployeeRoutes
    {
        public static void MapEmployeeRoutes(this RouteGroupBuilder group, string conn_str)
        {
            // POST /employee/login
            group.MapPost("/employee/login", async (EmployeeLoginRequest req) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(req.username) || string.IsNullOrWhiteSpace(req.password))
                        return Results.BadRequest(new { error = "Username and password are required." });

                    using var conn = new MySqlConnection(conn_str);

                    var employee = await conn.QueryFirstOrDefaultAsync<Employee>(
                        "SELECT employee_id, username, first_name, last_name, password_hash, role, stand_id, is_active FROM employees WHERE username = @username",
                        new { req.username });

                    if (employee == null || !employee.is_active || !BCrypt.Net.BCrypt.Verify(req.password, employee.password_hash))
                        return Results.Problem(detail: "Invalid credentials.", statusCode: 401);

                    return Results.Ok(new { logged_in = true });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in POST /employee/login: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });

            // POST /employee/create - Create employee
            group.MapPost("/employee/create", async (CreateEmployeeRequest req) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(req.username) || string.IsNullOrWhiteSpace(req.password))
                        return Results.BadRequest(new { error = "Username and password are required." });

                    string passwordHash = BCrypt.Net.BCrypt.HashPassword(req.password);

                    using var conn = new MySqlConnection(conn_str);

                    var employeeId = await conn.QuerySingleAsync<int>(
                        @"INSERT INTO employees (username, first_name, last_name, password_hash, role, stand_id)
                          VALUES (@username, @first_name, @last_name, @password_hash, @role, @stand_id);
                          SELECT LAST_INSERT_ID();",
                        new
                        {
                            req.username,
                            req.first_name,
                            req.last_name,
                            password_hash = passwordHash,
                            role = req.role ?? "staff",
                            stand_id = req.stand_id
                        });

                    return Results.Created($"/v1/employee/{employeeId}", new { employee_id = employeeId, req.username });
                }
                catch (MySqlException ex) when (ex.Number == 1062)
                {
                    return Results.Conflict(new { error = "Username already exists." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in POST /employee: {ex}");
                    return Results.Problem("Internal server error: " + ex.Message);
                }
            });
        }
    }
}
