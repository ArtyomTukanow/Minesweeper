using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using CommandSystem.Commands;
using Controller;
using Core;
using JetBrains.Annotations;
using ModelData.TileMap;
using UnityEngine;
using UserData.TileMap.Generators;
using Utils;
using View.Window;

namespace UserData.TileMap
{
    public class UserMap : IDisposable
    {
        public Vector2Int Size => new Vector2Int(Data.Width, Data.Height);
        
        public int Width => Data.Width;
        public int Height => Data.Height;
        public int Bombs => Data.Bombs;
        public int CellsCount => Height * Width;
        
        public Color Color => ColorSystem.GetOpenedColor(Data.Hue);
        
        public readonly MapData Data;

        public readonly TileMapGeneratorAbstract Generator;
        public readonly UserCommandSystem CommandSystem;
        
        public UserMapState State { get; private set; } = UserMapState.UNDEFINED;
        public event Action<UserMapState> OnStateUpdated;
        
        public Dictionary<Vector2Int, UserTile> Field => Generator.Field;

        public bool IsGenerated => Generator.IsGenerated;
        
        [CanBeNull]
        public UserTile this[int x, int y] => Field.TryGetValue(new Vector2Int(x, y), out var t) ? t : null;
        [CanBeNull]
        public UserTile this[Vector2Int pos] => Field.TryGetValue(pos, out var t) ? t : null;

        public UserMap(MapData data, TileMapGeneratorAbstract generator)
        {
            data.ThrowIfNotValid();
            
            Data = data;
            Generator = generator;
            
            CommandSystem = new UserCommandSystem(this);
            
            CommandSystem.OnUndo += OnCommand;
            CommandSystem.OnRedo += OnCommand;
        }
        
        private void OnCommand(ICommand obj)
        {
            TryUpdateState();
        }

        public void TryUpdateState()
        {
            var s = StateCalculate();
            
            if (State == s)
                return;
                
            State = s;
            OnStateUpdated?.Invoke(s);
        }

        public void ForceUpdateState()
        {
            OnStateUpdated?.Invoke(State);
        }

        public int BombsLeft => Bombs - Field.Values.Count(t => t.Flagged);

        private UserMapState StateCalculate()
        {
            if (Field.Values.Any(t => t.Opened && t.Bomb))
                return UserMapState.GameOver;
            
            if (Field.Values.Count(t => t.Opened) >= CellsCount - Bombs)
                return UserMapState.Complete;

            if (!Game.IsAppActive)
                return UserMapState.Pause;

            if (WindowsController.Instance && WindowsController.Instance.ActiveWindows.Any(w => w.Settings.PauseGameOnShow))
                return UserMapState.Pause;

            return UserMapState.Play;
        }

        public bool ContainsTile(Vector2Int pos) => ContainsTile(pos.x, pos.y);
        public bool ContainsTile(int x, int y) => x >= 0 && y >= 0 && x < Width && y < Height;

        public void TryGenerateField(Vector2Int startPos)
        {
            if (!Generator.IsGenerated)
                GenerateWithAttempts(startPos);
        }

        private void GenerateWithAttempts(Vector2Int startPos, int attempt = 0)
        {
            if(attempt > 5)
                throw new Exception("Max attempts for generate limited!");
            
            try
            {
                Generator.Generate(this, startPos);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + "\n" + e.StackTrace);
                GenerateWithAttempts(startPos, attempt + 1);
            }
        }


        public List<Vector2Int> GetNearPoints(Vector2Int pos)
        {
            var flaggedPoints = new List<Vector2Int>();
            PathFinder.getWave(pos, NearPointNext, true);
            return flaggedPoints;
            
            bool NearPointNext(Vector2Int pos, int iteration)
            {
                flaggedPoints.Add(pos);
                return false;
            }
        }

        public virtual void Dispose()
        {
        }
    }
}