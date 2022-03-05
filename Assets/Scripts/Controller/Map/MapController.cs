using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem.Commands;
using Core;
using Core.AssetsManager;
using JetBrains.Annotations;
using Libraries.RSG;
using ModelData.TileMap;
using UniRx;
using UnityEngine;
using UserData.Level;
using UserData.TileMap;
using Utils;
using View.Map;
using View.Tile;
using View.Window;
using Object = UnityEngine.Object;

namespace Controller.Map
{
    /// <summary>
    /// СОЗДАЕТ ТОЛЬКО ПРЕДСТАВЛЕНИЯ! Больше нигде представления не должны создаваться.
    /// </summary>
    public class MapController : IDisposable
    {
        private static MapController instance;
        public static MapController Instance => instance;

        public static event Action<MapController> OnMapLoaded;
        public static event Action<MapController> OnMapDestroyed;
        
        [NotNull]
        public readonly UserMap UserMap;

        public int Level => (UserMap.Data as MapLevelData)?.Level ?? default;

        private Dictionary<Vector2Int, MapObjectView> objects = new Dictionary<Vector2Int, MapObjectView>();

        public MapView MapView { get; private set; }
        private IDisposable onAppActiveChange;

        public static void Load()
        {
            instance?.Dispose();
            try
            {
                var userMap = UserMapFactory.LoadUserMap();
                if (userMap != null && userMap.State != UserMapState.GameOver)
                    Create(userMap);
                else
                    Create(Game.User.Level);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + "\n" + e.StackTrace);
                Create(Game.User.Level);
            }
        }
        
        public static void Create(int level, int? seed = null)
        {
            instance?.Dispose();
            UserMapSaver.RemoveMapData();
            var userMap = UserMapFactory.CreateUserMap(level, seed);
            UserMapSaver.SaveMapData(userMap.Data);
            new MapController(userMap);
        }
        
        public static void Create(MapDataType type)
        {
            instance?.Dispose();
            UserMapSaver.RemoveMapData();
            var userMap = UserMapFactory.CreateUserMap(type);
            UserMapSaver.SaveMapData(userMap.Data);
            new MapController(userMap);
        }
        
        public static void Create(MapData data)
        {
            instance?.Dispose();
            UserMapSaver.RemoveMapData();
            var userMap = UserMapFactory.CreateUserMap(data);
            UserMapSaver.SaveMapData(userMap.Data);
            new MapController(userMap);
        }
        
        public static void Create(UserMap userMap)
        {
            instance?.Dispose();
            new MapController(userMap);
        }

        public static void Recreate()
        {
            if(Instance.UserMap is UserMapLevel userMapLevel)
                Create(userMapLevel.LevelData.Level);
            else if(Instance.UserMap is UserMapClassic userMapClassic)
                Create(userMapClassic.ClassicData.type);
            else
                Create(Instance.UserMap.Data);
        }
        
        public MapController(UserMap userMap)
        {
            instance = this;
            
            UserMap = userMap;
            CreateView();
            CreateAllObjects();
            UpdateMapObjectsByState();
            AddListeners();

            OnMapLoaded?.Invoke(this);
        }

        private void AddListeners()
        {
            RemoveListeners();
            UserMap.CommandSystem.OnUndo += OnCommand;
            UserMap.CommandSystem.OnRedo += OnCommand;
            UserMap.OnStateUpdated += OnMapStateUpdated;
            WindowsController.Instance.OnWindowEvent += OnWindowEvent;

            onAppActiveChange = Game.Instance.AppActive.Subscribe(_ => UserMap.TryUpdateState());
        }

        private void OnWindowEvent()
        {
            UpdateMapObjectsByState();
        }

        private void RemoveListeners()
        {
            UserMap.CommandSystem.OnUndo -= OnCommand;
            UserMap.CommandSystem.OnRedo -= OnCommand;
            UserMap.OnStateUpdated -= OnMapStateUpdated;
            
            onAppActiveChange?.Dispose();
            onAppActiveChange = null;
        }

        private void OnCommand(ICommand command)
        {
            if (command is ITilesCommand tilesCommand)
            {
                PathFinder.getWave(tilesCommand.ChangedTiles, (pos, iteration) =>
                {
                    if(objects.ContainsKey(pos))
                        objects[pos].UpdateUserData(UserMap[pos]);
                    return iteration < 1;
                }, true, true);
            }
        }

        private bool IsTutorLevel(int level) => level <= 2;

        private void CreateView()
        {
            if(UserMap is UserMapLevel userMapLevel && IsTutorLevel(userMapLevel.LevelData.Level))
                MapView = AssetsLoader.CreateSync<MapTutorView>("prefabs/TutorMap");
            else
                MapView = AssetsLoader.CreateSync<MapView>("prefabs/Map");
            
            MapView.SetUserData(UserMap);
        }

        private void CreateAllObjects()
        {
            var isGenerated = UserMap.IsGenerated;
            
            for(var x = 0; x < UserMap.Width; x ++)
            for (var y = 0; y < UserMap.Height; y++)
            {
                var pos = new Vector2Int(x, y);
                objects[pos] = CreateObjectView();
                
                if (isGenerated)
                {
                    objects[pos].Init(MapView, UserMap[pos], pos);
                }
                else
                {
                    objects[pos].Init(MapView);
                    objects[pos].SetPosition(pos);
                }
            }
        }

        private void OnMapStateUpdated(UserMapState state)
        {
            UpdateMapObjectsByState();
        }

        private void UpdateMapObjectsByState()
        {
            var state = UserMap.State;
            var needShow = state != UserMapState.Pause && WindowsController.Instance.ActiveWindows.All(w => w.Settings.ShowCellsText);
            var forceWhiteText = state == UserMapState.Complete || state == UserMapState.GameOver;
            
            objects.Values.Each(m => m.OnMapStateUpdated(needShow, forceWhiteText));
            objects.Values.Each(m => m.TryUpdateState());
        }


        private MapObjectView CreateObjectView()
        {
            return AssetsLoader.CreateSync<MapObjectView>("prefabs/tile", MapView.transform);
        }
        

        public void Dispose()
        {
            RemoveListeners();
            Object.Destroy(MapView.gameObject);
            objects.Clear();
            
            instance = null;
        }
    }
}