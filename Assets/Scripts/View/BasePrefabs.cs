using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using View.HUD;

namespace View
{
    [CreateAssetMenu(fileName = "BasePrefab", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
    public class BasePrefabs : ScriptableObject
    {
        public static BasePrefabs Instance { get; private set; }
        
        [SerializeField] public HudContent content;
        
        [SerializeField] public Material openTileMaterial;
        [SerializeField] public Material closeTileMaterial;
        [SerializeField] public Material flagTileMaterial;
        
        [SerializeField] public Color openCellBlackColor;
        [SerializeField] public Color openCellWhiteColor;
        
        [SerializeField] public Color openTileGameOverColor;
        [SerializeField] public Color closeTileGameOverColor;
        [SerializeField] public Color flagTileGameOverColor;
        [SerializeField] public Color backgroundGameOverColor;
        
        [SerializeField] public Color openTileCompleteColor;
        [SerializeField] public Color closeTileCompleteColor;
        [SerializeField] public Color flagTileCompleteColor;
        [SerializeField] public Color backgroundCompleteColor;
        
        [SerializeField] public Sprite tileCornerInner;
        [SerializeField] public Sprite tileCornerOuter;
        [SerializeField] public Sprite tileBorderHorizontal;
        [SerializeField] public Sprite tileBorderVertical;
        [SerializeField] public Sprite tileFull;
        
        [SerializeField] public TileBase tileNone;
        
        [SerializeField] public TMP_FontAsset whiteFontAsset;
        [SerializeField] public TMP_FontAsset blackFontAsset;
        
        private void OnEnable() => Instance = this;
    }
}