namespace Backend.Models
{
    public class Stand
    {
        public required int stand_id { get; set; }
        public required string name { get; set; }
        public required int pickup_id { get; set; }
        public required int tablet_id { get; set; }
    }

    public class CreateStandRequest
    {
        public required string name { get; set; }
        public required int pickup_id { get; set; }
        public required int tablet_id { get; set; }
    }

    public class UpdateStandRequest
    {
        public string? name { get; set; }
        public int? pickup_id { get; set; }
        public int? tablet_id { get; set; }
    }
}
