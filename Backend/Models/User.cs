namespace Backend.Models
{
    public class User {
        public required int user_id { get; set; }
        public required string first_name { get; set; }
        public required string last_name { get; set; }
        public required float balance { get; set; }
    }
}