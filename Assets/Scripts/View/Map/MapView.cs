using System;
using System.Linq;
using CommandSystem.Commands;
using Controller;
using Controller.Map;
using Core;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;
using UserData.Level;
using UserData.TileMap;
using Utils;
using View.Tile;
using View.Window.Windows;

namespace View.Map
{
    public class MapView : MonoBehaviour
    {
        [SerializeField] public TileMatrixView tileMatrix;
        [SerializeField] public BoxCollider2D border;

        [SerializeField] public MapViewPort viewPort;
        [SerializeField] public MapTouchView touchView;

        [NotNull]
        public UserMap UserMap { get; private set; }

        private bool needZoomOnCommand = true;
        private bool needSaveZoom = false;

        public virtual void SetUserData(UserMap userMap)
        {
            UserMap = userMap;

            UpdateByUserData();
            CreateViewPort();
            AddTouchListeners();
            AddListeners();
            
            OnMapStateUpdated(UserMap.State);

            TimerUtils.NextFrame()
                .Then(() => ZoomByCommand(UserMap.CommandSystem.LastCommand));

            if (!userMap.IsGenerated)
                ShowTapToStart();
        }

        protected virtual void CreateViewPort()
        {
            viewPort.Init(2f, 20f, 5f, tileMatrix, border);
            viewPort.ZoomAndMove(int.MaxValue, border.offset, 0);
        }

        private void AddListeners()
        {
            RemoveListeners();
            UserMap.OnStateUpdated += OnMapStateUpdated;
            UserMap.CommandSystem.OnRedo += OnCommand;
            viewPort.OnZoomChange += OnZoomChange;
        }


        private void RemoveListeners()
        {
            UserMap.OnStateUpdated -= OnMapStateUpdated;
            UserMap.CommandSystem.OnRedo -= OnCommand;
            if(viewPort)
                viewPort.OnZoomChange -= OnZoomChange;
        }

        private void OnZoomChange()
        {
            needZoomOnCommand = false;
        }
        
        private void OnCommand(ICommand cmd)
        {
            Game.Hud.Content.HideToastText();
            
            needSaveZoom = true;
            
            if (needZoomOnCommand && cmd is ITilesCommand)
            {
                needZoomOnCommand = false;
                ZoomByCommand(cmd);
            }
        }

        public virtual void ZoomByCommand(ICommand cmd)
        {
            if (cmd is ITilesCommand tilesCommand)
            {
                if(Math.Abs(Game.User.Settings.Zoom - viewPort.Zoom) > 0.1)
                    viewPort.ZoomAndMove(Game.User.Settings.Zoom, tilesCommand.ChangedTiles.FirstOrDefault(), 1f);
            }
        }

        private void OnMapStateUpdated(UserMapState state)
        {
            Game.Hud.Content.HideToastText();
            switch (state)
            {
                case UserMapState.GameOver: SetGameOverState(); break;
                case UserMapState.Play: SetPlayState(); break;
                case UserMapState.Pause: SetPauseState(); break;
                case UserMapState.Complete: SetCompleteState(); break;
            }

            SaveZoom();
        }

        private void SaveZoom()
        {
            if(needSaveZoom)
                Game.User.Settings.Zoom = viewPort.Zoom;
        }

        private void SetGameOverState()
        {
            UpdateToGameOverColor();
            LockTouchForOneSec();
            ShowTapToReStart();
        }

        private void ClickOnComplete() => LevelEndController.OnGameWin();
        private void ClickOnGameOver() => LevelEndController.OnGameOver();
        
        private void SetPlayState() => UpdateToGamePlayColor();
        private void SetPauseState() => UpdateToGamePlayColor();

        private void SetCompleteState()
        {
            if(UserMap is UserMapLevel)
                Game.User.Level++;
            UserMapSaver.RemoveMapData();
            UpdateToCompleteColor();

            LockTouchForOneSec();
            ShowTapToContinue();
        }

        private void ShowTapToContinue()
        {
            var state = UserMap.State;
            TimerUtils.Wait(2).Then(() =>
            {
                if(this && UserMap.State == state)
                    Game.Hud.Content.ShowToastText();
            });
        }

        private void ShowTapToStart()
        {
            TimerUtils.Wait(2).Then(() =>
            {
                if(this && !UserMap.IsGenerated)
                    Game.Hud.Content.ShowToastText("tap_to_start");
            });
        }

        private void ShowTapToReStart()
        {
            var state = UserMap.State;
            TimerUtils.Wait(2).Then(() =>
            {
                if(this && UserMap.State == state)
                    Game.Hud.Content.ShowToastText("tap_to_restart");
            });
        }

        protected virtual void LockTouchForOneSec()
        {
            touchView.enabled = false;
            TimerUtils.Wait(1)
                .Then(() =>
                {
                    touchView.enabled = true;
                });
        }

        private void DOColor(Material material, Color toColor, float duration = 0.2f)
        {
            material.DOColor(toColor, duration)
                .SetLink(gameObject)
                .SetEase(Ease.Linear);
        }

        private void DOColor(Tilemap tilemap, Color toColor, float duration = 0.2f)
        {
            DOTween.To(() => tilemap.color, color => tilemap.color = color, toColor, duration)
                .SetLink(gameObject)
                .SetEase(Ease.Linear);
        }

