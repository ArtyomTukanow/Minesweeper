using System;
using CommandSystem.Commands;
using Tutors.Base;
using Tutors.Start;
using Tutors.Start2;
using UnityEngine;
using UserData.TileMap;
using View.Tile;

namespace View.Map
{
    public class MapTutorView : MapView
    {
        public event Action<Vector2Int> OnTileClickEvent;
        public event Action<Vector2Int> OnTileLongClickEvent;

        protected override void CreateViewPort()
        {
            base.CreateViewPort();
            viewPort.IsZoomAvailable = false;
            viewPort.IsDragAvailable = false;
            viewPort.ZoomAndMove(7, border.offset, 0);
        }

        private ComplexTutor tutor;

        public override void SetUserData(UserMap userMap)
        {
            base.SetUserData(userMap);

            if (userMap is UserMapLevel level)
            {
                if (level.LevelData.Level == 1)
                {
                    userMap.Data.Seed = 651914732;
                    userMap.CommandSystem.NeedSaveCommands = false;
                    tutor = ComplexTutor.RunTutor<StartTutor1>();
                }
                else if (level.LevelData.Level == 2)
                {
                    userMap.Data.Seed = 1592363585;
                    userMap.CommandSystem.NeedSaveCommands = false;
                    tutor = ComplexTutor.RunTutor<StartTutor2>();
                }
                else
                {
                    throw new Exception("unknown level " + level.LevelData.Level + " for tutor!");
                }
            }
            
        }

        protected override void LockTouchForOneSec()
        {
        }

        public override void ZoomByCommand(ICommand cmd)
        {
        }

        protected override void OnClickObject(MapObjectView mapObject)
        {
            if (tutor.Disposed)
                base.OnClickObject(mapObject);
            else
                OnTileClickEvent?.Invoke(mapObject.Position);
        }

        protected override void OnLongClickObject(MapObjectView mapObject)
        {
            if (tutor.Disposed)
                base.OnLongClickObject(mapObject);
            else
                OnTileLongClickEvent?.Invoke(mapObject.Position);
        }

        protected override void OnTileClick(Vector2Int position)
        {
            if (tutor.Disposed)
                base.OnTileClick(position);
            else
                OnTileClickEvent?.Invoke(position);
        }
        
        protected override void UpdateBorder()
        {
            var startPos = tileMatrix.GetCellCenterWorld(Vector2Int.zero);
            var endPos = tileMatrix.GetCellCenterWorld(UserMap.Size);

            var borderWidth = Mathf.Abs(startPos.x - endPos.x);
            var borderHeight = Mathf.Abs(startPos.y - endPos.y);
            
            border.offset = new Vector2(borderWidth / 2, borderHeight / 2);

            border.size = new Vector2(1000, 1000);
        }
    }
}