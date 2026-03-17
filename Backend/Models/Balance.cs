namespace Backend.Models
{
    public class BalanceUpdateRequest {
        public required int user_id { get; set; }
        public required float amount { get; set; }
    }
}