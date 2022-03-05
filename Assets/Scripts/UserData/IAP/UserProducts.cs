using System.Collections.Generic;
using System.Linq;
using ModelData.IAP;
using UnityEngine.Purchasing;

namespace UserData.IAP
{
    public class UserProducts
    {
        private readonly Dictionary<int, UserProduct> products = new Dictionary<int, UserProduct>();
        
        public UserProducts()
        {
            
        }

        public void AddProduct(Product unityProduct)
        {
            var product = UserProduct.FromUnityProduct(unityProduct);
            products[product.Id] = product;
        }

        public UserProduct GetProduct(int id)
        {
            return products.TryGetValue(id, out var result) ? result : null;
        }

        public UserProduct GetProduct(IAPPayoutType payout)
        {
            return products.Values.FirstOrDefault(p => p.Payout == payout);
        }

        public UserProduct GetProduct(string productId)
        {
            return products.Values.FirstOrDefault(p => p.ProductId.Equals(productId));
        }
    }
}