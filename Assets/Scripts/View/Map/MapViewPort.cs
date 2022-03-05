using System;
using System.Collections.Generic;
using Core;
using DG.Tweening;
using Libraries.RSG;
using UnityEngine;
using Utils;

namespace View.Map
{
    public class MapViewPort : MonoBehaviour
    {
        private static List<float> VELOCITY_WEIGHTS = new List<float> { 1f, 1.33f, 1.66f, 2f };
        private const int MAXIMUM_SAVED_VELOCITY_COUNT = 4;
        private const float CURRENT_VELOCITY_WEIGHT = 2.33f;

        private const float SIZE_TOLERANCE = .1f;

        public delegate void ChangeViewPort();
        public event ChangeViewPort ChangeViewPortEvent;
        public event ChangeViewPort OnPositionChange;
        public event ChangeViewPort OnZoomChange;

        private BoxCollider2D border;
        private TileMatrixView tileMatrix;

        public Bounds Bounds => border.bounds;
        private Bounds CameraBounds;

        public bool IsDragAvailable { get; set; } = true;
        public bool IsZoomAvailable { get; set; } = true;

        public bool IsDrag { get; private set; }
        public bool IsZoom { get; private set; }
        private bool isDown;

        private bool needSkipDrag;

        private float minSize = 1f;
        private float maxSize = 10f;

        public void SetDragAndZoomAvailable(bool val)
        {
            if (!IsDragAvailable && val)
                needSkipDrag = true;

            IsDragAvailable = IsZoomAvailable = val;
        }
        
        
        private Vector2 downPoint;
        private Vector2 speed = new Vector2(0f, 0f);
        private float near = 0.006f;
        private float velocity = 0.95f;
        private List<Vector2> previousVelocity = new List<Vector2>();
        private Camera camera;
        private float targetX;
        private float targetY;

        private float wasMagnitude = 0;
        private float curMagnitude = 0;
        
        public Vector2Int CenterPoint => tileMatrix.WorldToCell(camera.gameObject.transform.position);

        private Tweener movingTween = null;

        public void IncreaseBounds(float value)
        {
            Bounds.Expand(value);
        }

        public void Init(float minSize, float maxSize, float currentScale, TileMatrixView tileMatrix, BoxCollider2D border)
        {
            this.tileMatrix = tileMatrix;
            this.border = border;
            
            camera = Camera.main;
            
            if(!camera)
                return;

            Vector3 minmin = camera.ScreenToWorldPoint(new Vector3(0f, 0f, 0f));
            Vector3 maxmax = camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, camera.pixelHeight, 0f));

            Vector2 cameraSize = new Vector2(Bounds.size.x * camera.orthographicSize / (maxmax.x - minmin.x) - SIZE_TOLERANCE,
                                             Bounds.size.y * camera.orthographicSize / (maxmax.y - minmin.y) - SIZE_TOLERANCE);
            
            this.minSize = minSize;
            var boundsMinSize = Screen.width > Screen.height ? Bounds.size.x / 2 : Bounds.size.y / 2;
            this.maxSize = Mathf.Min(maxSize, boundsMinSize, cameraSize.x, cameraSize.y);
            SetCameraSize(Mathf.Clamp(currentScale, minSize, maxSize));
            ClampCameraPosition();

            AddListeners();
        }

		protected virtual void OnDestroy()
        {
            RemoveListeners();
        }

        private void RemoveListeners()
        {
            KillTween();
            Game.OnResize -= OnResize;
        }

        private void AddListeners()
        {
            RemoveListeners();
            Game.OnResize += OnResize;
        }

		private void OnResize()
		{
			CameraBounds = OrthographicBounds();
			ClampCameraPosition();
		}

        private void KillTween()
        {
            movingTween?.Kill();
            movingTween = null;
        }

        void Update()
        {
            var uiBlocks = false;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WSA || UNITY_WEBGL
            uiBlocks = AllUtils.Utils.IsPointerOverUIObject(true);
            if (!uiBlocks)
            {
                if (Input.GetMouseButtonDown(1) && !Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) && !Input.GetMouseButton(1))
                {
                    OnMouseDown();
                    needSkipDrag = false;
                }
                else if (Input.GetMouseButtonUp(1) && !Input.GetMouseButton(0) || Input.GetMouseButtonUp(0) && !Input.GetMouseButton(1))
                {
                    OnMouseUp();
                }
                else if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && (!Input.GetAxis("Mouse X").Equals(0f) || !Input.GetAxis("Mouse Y").Equals(0f)))
                {
                    OnMouseMove();
                }
            }

            if (!Input.mouseScrollDelta.y.Equals(0) && IsZoomAvailable)
            {
                finishDrag();
                var curSize = camera.orthographicSize;
                curSize -= Input.mouseScrollDelta.y / 2f;
                curSize = Mathf.Clamp(curSize, minSize, maxSize);
                SetCameraSize(curSize);
                CameraBounds = OrthographicBounds();
                ClampCameraPosition();
            }
