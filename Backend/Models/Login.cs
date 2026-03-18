namespace Backend.Models
{
    public class LoginRequest {
        public required int ticket_id { get; set; }
        public required string username { get; set; }
    }
}