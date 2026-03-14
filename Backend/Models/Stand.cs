namespace Backend.Models
{
    public class Stand
    {
        public required int stand_id { get; set; }
        public required string name { get; set; }
        public required string pickup_id { get; set; }
        public required string tablet_id { get; set; }
    }

    public class CreateStandRequest
    {
        public required string name { get; set; }
        public required string pickup_id { get; set; }
        public required string tablet_id { get; set; }
    }

    public class UpdateStandRequest
    {
        public string? name { get; set; }
        public string? pickup_id { get; set; }
        public string? tablet_id { get; set; }
    }
}
