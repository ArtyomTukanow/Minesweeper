using AllUtils;
using CommandSystem.Commands;
using Controller.Map;
using Core;
using Libraries.RSG;
using ModelData.IAP;
using ModelData.TileMap;
using UnityEngine;
using UserData.IAP;
using UserData.TileMap;
using View.Window.Windows;

namespace Controller
{
    public class LevelEndController
    {
        public static void OnGameOver()
        {
            if(MapController.Instance.UserMap.Data.type == MapDataType.beginner)
                CreateMap(); //без интерстишла, без наворотов
            else if (MapController.Instance.UserMap.Data.type.IsTimer())
                ShowInterstitial().Finally(CreateMap);
            else
                OnLevelMapGameOver();
        }


        public static void OnGameWin()
        {
            if (MapController.Instance.UserMap is UserMapClassic classic)
            {
                if(MapController.Instance.UserMap.Data.type == MapDataType.beginner)
                    ShowRatingWindow(classic).Then(CreateMap); //без интерстишла, без наворотов
                else
                    ShowInterstitial()
                        .Finally(() => ShowRatingWindow(classic))
                        .Then(CreateMap);
            }
            else
                ShowRateUsWindow().Then(CreateMap);
        }

        private static IPromise ShowRateUsWindow()
        {
            if(Game.User.Settings.RateUs)
                return Promise.Resolved();
            
            if (MapController.Instance.UserMap is UserMapLevel level 
                && level.LevelData.Level >= 25 
                && level.LevelData.Level % 5 == 0)
            {
                Game.Mobile.RateUsController.PrepareRateUs();
                RateUsWindow wnd = null;
                wnd = RateUsWindow.Of(userLikeTheGame =>
                {
                    Game.User.Settings.RateUs = true;
                    if(userLikeTheGame)
                        Game.Mobile.RateUsController.ShowRateUs();
                    wnd?.Hide();
                });

                return wnd.ClosePromise;
            }
            
            return Promise.Resolved();
        }

        private static IPromise ShowRatingWindow(UserMapClassic classic)
        {
            var wnd = RatingWindow.Of(classic.Data.type, true, classic.Timer.Place);
            return wnd.ClosePromise;
        }


        private static IPromise ShowInterstitial(bool needShowBuyWindow = true)
        {
            if (Game.User.Settings.Vip || !Game.User.Advert.IsInterstitialAvailable)
                return Promise.Resolved();
            
            var promise = new Promise();

            Game.User.Advert.ShowInterstitial(ShowBuyWindow, promise.ResolveOnce);
            return promise;

            void ShowBuyWindow()
            {
                if (!needShowBuyWindow)
                {
                    promise.ResolveOnce();
                    return;
                }
                
                var product = Game.User.Products.GetProduct(IAPPayoutType.BuyGame);
                if (product != null)
                {
                    InfoWindow.Of()
                        .SetPathOfView("Windows/VipWindow")
                        .SetTitle("game_over".Localize())
                        .SetDescription("disable_ad_desc".Localize())
                        .SetYesNoType("buy_for".Localize(product.LocalizedPrice), "cancel".Localize())
                        .SetOnOk(() => StartBuyProduct(product))
                        .SetOnCancel(promise.ResolveOnce)
                        .Build();
                }
            }
            
            void StartBuyProduct(UserProduct product)
            {
                product.MakePurchase(promise.ResolveOnce, promise.ResolveOnce);
            }
        }

        private static void OnLevelMapGameOver()
        {
            if (MapController.Instance.UserMap is UserMapLevel level && level.LevelData.Level < 10)
            {
                CreateMap();
                return;
            }
            
            if (Game.User.Settings.Vip)
            {
                InfoWindow.Of()
                    .SetPathOfView("Windows/VipWindow")
                    .SetTitle("game_over".Localize())
                    .SetDescription("undo_step_desc".Localize())
                    .SetYesNoType("undo_step".Localize(), "play_again".Localize())
                    .SetOnOk(SetUndoStep)
                    .SetOnCancel(CreateMap)
                    .Build();
                
                return;
            }
            
            if (Game.User.Advert.IsRewardAvailable)
            {
                InfoWindow.Of()
                    .SetDescription("adv_desc".Localize())
                    .SetYesNoType("watch_ad".Localize(), "play_again".Localize())
                    .SetOnOk(StartShowReward)
                    .SetOnCancel(CreateMap)
                    .Build();
                
                return;
            }
            
            var product = Game.User.Products.GetProduct(IAPPayoutType.BuyGame);
            if (product != null)
            {
                InfoWindow.Of()
                    .SetPathOfView("Windows/VipWindow")
                    .SetTitle("game_over".Localize())
                    .SetDescription("endless_undo_desc".Localize())
                    .SetYesNoType("buy_for".Localize(product.LocalizedPrice), "play_again".Localize())
                    .SetOnOk(() => StartBuyProduct(product))
                    .SetOnCancel(CreateMap)
                    .Build();
                
                return;
            }
            
            CreateMap();
            

            void StartShowReward()
            {
                Game.User.Advert.ShowReward(SetUndoStep, OnRewardClosed);
            }
            
            void OnRewardClosed() {}

            void StartBuyProduct(UserProduct product)
            {
                product.MakePurchase(SetUndoStep, CreateMap);
            }
        }
        
        
        
        
        private static void CreateMap()
        {
            MapController.Create(MapController.Instance.UserMap.Data.type);
        }

        private static void SetUndoStep()
        {
            var userMap = MapController.Instance.UserMap;
            
            Vector2Int[] bombTiles = null;
            if(userMap.CommandSystem.LastCommand is CatchBombCommand catchBomb)
                bombTiles = catchBomb.BombPoses;
            
            MapController.Instance.MapView.ZoomByCommand(userMap.CommandSystem.LastCommand);
            
            userMap.CommandSystem.Undo();
            
            if(bombTiles != null)
                userMap.CommandSystem.SetFlags(bombTiles);
            
        }
    }
}