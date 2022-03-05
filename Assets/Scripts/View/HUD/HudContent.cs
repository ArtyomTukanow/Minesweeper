using System;
using AllUtils;
using CommandSystem.Commands;
using Controller.Map;
using Core;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UserData.TileMap;
using View.Map;
using View.Window.Windows;

namespace View.HUD
{
    public class HudContent : MonoBehaviour
    {
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text bombsText;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text tapToContinue;
        [SerializeField] private CanvasGroup tapToContinueGroup;
        [SerializeField] private Button paletteButton;
        [SerializeField] private Button menuButton;
        [SerializeField] public CanvasGroup canvasGroup;
        
        private IDisposable timerSub;
        
        private void Awake()
        {
            AddListeners();
            HideToastText();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void AddListeners()
        {
            RemoveListeners();
            Game.User.Settings.OnLocalesChanged += OnLocalesChanged;
            MapController.OnMapLoaded += OnMapLoaded;
            MapController.OnMapDestroyed += RemoveMapListeners;
            
            paletteButton.onClick.AddListener(OnPaletteClick);
            menuButton.onClick.AddListener(MainMenuWindow.Of);
        }

        private void RemoveListeners()
        {
            timerSub?.Dispose();
            timerSub = null;
            
            Game.User.Settings.OnLocalesChanged -= OnLocalesChanged;
            MapController.OnMapLoaded -= OnMapLoaded;
            paletteButton.onClick.RemoveAllListeners();
            menuButton.onClick.RemoveAllListeners();
        }

        private void OnPaletteClick()
        {
            Game.User.Settings.BlackPalette = !Game.User.Settings.BlackPalette;
        }

        private void OnMapLoaded(MapController mapController)
        {
            AddMapListeners(mapController);
            Draw(mapController);
        }

        private void AddMapListeners(MapController mapController)
        {
            mapController.UserMap.CommandSystem.OnCommand += OnCommand;
        }

        private void RemoveMapListeners(MapController mapController)
        {
            mapController.UserMap.CommandSystem.OnCommand -= OnCommand;
        }

        private void OnCommand(ICommand obj)
        {
            DrawBombs();
        }

        private void OnLocalesChanged()
        {
            Draw(MapController.Instance);
        }


        private void Draw(MapController mapController)
        {
            DrawLevels(mapController);
            DrawBombs();
            DrawTimer(mapController);
            
            menuButton.gameObject.SetActive(!(mapController.MapView is MapTutorView));
        }

        private void DrawLevels(MapController mapController)
        {
            var isLevel = mapController.UserMap is UserMapLevel;
            
            levelText.gameObject.SetActive(isLevel);
            
            if (isLevel)
            {
                var userMapLevel = (UserMapLevel) MapController.Instance.UserMap;
                levelText.text = "level".Localize(userMapLevel.LevelData.Level.ToString());
            }
        }

        private void DrawTimer(MapController mapController)
        {
            var isClassic = mapController.UserMap is UserMapClassic;
            
            timerText.gameObject.SetActive(isClassic);
            
            if (isClassic)
            {
                var userMapClassic = (UserMapClassic) MapController.Instance.UserMap;
                timerSub?.Dispose();
                timerSub = userMapClassic.Timer.TimerSub.Subscribe(time =>
                {
                    timerText.text = userMapClassic.Timer.StringTime;
                });
            }
        }

        private void DrawBombs()
        {
            bombsText.text = MapController.Instance.UserMap.BombsLeft.ToString();
        }


        private TweenerCore<Vector3, Vector3, VectorOptions> tapToContinueTween;
        

        public void ShowToastText(string title = "tap_to_continue")
        {
            tapToContinue.gameObject.SetActive(true);
            tapToContinue.text = title.Localize();

            tapToContinueGroup.DOFade(1f, 0.5f)
                .From(0f)
                .SetEase(Ease.Linear)
                .SetLink(gameObject);

            tapToContinueTween?.Kill();
            tapToContinueTween = tapToContinue.transform.DOScale(Vector3.one, 1f)
                .From(new Vector3(0.9f, 0.9f, 0.9f))
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetLink(gameObject);
        }


        public void HideToastText()
        {
            tapToContinueTween?.Kill();
            tapToContinue.transform.localScale = Vector3.one;
            tapToContinue.gameObject.SetActive(false);
        }
    }
}