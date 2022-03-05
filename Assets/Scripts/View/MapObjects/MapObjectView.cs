using AllUtils;
using Controller;
using Core;
using Core.AssetsManager;
using DG.Tweening;
using JetBrains.Annotations;
using Libraries.RSG;
using ModelData.TileMap;
using UnityEngine;
using UserData.TileMap;
using View.Map;

namespace View.Tile
{
    public class MapObjectView : MonoBehaviour
    {
        private enum State
        {
            Closed,
            Flagged,
            Opened,
            Bombed,
            None,
        }
        
        [SerializeField] private SpriteRenderer topLeft;
        [SerializeField] private SpriteRenderer topRight;
        [SerializeField] private SpriteRenderer bottomLeft;
        [SerializeField] private SpriteRenderer bottomRight;
        
        [SerializeField] private SpriteRenderer count;
        [SerializeField] private Transform flag;
        [SerializeField] private Transform error;
        [SerializeField] private Transform bomb;

        private bool lastNeedShow;
        private bool lastForceWhiteText;
        private string lastPrefabPath;

        public Vector2Int Position { get; private set; }
        
        public MapView Map { get; private set; }
        public UserMap UserMap => Map.UserMap;
        
        [CanBeNull]
        public UserTile UserTile { get; private set; }

        private State state = State.None;

        public void Init(MapView map)
        {
            Map = map;
        }

        public void Init(MapView map, UserTile userTile, Vector2Int pos)
        {
            Map = map;
            UserTile = userTile;
            Position = pos;
            UpdatePosition();
            TryUpdateState();
        }

        public void UpdateUserData(UserTile userTile)
        {
            UserTile = userTile;
            TryUpdateState();
        }
        
        public void SetPosition(Vector2Int pos)
        {
            if (Position == default || Position != pos)
            {
                Position = pos;
                UpdatePosition();
            }
            TryUpdateState();
        }

        private void UpdatePosition()
        {
            transform.position = Map.tileMatrix.GetCellCenterWorld(Position);
        }

        public void UpdateContent()
        {
            UpdateSprites();
            UpdateMaterials();
            UpdateText();
            UpdateError();
            OnMapStateUpdated(lastNeedShow, lastForceWhiteText);
        }

        
        public void OnMapStateUpdated(bool needShow, bool forceWhiteText)
        {
            lastNeedShow = needShow;
            lastForceWhiteText = forceWhiteText;
            
            count.enabled = needShow;

            if (UserTile != null)
            {
                string path = (Game.User.Settings.BlackPalette || forceWhiteText)
                    ? $"img/tiles/text/white/{UserTile.BombsNear}"
                    : $"img/tiles/text/black/{UserTile.BombsNear}";

                SetCountPrefab(path);
            }
            
            UpdateError();
        }
        
        private void SetCountPrefab(string path)
        {
            if (lastPrefabPath == path)
                return;

            lastPrefabPath = path;
            count.sprite = AssetsLoader.CreateSync<Sprite>(path);
        }

        private void UpdateError()
        {
            error.gameObject.SetActive(UserMap.State == UserMapState.GameOver && UserTile != null && UserTile.Flagged && !UserTile.Bomb);
        }

        private void UpdateText()
        {
            if (UserTile == null)
            {
                count.gameObject.SetActive(false);
                flag.gameObject.SetActive(false);
                bomb.gameObject.SetActive(false);
                return;
            }

            flag.gameObject.SetActive(UserTile.IsFlagged);
            bomb.gameObject.SetActive(UserTile.IsOpened && UserTile.Bomb);
            count.gameObject.SetActive(UserTile.IsOpened && !UserTile.Bomb);
        }

        private void UpdateMaterials()
        {
            UpdateTileMaterial(topLeft);
            UpdateTileMaterial(topRight);
            UpdateTileMaterial(bottomLeft);
            UpdateTileMaterial(bottomRight);
        }

        private void UpdateSprites()
        {
            UpdateSprite(topLeft, new Vector2Int(-1, 1));
            UpdateSprite(topRight, new Vector2Int(1, 1));
            UpdateSprite(bottomLeft, new Vector2Int(-1, -1));
            UpdateSprite(bottomRight, new Vector2Int(1, -1));
        }

