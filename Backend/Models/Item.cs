namespace Backend.Models
{
    public class Item
    {
        public required int item_id { get; set; }
        public required int stand_id { get; set; }
        public required string name { get; set; }
        public required decimal price { get; set; }
        public required int stock { get; set; }
    }

    public class CreateItemRequest
    {
        public required int stand_id { get; set; }
        public required string name { get; set; }
        public required decimal price { get; set; }
        public required int stock { get; set; }
    }

    public class UpdateItemRequest
    {
        public string? name { get; set; }
        public decimal? price { get; set; }
        public int? stock { get; set; }
    }
}
