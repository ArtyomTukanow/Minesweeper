using System;
using UnityEngine.Purchasing;

namespace ModelData.IAP
{
    public enum IAPProductType
    {
        Consumable = 0,
        NonConsumable = 1,
        Subscription = 2
    }

    public static class IAPProductTypeUtil
    {
        public static ProductType ToUnityType(this IAPProductType type)
        {
            return type switch
            {
                IAPProductType.Consumable => ProductType.Consumable,
                IAPProductType.NonConsumable => ProductType.NonConsumable,
                IAPProductType.Subscription => ProductType.Subscription,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}