#endif

            if (Input.touchCount > 0)
            {
                uiBlocks = AllUtils.Utils.IsPointerOverUIObject(true);
                
                if (Input.touchCount >= 2 && IsZoomAvailable && !uiBlocks)
                {
                    finishDrag();

                    curMagnitude = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);

                    if (!IsZoom)
                    {
                        wasMagnitude = curMagnitude;
                    }

                    IsZoom = true;
                    SetCameraSize(Mathf.Clamp(camera.orthographicSize / (curMagnitude / wasMagnitude), minSize,
                        maxSize));

                    wasMagnitude = curMagnitude;
                    
                    CameraBounds = OrthographicBounds();
                    ClampCameraPosition();

                    return;
                }
                else if (Input.touchCount == 1 && !uiBlocks)
                {
                    var touch = Input.GetTouch(0).phase;
                    if (touch == TouchPhase.Began)
                    {
                        OnMouseDown();
                        needSkipDrag = false;
                    }
                    else if (touch == TouchPhase.Ended)
                    {
                        OnMouseUp();
                    }
                    else if (touch == TouchPhase.Moved)
                    {
                        OnMouseMove();
                    }
                }
                else
                    IsZoom = false;
            }
            else
                IsZoom = false;

            if (!IsDrag) return;
            if (isDown) return;

            targetX += speed.x;
            targetY += speed.y;

            targetX = getClampX(targetX);
            targetY = getClampY(targetY);

            speed.x *= velocity;
            speed.y *= velocity;

            if (Mathf.Abs(camera.gameObject.transform.position.x - targetX) >= near ||
                Mathf.Abs(camera.gameObject.transform.position.y - targetY) >= near)
            {
                SetCameraPosition(new Vector3(targetX, targetY, camera.gameObject.transform.position.z));
            }
            else
            {
                if (!isDown)
                {
                    finishDrag();
                }
            }
        }

		public void AnimZoom(bool isIn = true)
		{
			float mod = isIn ? -1f : 1f;
			var wasZoom = camera.orthographicSize;
			var zoom = Mathf.Clamp(wasZoom + mod, minSize, maxSize);
			var delta = zoom - wasZoom;

			float value = 0f;
			float time = 0.3f;

			KillTween();

			movingTween = DOTween.To(() => value,
									 val => value = val,
									 1,
									 time)
								 .OnUpdate(onUpdate)
								 .SetLink(gameObject);

			void onUpdate()
			{
				SetCameraSize(wasZoom + delta * value);
				CameraBounds = OrthographicBounds();
				ClampCameraPosition();
			}
		}

        public float Zoom => camera.orthographicSize;

        private void SetCameraSize(float size)
        {
            if (camera.orthographicSize.Equals(size)) return;

            camera.orthographicSize = Mathf.Clamp(size, minSize, maxSize);

            ChangeViewPortEvent?.Invoke();
            OnZoomChange?.Invoke();
        }

        private void SetCameraPosition(Vector3 pos, bool needChangeEvent = true)
        {
            if (camera.gameObject.transform.position.Equals(pos)) return;

            camera.gameObject.transform.position = pos;

            if (needChangeEvent)
            {
                ChangeViewPortEvent?.Invoke();
                OnPositionChange?.Invoke();
            }
        }
        
        // public IPromise MoveTo(IsoObject isoObject, float time, float cameraSize = 0f, Vector3 delta = default)
        // {
        //     if (isoObject == null)
        //         return Promise.Resolved();
        //     
        //     var colliders = isoObject.GetComponentsInChildren<Collider2D>();
        //     var bounds = new Bounds();
        //     foreach (var collider in colliders)
        //     {
        //         if (bounds == default)
        //             bounds = collider.bounds;
        //         else
        //             bounds.Encapsulate(collider.bounds);
        //     }
        //
        //     if (colliders.Length == 0)
        //         return MoveTo(isoObject.PositionMax, time, cameraSize);
        //     
        //     var toY = (bounds.max.y + bounds.min.y) / 2;
        //     
        //     var max = isoObject.Container.TileMatrix.GetCellCenterWorld(isoObject.PositionMax);
        //     var point = new Vector3((max.x + isoObject.transform.position.x) / 2f, toY, isoObject.transform.position.z);
        //     point += delta;
        //     cameraSize = cameraSize.Equals(0f) ? camera.orthographicSize : cameraSize;
        //     return ZoomAndMove(cameraSize, point, time);
        // }

        // public IPromise ZoomAndMoveTo(IsoObject isoObject, Vector3 delta = default, float timer = MapTouchController.TIME_TO_MOVE_AND_ZOOM) => MoveTo(isoObject,
        //     timer, MapTouchController.ZOOM, delta);
        
        public IPromise MoveTo(Vector2Int tilePosition, float time, float cameraSize = 0f)
        {
            if(!camera)
                return Promise.Resolved();
            
            var point = tileMatrix.GetCellCenterWorld(tilePosition);
            cameraSize = cameraSize.Equals(0f) ? camera.orthographicSize : cameraSize;
            return ZoomAndMove(cameraSize, point, time);
        }

        public IPromise ZoomAndMove(float zoom, Vector2Int tilePosition, float time)
        {
            var point = tileMatrix.GetCellCenterWorld(tilePosition);
            return ZoomAndMove(zoom, point, time);
        }

        public IPromise ZoomAndMove(float zoom, Vector3 point, float time, Ease ease = Ease.InOutCubic)
        {
            finishDrag();
            var cameraMovePromise = new Promise();

            zoom = Mathf.Clamp(zoom, minSize, maxSize);
            //point.x = getClampX(point.x);
            //point.y = getClampY(point.y);

            var targetDPoint = new Vector3(point.x - camera.gameObject.transform.position.x,
                point.y - camera.gameObject.transform.position.y,
                camera.gameObject.transform.position.z);

            var wasZoom = camera.orthographicSize;
            var wasPosition = camera.gameObject.transform.position;
            var targetDZoom = zoom - camera.orthographicSize;

            float value = 0f;

            if (time == 0f)
            {
                value = 1f;
                onUpdate();
                cameraMovePromise.Resolve();
            }
            else
            {
                movingTween = DOTween.To(() => value,
                                         val => value = val,
                                         1,
                                         time)
                                     .OnUpdate(onUpdate)
                                     .SetEase(ease)
                                     .SetLink(gameObject);
                
                TimerUtils.Wait(time).Then(cameraMovePromise.Resolve);
            }

            return cameraMovePromise;

            void onUpdate()
            {
                SetCameraSize(wasZoom + targetDZoom * value);

                SetCameraPosition(new Vector3(
                                              wasPosition.x + targetDPoint.x * value,
                                              wasPosition.y + targetDPoint.y * value,
                                              camera.gameObject.transform.position.z));

                CameraBounds = OrthographicBounds();

                ClampCameraPosition();
            }
        }
        
        
        /* Выбираем такой зум и позицию, чтобы все точки были на экране */
        public IPromise ZoomAndMoveToAllPoints(List<Vector2Int> points, float time = 0f/* , float scaleFactor = 1f, float maxZoom = 1 */)
        {
            float minX = int.MaxValue;
            float minY = int.MaxValue;
            float maxX = int.MinValue;
            float maxY = int.MinValue;

            var centerX = 0;
            var centerY = 0;

            foreach(var point in points)
            {
                var worldPoint = tileMatrix.GetCellCenterWorld(point);
                
                minX = Math.Min(worldPoint.x, minX);
                minY = Math.Min(worldPoint.y, minY);
                maxX = Math.Max(worldPoint.x, maxX);
                maxY = Math.Max(worldPoint.y, maxY);
            }

            var centerXAxis = minX + ((maxX - minX) * 0.5f);
            var centerYAxis = maxY - ((maxY - minY) * 0.5f);
            var center = tileMatrix.WorldToCell(new Vector3(centerXAxis, centerYAxis, 0f));
            centerX = (int)center.x;
            centerY = (int)center.y;

            var widthOffset = 5;
            var heightOffset = 5;

            var targetWidth = (maxX - minX) + (widthOffset);
            var targetHeight = (maxY - minY) + (heightOffset);

            var screenRation = (float)Screen.width / (float)Screen.height;
            var targetRatio = targetWidth / targetHeight;
            var cameraSize = minSize;

            if (screenRation >= targetRatio)
            {
                cameraSize = targetHeight * 0.5f;
            }
            else
            {
                var differenceInSize = targetRatio / screenRation;
                cameraSize = targetHeight * 0.5f * differenceInSize;
            }
            
            if (cameraSize > maxSize)
            {
                var currentHeight = 2f * maxSize;
                var currentWidth = screenRation * currentHeight;
                
                var lastPoint = points[points.Count - 1];
               
                var lastPointWorld = tileMatrix.GetCellCenterWorld(lastPoint);

                

                var currentMinX = centerXAxis - (currentWidth * 0.5f) + (widthOffset);
                var currentMaxX = centerXAxis + (currentWidth * 0.5f) - (widthOffset);

                var currentMinY = centerYAxis - (currentHeight * 0.5f) + (heightOffset);
                var currentMaxY = centerYAxis + (currentHeight * 0.5f) - (heightOffset);

                var offsetX = 0f;
                var offsetY = 0f;

                if (lastPointWorld.x < currentMinX)
                {
                    offsetX = (lastPointWorld.x - currentMinX) - (widthOffset);
                }
                else
                if (lastPointWorld.x > currentMaxX)
                {
                    offsetX = (lastPointWorld.x - currentMaxX) + (widthOffset);
                }

                if (lastPointWorld.y < currentMinY)
                {
                    offsetY = (lastPointWorld.y - currentMinY) - (heightOffset);
                }
                else
                if (lastPointWorld.y > currentMaxY)
                {                    
                    offsetY = (lastPointWorld.y - currentMaxY) + (heightOffset);
                }

                centerXAxis += offsetX;
                centerYAxis += offsetY;
               
                center = tileMatrix.WorldToCell(new Vector3(centerXAxis, centerYAxis, 0f));
                centerX = (int)center.x;
                centerY = (int)center.y;
            }
            
            return MoveTo(new Vector2Int(centerX, centerY), time, cameraSize);
        }

        private float getClampX(float targetX)
        {
            return Mathf.Clamp(targetX, Bounds.min.x + CameraBounds.size.x / 2, Bounds.max.x - CameraBounds.size.x / 2);
        }

        private float getClampY(float targetY)
        {
            return Mathf.Clamp(targetY, Bounds.min.y + CameraBounds.size.y / 2, Bounds.max.y - CameraBounds.size.y / 2);
        }

        private void finishDrag()
        {
            IsDrag = false;
            isDown = false;
            speed.x = 0;
            speed.y = 0;
            previousVelocity.Clear();
            KillTween();
        }

        protected void OnMouseDown()
        {
            KillTween();
            
            CameraBounds = OrthographicBounds();
            speed.x = 0;
            speed.y = 0;
            downPoint = GetWorldPosition();
            isDown = true;
            IsDrag = false;

            if (!IsDragAvailable) return;
        }

        protected void OnMouseUp()
        {
            if (!IsDragAvailable) return;
            if (!IsDrag) return;

            KillTween();
            isDown = false;
            if (IsZoom)
                IsZoom = false;
            wasMagnitude = 0;

            Vector2 sum;
            int velocityCount;
            float totalWeight;
            int i;
            float weight;

            sum.x = speed.x * CURRENT_VELOCITY_WEIGHT;
            sum.y = speed.y * CURRENT_VELOCITY_WEIGHT;
            velocityCount = previousVelocity.Count;
            totalWeight = CURRENT_VELOCITY_WEIGHT;
            for (i = 0; i < velocityCount; i++)
            {
                weight = VELOCITY_WEIGHTS[i];
                sum.x += previousVelocity[0].x * weight;
                sum.y += previousVelocity[0].y * weight;
                previousVelocity.RemoveAt(0);
                totalWeight += weight;
            }

            speed.x = sum.x / totalWeight;
            speed.y = sum.y / totalWeight;

            if (Time.realtimeSinceStartup - lastMoveTime > 0.05)
            {
                speed.x = 0;
                speed.y = 0;
            }

            previousVelocity.Clear();
        }


        private float lastMoveTime;

        protected void OnMouseMove()
        {
            if (!IsDragAvailable || needSkipDrag) return;

            if (!IsDrag && isDown)
            {
                var curPoint = Input.mousePosition;
                var downPointScreen = camera.WorldToScreenPoint(downPoint);

                //Драгать только в том случае, если прошли определённое расстояние, в зависимости от dpi.
                if (Vector2.Distance(curPoint, downPointScreen) > Screen.dpi / 12)
                {
                    downPoint = GetWorldPosition();
                    if (isDown) IsDrag = true;
                }
            }

            if (!IsDrag) return;

            var newPosition = GetWorldPosition();
            //Если позиция мыши за пределами камеры, то не двигаемся
            Vector3 cameraExtents = CameraBounds.extents;
            Vector3 cameraPosition = camera.transform.position;
            if (newPosition.x < cameraPosition.x - cameraExtents.x || newPosition.x > cameraPosition.x + cameraExtents.x ||
                newPosition.y < cameraPosition.y - cameraExtents.y || newPosition.y > cameraPosition.y + cameraExtents.y)
                return;

            var moveVector = downPoint - newPosition;

            if (moveVector.Equals(Vector2.zero))
                return;

            if (isDown)
            {
                lastMoveTime = Time.realtimeSinceStartup;

                previousVelocity.Add(moveVector);
                if (previousVelocity.Count > MAXIMUM_SAVED_VELOCITY_COUNT)
                    previousVelocity.RemoveAt(0);
            }

            SetCameraPosition(new Vector3(camera.gameObject.transform.position.x + moveVector.x,
                                          camera.gameObject.transform.position.y + moveVector.y,
                                            camera.gameObject.transform.position.z));
            ClampCameraPosition();

            targetX = camera.gameObject.transform.position.x;
            targetY = camera.gameObject.transform.position.y;
        }

        private void ClampCameraPosition()
        {
            if (CameraBounds.Equals(default))
                CameraBounds = OrthographicBounds();
            
            var vect = camera.gameObject.transform.position;
            vect.x = Mathf.Clamp(vect.x, Bounds.min.x + CameraBounds.size.x / 2, Bounds.max.x - CameraBounds.size.x / 2);
            vect.y = Mathf.Clamp(vect.y, Bounds.min.y + CameraBounds.size.y / 2, Bounds.max.y - CameraBounds.size.y / 2);
            SetCameraPosition(vect);
        }

        private Bounds OrthographicBounds()
        {
            var t = camera.transform;
            var x = t.position.x;
            var y = t.position.y;
            var size = camera.orthographicSize * 2;
            var width = size * (float)Screen.width / Screen.height;
            var height = size;

            return new Bounds(new Vector3(x, y, 0), new Vector3(width, height, 0));
        }

        private Vector2 GetWorldPositionOfFinger(int FingerIndex)
        {
            return camera.ScreenToWorldPoint(Input.GetTouch(FingerIndex).position);
        }

        private Vector2 GetWorldPosition()
        {
            return camera.ScreenToWorldPoint(Input.mousePosition);
        }

        public bool IsSignOfCamera(Vector2Int point)
        {
            var worldPoint = tileMatrix.GetCellCenterWorld(point);
            return CameraBounds.Contains(worldPoint);
        }
    }
}