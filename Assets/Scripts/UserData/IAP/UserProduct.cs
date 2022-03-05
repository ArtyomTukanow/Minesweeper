using System;
using System.Linq;
using Core;
using ModelData.IAP;
using UnityEngine.Purchasing;

namespace UserData.IAP
{
    public class UserProduct
    {
        public int Id { get; private set; }
        public string ProductId { get; private set; }
        public IAPPayoutType Payout { get; private set; }
        
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string LocalizedPrice { get; private set; }
        public decimal Price { get; private set; }
        public string Curency { get; private set; }

        public static UserProduct FromUnityProduct(Product unityProduct)
        {
            var staticProduct = Game.Static.Products.FirstOrDefault(p => p.ProductId == unityProduct.definition.id);
            
            return new UserProduct
            {
                Id = staticProduct.Id,
                ProductId = staticProduct.ProductId,
                Payout = staticProduct.Payout,
                Title = unityProduct.metadata.localizedTitle,
                Description = unityProduct.metadata.localizedDescription,
                Curency = unityProduct.metadata.isoCurrencyCode,
                LocalizedPrice = unityProduct.metadata.localizedPriceString,
                Price = unityProduct.metadata.localizedPrice,
            };
        }

        public void MakePurchase(Action onSuccess = null, Action onFailed = null)
        {
            Game.Iap.MakePurchase(ProductId, onSuccess, onFailed);
        }
    }
}