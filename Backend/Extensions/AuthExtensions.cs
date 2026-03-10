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
    }
}
