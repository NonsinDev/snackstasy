namespace Backend.Services
{
    public static class TicketService
    {
        public static string GenerateTicketId(string firstName, string lastName)
        {
            string initials = $"{firstName[0]}{lastName[0]}".ToUpper();
            int random = Random.Shared.Next(100000, 999999);

            return $"{initials}-{random}";
        }
    }
}