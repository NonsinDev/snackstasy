namespace Backend.Services
{
    public static class TicketService
    {
        public static string GenerateTicketId(string firstName, string lastName)
        {
            string initials = $"{firstName[0]}{lastName[0]}".ToUpper();
            int maxMagic = Random.Shared.Next(100, 999);
            string timePart = DateTime.UtcNow.Ticks.ToString("x"); // Hexadezimal
            string ticketId = $"{initials}{timePart[^6..]}{maxMagic}"; // nur die letzten 6 Zeichen
            return ticketId;
        }
    }
}DA    207274   757