        private void UpdateSprite(SpriteRenderer sprite, Vector2Int direction)
        {
            if (UserTile?.IsOpened == true)
            {
                SetSpriteAsOuterCorner(sprite);
                UpdateRotation(sprite, direction, isCorner : true);
                return;
            }
            
            var nearPosX = new Vector2Int(Position.x + direction.x, Position.y);
            var nearPosY = new Vector2Int(Position.x, Position.y + direction.y);
            var nearPosXY = Position + direction;

            var isFlag = UserTile?.IsFlagged == true;
            bool xOpened, yOpened, xyOpened;

            if (isFlag)
            {
                xOpened = UserMap[nearPosX]?.IsFlagged == false;
                yOpened = UserMap[nearPosY]?.IsFlagged == false;
                xyOpened = UserMap[nearPosXY]?.IsFlagged == false;
            }
            else
            {
                xOpened = UserMap[nearPosX]?.IsOpened == true || UserMap[nearPosX]?.IsFlagged == true;
                yOpened = UserMap[nearPosY]?.IsOpened == true || UserMap[nearPosY]?.IsFlagged == true;
                xyOpened = UserMap[nearPosXY]?.IsOpened == true || UserMap[nearPosXY]?.IsFlagged == true;
            }

            var isOuterCorner = xOpened && yOpened;
            var isInnerCorner = !xOpened && !yOpened && xyOpened;
            var isCorner = isOuterCorner || isInnerCorner;

            if (isOuterCorner)
                SetSpriteAsOuterCorner(sprite);
            else if (isInnerCorner)
                SetSpriteAsInnerCorner(sprite);
            else if (yOpened)
                SetSpriteAsHorizontalBorder(sprite);
            else if (xOpened)
                SetSpriteAsVerticalBorder(sprite);
            else
                SetSpriteAsFull(sprite);

            UpdateRotation(sprite, direction, isCorner, xOpened, yOpened);
        }

        private IPromise AnimateSpritesToColor(Color toColor, Material toMaterial)
        {
            var tempMaterial = new Material(BasePrefabs.Instance.closeTileMaterial);

            topLeft.material = topRight.material = bottomLeft.material = bottomRight.material = tempMaterial;
            
            return tempMaterial.DOColor(toColor, 0.2f)
                    .ToPromise(true)
                    .Then(() =>
                    {
                        if(!this)
                            return;
                        
                        topLeft.material = topRight.material = bottomLeft.material = bottomRight.material = toMaterial;
                    });
        }

        private void SetSpritesAsCorner()
        {
            SetSpriteAsOuterCorner(topLeft);
            SetSpriteAsOuterCorner(topRight);
            SetSpriteAsOuterCorner(bottomLeft);
            SetSpriteAsOuterCorner(bottomRight);
            
            UpdateRotation(topLeft, new Vector2Int(-1, 1), isCorner: true);
            UpdateRotation(topRight, new Vector2Int(1, 1), isCorner: true);
            UpdateRotation(bottomLeft, new Vector2Int(-1, -1), isCorner: true);
            UpdateRotation(bottomRight, new Vector2Int(1, -1), isCorner: true);
        }

        private void UpdateTileMaterial(SpriteRenderer sprite)
        {
            if (UserTile == null)
                sprite.material = BasePrefabs.Instance.closeTileMaterial;
            else if (UserTile.IsOpened)
                sprite.material = BasePrefabs.Instance.openTileMaterial;
            else if (UserTile.IsFlagged)
                sprite.material = BasePrefabs.Instance.flagTileMaterial;
            else
                sprite.material = BasePrefabs.Instance.closeTileMaterial;
        }

