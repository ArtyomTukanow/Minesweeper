using System;
using AllUtils;
using Controller.Map;
using Core;
using Core.AssetsManager;
using ModelData.IAP;
using ModelData.TileMap;
using UnityEngine;
using UserData.IAP;
using UserData.TileMap;
using View.Map;
using View.Window.Settings;
using View.Window.Views;

namespace View.Window.Windows
{
    public class MainMenuWindow : Window<MainMenuView>
    {
        private MapDataType mapType;
        
        public MainMenuWindow(WindowSettings settings = null) : base(settings)
        {
        }

        public override string PathOfView => "Windows/MainMenu";


        public static void Of()
        {
            if(MapController.Instance.MapView is MapTutorView)
                return;
            
            var settings = new WindowSettings.Builder
            {
                Priory = 150,
                ShowInQueue = false
            }.Build();
            
            var wnd = new MainMenuWindow(settings);
            WindowsController.Instance.AddWindow(wnd);
        }

        protected override void OnShow()
        {
            base.OnShow();

            mapType = MapController.Instance?.UserMap.Data.type ?? MapDataType.level;
            
            DrawText();
            AddButtonListeners();

            View.swiper.OnSwipeToLeft = NextMapType;
            View.swiper.OnSwipeToRight = PrevMapType;
            
            DrawMapType();
        }

        private void DrawText()
        {
            View.continueText.text = "Continue".Localize();
            View.newGameText.text = "new_game".Localize();
            View.unlockByVipText.text = "unlock_by".Localize("");
        }

        private void NextMapType()
        {
            mapType.Next();
            DrawMapType();
        }

        private void PrevMapType()
        {
            mapType.Prev();
            DrawMapType();
        }

        private void DrawMapType()
        {
            if (mapType == MapDataType.level)
                View.targetTypeText.text = mapType.ToString().Localize(Game.User.Level.ToString());
            else
                View.targetTypeText.text = mapType.ToString().Localize();

            var needContinue = MapController.Instance?.UserMap.Data.type == mapType;

            var sprite = AssetsLoader.CreateSync<Sprite>("covers/" + mapType);
            View.cover.sprite = sprite;

            var hasProduct = Game.User.Products.GetProduct(IAPPayoutType.BuyGame) != null;
            var isLock = Game.User.Levels.IsLock(mapType);
            
            View.lockView.gameObject.SetActive(isLock);
            View.lockerText.gameObject.SetActive(isLock);
            
            View.newGameButton.gameObject.SetActive(!isLock);
            View.continueButton.gameObject.SetActive(!isLock && needContinue);
            View.unlockByVipButton.gameObject.SetActive(hasProduct && isLock);

            if (isLock)
                View.lockerText.text = "unlock_by_level".Localize(Game.User.Levels.GetLockLevel(mapType).ToString());
        }

        private void AddButtonListeners()
        {
            View.continueButton.onClick.RemoveAllListeners();
            View.continueButton.onClick.AddListener(OnContinueClick);
            
            View.unlockByVipButton.onClick.RemoveAllListeners();
            View.unlockByVipButton.onClick.AddListener(OnUnlockByVipClick);
            
            View.newGameButton.onClick.RemoveAllListeners();
            View.newGameButton.onClick.AddListener(OnNewGameClick);
            
            View.paletteButton.onClick.RemoveAllListeners();
            View.paletteButton.onClick.AddListener(OnPaletteClick);
            
            View.exitButton.onClick.RemoveAllListeners();
            View.exitButton.onClick.AddListener(OnExitClick);
            
            View.storeButton.onClick.RemoveAllListeners();
            View.storeButton.onClick.AddListener(OnStoreClick);
            
            View.localizeButton.onClick.RemoveAllListeners();
            View.localizeButton.onClick.AddListener(OnLocalizeClick);
            
            View.ratingButton.onClick.RemoveAllListeners();
            View.ratingButton.onClick.AddListener(OnRatingClick);
        }

        private void OnStoreClick()
        {
            Game.Mobile.GoToStore();
        }

        private void OnLocalizeClick()
        {
            Hide();
            ChooseLocaleWindow.Of();
        }

        private void OnRatingClick()
        {
            Hide();
            if(mapType.IsTimer())
                RatingWindow.Of(mapType);
            else
                RatingWindow.Of();
        }

        private void OnExitClick()
        {
            Game.Quit();
        }

        private void OnPaletteClick()
        {
            Game.User.Settings.BlackPalette = !Game.User.Settings.BlackPalette;
        }

        private void OnContinueClick()
        {
            Hide();
        }

        private void OnUnlockByVipClick()
        {
            var product = Game.User.Products.GetProduct(IAPPayoutType.BuyGame);
            if (product != null)
            {
                Hide();
                InfoWindow.Of()
                    .SetPathOfView("Windows/VipWindow")
                    .SetTitle("vip_buy_title".Localize())
                    .SetDescription("unlock_all_modes".Localize())
                    .SetYesNoType("buy_for".Localize(product.LocalizedPrice), "cancel".Localize())
                    .SetOnOk(() => product.MakePurchase())
                    .Build();
            }
        }

        private void OnNewGameClick()
        {
            Hide();
            TryShowAttention(() => MapController.Create(mapType));
        }

        private void TryShowAttention(Action onOk)
        {
            if (MapController.Instance?.UserMap.IsGenerated != true)
            {
                onOk?.Invoke();
                return;
            }
            
            if (MapController.Instance.UserMap.State == UserMapState.GameOver)
            {
                onOk?.Invoke();
                return;
            }
            
            InfoWindow.Of()
                .SetDescription("lose_prog".Localize())
                .SetYesNoType("new_game".Localize(), "continue".Localize())
                .SetOnOk(onOk)
                .Build();
        }
    }
}