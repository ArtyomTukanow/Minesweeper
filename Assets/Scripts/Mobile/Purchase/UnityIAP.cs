using System;
using Core;
using ModelData.IAP;
using UnityEngine;
using UnityEngine.Purchasing;
using UserData.IAP;

namespace Mobile.Purchase
{
    public class UnityIAP : MonoBehaviour, IStoreListener
    {
        public event Action<string, bool> OnPurchase;
        
        private IStoreController controller;

        private ITransactionHistoryExtensions transactionHistory;
        
        private bool isInProgress;

        private bool initOnce = false;
        
        public void Initialize()
        {
            if(initOnce)
                return;

            initOnce = true;
            
            var module = StandardPurchasingModule.Instance();
            module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;

            var builder = ConfigurationBuilder.Instance(module);

            foreach (var productData in Game.Static.Products)
                builder.AddProduct(productData.ProductId, productData.ProductType.ToUnityType());

            UnityPurchasing.Initialize(this, builder);
        }

        public void MakePurchase(string productID, Action onSuccess = null, Action onFailed = null)
        {
            if (isInProgress)
            {
                Debug.Log("Please wait, purchase in progress");
                onFailed?.Invoke();
                return;
            }

            if (controller == null)
            {
                Debug.LogError("Purchasing is not initialized");
                onFailed?.Invoke();
                return;
            }

            if (controller.products.WithID(productID) == null)
            {
                Debug.LogError("No product has id " + productID);
                onFailed?.Invoke();
                return;
            }

            isInProgress = true;
            controller.InitiatePurchase(controller.products.WithID(productID));
            
            OnPurchase += OnPurchased;
            
            void OnPurchased(string purchasedProductId, bool success)
            {
                if (purchasedProductId != productID)
                    return;
                
                OnPurchase -= OnPurchased;
                    
                if (success)
                    onSuccess?.Invoke();
                else
                    onFailed?.Invoke();
            }
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            this.controller = controller;
            transactionHistory = extensions.GetExtension<ITransactionHistoryExtensions>();

            foreach (var item in controller.products.all)
            {
                if (item.availableToPurchase)
                    Game.User.Products.AddProduct(item);
            }
        }
        
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log("Billing failed to initialize!");
            switch (error)
            {
                case InitializationFailureReason.AppNotKnown:
                    Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
                    break;
                case InitializationFailureReason.PurchasingUnavailable:
                    Debug.Log("Billing disabled!");
                    break;
                case InitializationFailureReason.NoProductsAvailable:
                    Debug.Log("No products available for purchase!");
                    break;
            }
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            var productId = e.purchasedProduct.definition.id;
            var receipt = e.purchasedProduct.receipt;
            
            Debug.Log("Purchase OK: " + productId);
            Debug.Log("Receipt: " + receipt);

            isInProgress = false;

            OnPurchase?.Invoke(productId, true);

            var confirm = Game.User.PurchaseHandler.ConfirmPurchase(productId);

            return confirm ? PurchaseProcessingResult.Complete : PurchaseProcessingResult.Pending;
        }

        public void OnPurchaseFailed(Product item, PurchaseFailureReason r)
        {
            var productId = item.definition.id;
            
            Debug.Log("Purchase failed: " + productId);
            Debug.Log(r);

            Debug.Log("Store specific error code: " + transactionHistory.GetLastStoreSpecificPurchaseErrorCode());
            if (transactionHistory.GetLastPurchaseFailureDescription() != null)
            {
                Debug.Log("Purchase failure description message: " +
                          transactionHistory.GetLastPurchaseFailureDescription().message);
            }

            isInProgress = false;

            OnPurchase?.Invoke(productId, false);
        }

    }
}