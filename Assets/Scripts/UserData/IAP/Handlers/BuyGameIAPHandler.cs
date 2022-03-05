using AllUtils;
using Core;
using View.Window.Windows;

namespace UserData.IAP.Handlers
{
    public class BuyGameIAPHandler : BaseIAPHandler
    {
        public BuyGameIAPHandler(string productId) : base(productId)
        {
        }

        protected override bool ConfirmPurchase()
        {
            var needShowWindow = !Game.User.Settings.Vip;
            if (needShowWindow)
            {
                InfoWindow.Of()
                    .SetPathOfView("Windows/VipWindow")
                    .SetTitle("pay_success".Localize())
                    .SetDescription("vip_pay_success_desc".Localize())
                    .SetOkType("ok".Localize())
                    .Build();
            }
            
            Game.User.Settings.Vip = true;

            return true;
        }
    }
}