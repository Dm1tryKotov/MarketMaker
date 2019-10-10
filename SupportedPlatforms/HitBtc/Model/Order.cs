using Newtonsoft.Json;

namespace MMS.SupportedPlatforms.HitBtc.Model
{
    public class Order
    {
        [JsonProperty("error")]
        public Error Error { get; set; }

        [JsonProperty("exception")]
        public ExceptionInfo Exception { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("clientOrderId")]
        public string ClientOrderId { get; set; }
        
        [JsonProperty("symbol")]
        public string Symbol { get; set; }
        
        [JsonProperty("side")]
        public string Side { get; set; }
        
        [JsonProperty("status")]
        public string Status { get; set; }
        
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("timeInForce")]
        public string TimeInForce { get; set; }
        
        [JsonProperty("quantity")]
        public float Quantity { get; set; }
        
        [JsonProperty("price")]
        public float Price { get; set; }
        
        [JsonProperty("cumQuantity")]
        public float CumQuantity { get; set; }
        
        [JsonProperty("createdAt")]
        public string CreatedAt { get; set; }
        
        [JsonProperty("updatedAt")]
        public string UpdatedAt { get; set; }
        
        [JsonProperty("stopPrice")]
        public string StopPrice { get; set; }
        
        [JsonProperty("expireTime")]
        public string ExpireTime { get; set; }
        
        [JsonProperty("timestamp")]
        public string timestamp { get; set; }

        public override string ToString()
        {
            return $"id: {Id}" +
                   $"\nclient: {ClientOrderId}" +
                   $"\nside: {Side}" +
                   $"\namount: {Quantity}" +
                   $"\nprice: {Price}";
        }
    }
}
