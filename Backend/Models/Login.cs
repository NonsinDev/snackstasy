namespace Backend.Models
{
    public class LoginRequest {
        public required string user_id { get; set; }
        public required string username { get; set; }
    }
}