        private static void UpdateRotation(SpriteRenderer sprite, Vector2Int direction, bool isCorner = false, bool isVertical = false, bool isHorizontal = false)
        {
            float dec = 0f;

            if (isCorner)
            {
                if (direction == new Vector2Int(1, 1))
                    dec = 0f;
                else if (direction == new Vector2Int(1, -1))
                    dec = -90f;
                else if (direction == new Vector2Int(-1, 1))
                    dec = 90f;
                else if (direction == new Vector2Int(-1, -1))
                    dec = 180f;
            }
            else if (isVertical && direction.x == -1)
                dec = 180f;
            else if (isHorizontal && direction.y == -1)
                dec = 180f;
            
            sprite.transform.rotation = Quaternion.Euler(0, 0, dec);
        }
        
        private static void SetSpriteAsOuterCorner(SpriteRenderer sprite) => sprite.sprite = BasePrefabs.Instance.tileCornerOuter;
        private static void SetSpriteAsInnerCorner(SpriteRenderer sprite) => sprite.sprite = BasePrefabs.Instance.tileCornerInner;
        private static void SetSpriteAsHorizontalBorder(SpriteRenderer sprite) => sprite.sprite = BasePrefabs.Instance.tileBorderHorizontal;
        private static void SetSpriteAsVerticalBorder(SpriteRenderer sprite) => sprite.sprite = BasePrefabs.Instance.tileBorderVertical;
        private static void SetSpriteAsFull(SpriteRenderer sprite) => sprite.sprite = BasePrefabs.Instance.tileFull;





        public void OnClick()
        {
            UserMap.CommandSystem.OnClickTile(Position);
        }

        public void OnLongClick()
        {
            UserMap.CommandSystem.OnLongClick(Position);
        }



        private bool lockUpdateState = false;
        public void TryUpdateState()
        {
            if(lockUpdateState)
                return;
            
            var newState = StateCalculate();
            if (state != newState)
            {
                if (state == State.None)
                {
                    state = newState;
                    UpdateContent();
                    return;
                }
                state = newState;
                lockUpdateState = true;
                ChangeState(state)
                    .Then(() =>
                    {
                        lockUpdateState = false;
                        UpdateContent();
                        TryUpdateState();
                    });
            }
            else
                UpdateContent();
        }

        private State StateCalculate()
        {
            if (UserTile == null)
                return State.Closed;
            
            if (UserTile.Opened && UserTile.Bomb)
                return State.Bombed;

            if (UserTile.IsFlagged)
                return State.Flagged;

            if (UserTile.IsOpened)
                return State.Opened;
            
            return State.Closed;
        }

        private IPromise ChangeState(State newState)
        {
            if (newState == State.Flagged)
                return PickFlagAnim();
            if (newState == State.Bombed)
                return DropBombAnim();
            if (newState == State.Opened)
                return OpenAnim();

            return Promise.Resolved();
        }

        private IPromise PickFlagAnim()
        {
            UpdateSprites();
            var color = ColorSystem.GetFlaggedColor(UserMap.Data.Hue);
            var colorPromise = AnimateSpritesToColor(color, BasePrefabs.Instance.flagTileMaterial);
            flag.gameObject.SetActive(true);
            var scalePromise = flag.DOScale(Vector3.one / 4, 0.2f)
                .ChangeStartValue(Vector3.zero)
                .SetEase(Ease.OutBack)
                .SetLink(flag.gameObject)
                .ToPromise(true);

            return Promise.All(colorPromise, scalePromise);
        }

        private IPromise DropBombAnim()
        {
            var colorPromise = AnimateSpritesToColor(BasePrefabs.Instance.openTileGameOverColor, BasePrefabs.Instance.openTileMaterial);
            bomb.gameObject.SetActive(true);
            var scalePromise = bomb.DOScale(Vector3.one / 4, 0.2f)
                .ChangeStartValue(Vector3.zero)
                .SetEase(Ease.OutBack)
                .SetLink(bomb.gameObject)
                .ToPromise(true);

            return Promise.All(colorPromise, scalePromise);
        }

        private IPromise OpenAnim()
        {
            SetSpritesAsCorner();
            return gameObject.transform
                .DOScale(Vector3.zero, 0.2f)
                .SetEase(Ease.InBack)
                .SetLink(gameObject)
                .ToPromise(true)
                .Then(() =>
                {
                    if(gameObject)
                        gameObject.transform.localScale = Vector3.one;
                });
        }
    }
}