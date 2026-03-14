namespace Backend.Models
{
    public class BalanceUpdateRequest {
        public required string user_id { get; set; }
        public required float amount { get; set; }
    }
}