        private void DOColor(Camera camera, Color toColor, float duration = 0.2f)
        {
            DOTween.To(() => camera.backgroundColor, color => camera.backgroundColor = color, toColor, duration)
                .SetLink(gameObject)
                .SetEase(Ease.Linear);
        }

        private void UpdateToGameOverColor()
        {
            DOColor(BasePrefabs.Instance.openTileMaterial, BasePrefabs.Instance.openTileGameOverColor, 0.4f);
            DOColor(BasePrefabs.Instance.closeTileMaterial, BasePrefabs.Instance.closeTileGameOverColor, 0.4f);
            DOColor(BasePrefabs.Instance.flagTileMaterial, BasePrefabs.Instance.flagTileGameOverColor, 0.4f);
            DOColor(tileMatrix.Tilemap, BasePrefabs.Instance.openCellBlackColor, 0.4f);
            DOColor(Game.MainCamera, BasePrefabs.Instance.backgroundGameOverColor, 0.4f);
        }

        private void UpdateToGamePlayColor()
        {
            DOColor(BasePrefabs.Instance.openTileMaterial, ColorSystem.GetOpenedColor(UserMap.Data.Hue));
            DOColor(BasePrefabs.Instance.closeTileMaterial, ColorSystem.GetClosedColor(UserMap.Data.Hue));
            DOColor(BasePrefabs.Instance.flagTileMaterial, ColorSystem.GetFlaggedColor(UserMap.Data.Hue));
            DOColor(tileMatrix.Tilemap, ColorSystem.GetCellColor());
            DOColor(Game.MainCamera, ColorSystem.GetBackColor(UserMap.Data.Hue));
        }

        private void UpdateToCompleteColor()
        {
            DOColor(BasePrefabs.Instance.openTileMaterial, BasePrefabs.Instance.openTileCompleteColor);
            DOColor(BasePrefabs.Instance.closeTileMaterial, BasePrefabs.Instance.closeTileCompleteColor);
            DOColor(BasePrefabs.Instance.flagTileMaterial, BasePrefabs.Instance.flagTileCompleteColor);
            DOColor(tileMatrix.Tilemap, BasePrefabs.Instance.openCellBlackColor);
            DOColor(Game.MainCamera, BasePrefabs.Instance.backgroundCompleteColor);
        }

        private void AddTouchListeners()
        {
            RemoveTouchListeners();
            touchView.OnObjectUpEvent += OnClickObject;
            touchView.OnObjectLongClickEvent += OnLongClickObject;
            touchView.OnObjectRightButtonUpEvent += OnLongClickObject;
            touchView.OnTileUpEvent += OnTileClick;
        }

        private void RemoveTouchListeners()
        {
            touchView.OnObjectUpEvent -= OnClickObject;
            touchView.OnObjectLongClickEvent -= OnLongClickObject;
            touchView.OnObjectRightButtonUpEvent -= OnLongClickObject;
            touchView.OnTileUpEvent -= OnTileClick;
        }

        protected virtual void OnClickObject(MapObjectView mapObject)
        {
            if (UserMap.State == UserMapState.GameOver)
            {
                ClickOnGameOver();
                return;
            }
            if (UserMap.State == UserMapState.Complete)
            {
                ClickOnComplete();
                return;
            }
            Debug.Log($"Click on: {mapObject.UserTile}");
            mapObject.OnClick();
        }

        protected virtual void OnLongClickObject(MapObjectView mapObject)
        {
            if (UserMap.State == UserMapState.GameOver)
            {
                ClickOnGameOver();
                return;
            }
            if (UserMap.State == UserMapState.Complete)
            {
                ClickOnComplete();
                return;
            }
            Debug.Log($"Long click on: {mapObject.UserTile}");
            mapObject.OnLongClick();
        }

        protected virtual void OnTileClick(Vector2Int position)
        {
            if (UserMap.State == UserMapState.GameOver)
            {
                ClickOnGameOver();
                return;
            }
            if (UserMap.State == UserMapState.Complete)
            {
                ClickOnComplete();
                return;
            }
            Debug.Log($"On tile click: {position}");
            UserMap.CommandSystem.OnClickTile(position);
        }


        private void UpdateByUserData()
        {
            UpdateTileMatrix();
            UpdateBorder();
        }
        
        private void UpdateTileMatrix() => tileMatrix.SetUserData(UserMap);

        protected virtual void UpdateBorder()
        {
            var startPos = tileMatrix.GetCellCenterWorld(Vector2Int.zero);
            var endPos = tileMatrix.GetCellCenterWorld(UserMap.Size);

            var borderWidth = Mathf.Abs(startPos.x - endPos.x);
            var borderHeight = Mathf.Abs(startPos.y - endPos.y);
            
            border.offset = new Vector2(borderWidth / 2, borderHeight / 2);

            if (Screen.width > Screen.height)
                borderWidth = borderHeight * Screen.width / Screen.height;
            else
                borderHeight = borderWidth * Screen.height / Screen.width;

            border.size = new Vector2(borderWidth + 4f, borderHeight + 4f);
        }
    }
}