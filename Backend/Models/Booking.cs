namespace Backend.Class
{
    public class BookRequest {
        public required int user_id { get; set; }
        public required string first_name { get; set; }
        public required string last_name { get; set; }
        public required DateTime birth_date { get; set; }
    }
}