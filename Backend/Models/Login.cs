namespace Backend.Models
{
    public class LoginRequest {
        public required int user_id { get; set; }
        public required string username { get; set; }
    }
}