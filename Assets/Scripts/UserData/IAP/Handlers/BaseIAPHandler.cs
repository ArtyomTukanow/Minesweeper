using UnityEngine;

namespace UserData.IAP.Handlers
{
    public abstract class BaseIAPHandler
    {
        public readonly string productId;

        public BaseIAPHandler(string productId)
        {
            this.productId = productId;
        }

        public bool Confirm()
        {
            Debug.Log("Confirm purchase " + GetType().Name);
            return ConfirmPurchase();
        }

        protected abstract bool ConfirmPurchase();
    }
}