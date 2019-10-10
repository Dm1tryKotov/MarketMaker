namespace MMS.Models
{
    public class Order
    {
        public string Id { get; private set; }
        
        public string ClientOrderId { get; private set; }
        
        public string Symbol { get; private set; }
        
        public string Side { get; private set; }
        
        public string Status { get; private set; }
        
        public string Type { get; private set; }
        
        public string TimeInForce { get; private set; }
        
        public float Quantity { get; private set; }
        
        public float Price { get; private set; }
        
        public string CreatedAt { get; private set; }
        
        public string UpdatedAt { get; private set; }
        
        public string Timestamp { get; private set; }

        public Order(
            string id = "",
            string symbol = "",
            string clientOrderId = "",
            string status = "",
            string type = "",
            string timeInForce = "",
            string createdAt = "",
            string updatedAt = "",
            string timestamp = "",
            float quantity = 0f,
            float price = 0f,
            string side = ""
            )
        {
            Id = id;
            Symbol = symbol;
            ClientOrderId = clientOrderId;
            Status = status;
            Type = type;
            TimeInForce = timeInForce;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            Timestamp = timestamp;
            Quantity = quantity;
            Price = price;
            Side = side;
        }
    }
}
