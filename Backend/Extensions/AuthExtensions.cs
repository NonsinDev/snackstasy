namespace Backend.Extensions
{
    public static class AuthExtensions
    {
        /// <summary>
        /// Filter der prüft ob eine gültige Session existiert
        /// </summary>
        public static RouteHandlerBuilder RequireSession(this RouteHandlerBuilder builder)
        {
            return builder.AddEndpointFilter(async (context, next) =>
            {
                var http_context = context.HttpContext;
                var user_id = http_context.Session.GetString("user_id");

                if (string.IsNullOrEmpty(user_id))
                {
                    return Results.Unauthorized();
                }

                return await next(context);
            });
        }

        /// <summary>
        /// Filter der prüft ob ein Mitarbeiter eingeloggt ist
        /// </summary>
        public static RouteHandlerBuilder RequireEmployee(this RouteHandlerBuilder builder)
        {
            return builder.AddEndpointFilter(async (context, next) =>
            {
                var http_context = context.HttpContext;
                var employee_id = http_context.Session.GetString("employee_id");

                if (string.IsNullOrEmpty(employee_id))
                {
                    return Results.Json(new { error = "Employee login required" }, statusCode: 401);
                }

                return await next(context);
            });
        }

        /// <summary>
        /// Filter der prüft ob der Mitarbeiter eine bestimmte Rolle hat
        /// Roles: admin, manager, cashier, staff
        /// </summary>
        public static RouteHandlerBuilder RequireRole(this RouteHandlerBuilder builder, params string[] allowed_roles)
        {
            return builder.AddEndpointFilter(async (context, next) =>
            {
                var http_context = context.HttpContext;
                var employee_id = http_context.Session.GetString("employee_id");
                var role = http_context.Session.GetString("employee_role");

                if (string.IsNullOrEmpty(employee_id))
                {
                    return Results.Json(new { error = "Employee login required" }, statusCode: 401);
                }

                if (string.IsNullOrEmpty(role) || !allowed_roles.Contains(role))
                {
                    return Results.Json(new { error = $"Insufficient permissions. Required roles: {string.Join(", ", allowed_roles)}" }, statusCode: 403);
                }

                return await next(context);
            });
        }

        /// <summary>
        /// Admin only access
        /// </summary>
        public static RouteHandlerBuilder RequireAdmin(this RouteHandlerBuilder builder)
        {
            return builder.RequireRole("admin");
        }

        /// <summary>
        /// Manager or Admin access
        /// </summary>
        public static RouteHandlerBuilder RequireManager(this RouteHandlerBuilder builder)
        {
            return builder.RequireRole("admin", "manager");
        }

        /// <summary>
        /// Cashier, Manager or Admin access
        /// </summary>
        public static RouteHandlerBuilder RequireCashier(this RouteHandlerBuilder builder)
        {
            return builder.RequireRole("admin", "manager", "cashier");
        }
    }
}
