using Newtonsoft.Json;

namespace ModelData.IAP
{
    public class IAPProductData
    {
        [JsonProperty("type")]
        public IAPProductType ProductType { get; private set; }
        
        [JsonProperty("profit")]
        public IAPPayoutType Payout { get; private set; }
        
        [JsonProperty("id")]
        public int Id { get; private set; }
        
        [JsonProperty("product_id")]
        public string ProductId { get; private set; }
    }
}