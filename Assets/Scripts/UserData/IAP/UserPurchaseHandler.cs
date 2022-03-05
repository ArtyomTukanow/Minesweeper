using System;
using Core;
using ModelData.IAP;
using UnityEngine;
using UserData.IAP.Handlers;

namespace UserData.IAP
{
    public class UserPurchaseHandler
    {
        public UserPurchaseHandler()
        {
        }
        
        public bool ConfirmPurchase(string productId)
        {
            var userProduct = Game.User.Products.GetProduct(productId);

            if (userProduct == null)
                return false;

            try
            {
                return GetHandler(userProduct).Confirm();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        public BaseIAPHandler GetHandler(UserProduct userProduct)
        {
            switch (userProduct.Payout)
            {
                case IAPPayoutType.BuyGame:
                    return new BuyGameIAPHandler(userProduct.ProductId);
                // case IAPPayoutType.BuyHelps:
                    // break;
                // case IAPPayoutType.RemoveAd:
                    // break;
                default:
                    Debug.LogError($"Unknown Payout: {userProduct.Payout} productId: {userProduct.ProductId}");
                    throw new ArgumentOutOfRangeException(nameof(userProduct.Payout), userProduct.Payout, "Unknown Payout");
            }
        }
    }
}