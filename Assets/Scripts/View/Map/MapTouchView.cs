using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using DG.Tweening;
using UnityEngine;
using View.Tile;

namespace View.Map
{
    public class MapTouchView : MonoBehaviour
    {
        [SerializeField] public MapView map;

        public event Action<MapObjectView> OnObjectMoveEvent;
        public event Action<MapObjectView> OnObjectDownEvent;
        public event Action<MapObjectView> OnObjectUpEvent;
        public event Action<MapObjectView> OnObjectLongClickEvent;
        public event Action<MapObjectView> OnObjectRightButtonUpEvent;
        public event Action<Vector3> OnWorldPointClickEvent;

        public event EventHandler TouchStart;

        public event Action<Vector2Int> OnTileDownEvent;
        public event Action<Vector2Int> OnTileUpEvent;

        protected Vector3 mousePos;
        private bool wasLongClick = false;
        private Tween longClickTween;
        
        protected MapObjectView currentDownObject;
        protected MapObjectView currentMoveOnObject;

        private List<MapObjectView> tmpObjectsFromHit = new List<MapObjectView>();

        public float LongClickTime { get; set; } = 0.25f;

        protected bool lastMouseDownNotOnUI;
        
        public virtual void Update()
        {
            var buttonDown = Input.GetMouseButtonDown(0);
            var buttonUp = Input.GetMouseButtonUp(0);
            var buttonHold = Input.GetMouseButton(0);
            var buttonRightHold = Input.GetMouseButton(1);
            var buttonRightUp = Input.GetMouseButtonUp(1);

            if (!buttonUp && !buttonDown && !buttonHold && !buttonRightUp && !buttonRightHold)
                return;

            var wasClickAfterUnfocus = buttonUp && buttonDown && buttonHold;
            if (wasClickAfterUnfocus)
                return;

            if (buttonUp)
                KillLongClick();
            
            var mousePosInput = Input.mousePosition;

            var invalidTouch = mousePosInput.x < 0 || mousePosInput.x >= Screen.width || mousePosInput.y < 0 || mousePosInput.y >= Screen.height;
            if (invalidTouch)
                return;

            if (buttonDown)
            {
                lastMouseDownNotOnUI = !AllUtils.Utils.IsPointerOverUIObject(true);
                currentDownObject = null;
                TouchStart?.Invoke(this, EventArgs.Empty);
            }

            if (map.viewPort.IsDrag || map.viewPort.IsZoom)
                wasDragOnTouch = true;
            
            if (!wasDragOnTouch)
            {
                mousePos = Game.MainCamera.ScreenToWorldPoint(mousePosInput);

                if (buttonUp && lastMouseDownNotOnUI)
                    OnWorldPointClickEvent?.Invoke(mousePos);

                if (buttonUp && currentDownObject && lastMouseDownNotOnUI)
                    OnObjectUp();
                else
                {
                    GetObjectsFromHit();

                    if (tmpObjectsFromHit.FirstOrDefault() is {} objectView)
                        OnObjectTouch(objectView);
                    else
                        OnTileTouch();
                }
            }
            
            if(!buttonHold && !buttonRightHold)
                wasDragOnTouch = false;

            void GetObjectsFromHit()
            {
                tmpObjectsFromHit = Physics2D.RaycastAll(new Vector2(mousePos.x, mousePos.y), Vector2.zero)
                    .Select(GetObjectViewFromCollider)
                    .Where(o => o != null)
                    .ToList();
            }

            void OnObjectTouch(MapObjectView objectView)
            {
                if (buttonDown && lastMouseDownNotOnUI)
                {
                    OnObjectDown(objectView);
                }
                else if (buttonHold && !currentDownObject && currentMoveOnObject != objectView)
                {
                    currentMoveOnObject = objectView;
                    OnObjectMoveEvent?.Invoke(objectView);
                }
                else if (buttonRightUp)
                {
                    OnObjectRightButtonUpEvent?.Invoke(objectView);
                }
            }

            void OnObjectDown(MapObjectView objectView)
            {
                KillLongClick();

                longClickTween = DOVirtual.DelayedCall(LongClickTime, () =>
                {
                    KillLongClick();
                    
                    if(!map || !map.viewPort)
                        return;

                    if (!map.viewPort.IsDrag && !map.viewPort.IsZoom)
                    {
                        wasLongClick = true;
                        OnObjectLongClickEvent?.Invoke(objectView);
                    }
                });

                currentDownObject = objectView;
                OnObjectDownEvent?.Invoke(objectView);
            }

            void OnObjectUp()
            {
                wasDragOnTouch = false;
                
                if (wasLongClick)
                    wasLongClick = false;
                else
                {
                    OnObjectUpEvent?.Invoke(currentDownObject);
                    currentDownObject = null;
                }
            }

            void OnTileTouch()
            {
                currentMoveOnObject = null;
                mousePos.z = 0;
                TileMatrixView tileMatrix = map.tileMatrix;
                var tilePos = tileMatrix.WorldToCell(mousePos);

                if (lastMouseDownNotOnUI)
                {
                    if (buttonDown)
                        OnTileDownEvent?.Invoke(tilePos);
                    else if (buttonUp)
                        OnTileUpEvent?.Invoke(tilePos);
                }
            }
            
            MapObjectView GetObjectViewFromCollider(RaycastHit2D raycast) =>
                raycast.collider.gameObject.transform.GetComponentInParent<MapObjectView>();
        }

        private bool wasDragOnTouch = false;


        protected virtual void KillLongClick()
        {
            longClickTween?.Kill();
            longClickTween = null;
        }
    }
}
