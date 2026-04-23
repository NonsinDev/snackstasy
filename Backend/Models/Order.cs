namespace Backend.Models
{
    public class Order
    {
        public required int order_id { get; set; }
        public required int user_id { get; set; }
        public required int stand_id { get; set; }
        public required decimal total_price { get; set; }
        public required string status { get; set; }
        public required DateTime created_at { get; set; }
    }

    public class OrderItem
    {
        public required int order_item_id { get; set; }
        public required int order_id { get; set; }
        public required int item_id { get; set; }
        public required bool is_collected { get; set; }
        public required int quantity { get; set; }
        public required decimal price { get; set; }
    }

    public class CreateOrderRequest
    {
        public required int stand_id { get; set; }
        public required List<OrderItemRequest> items { get; set; }
    }

    public class OrderItemRequest
    {
        public required int item_id { get; set; }
        public required int quantity { get; set; }
    }

    public class UpdateOrderItemCollectedRequest
    {
        public required bool is_collected { get; set; }
    }
}
