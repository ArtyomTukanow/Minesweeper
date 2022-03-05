using Core;
using UnityEngine;
using View;

namespace Controller
{
    public class ColorSystem
    {
        public const float BLACK_CLOSED_TILE_SATURATION = 0.25f;
        public const float BLACK_CLOSED_TILE_BRIGHTNESS = 0.75f;
        
        public const float BLACK_OPENED_TILE_SATURATION = 0.1f;
        public const float BLACK_OPENED_TILE_BRIGHTNESS = 0.3f;
        
        public const float BLACK_FLAG_TILE_SATURATION = 0.1f;
        public const float BLACK_FLAG_TILE_BRIGHTNESS = 0.5f;
        
        public const float BLACK_BACK_SATURATION = 0.1f;
        public const float BLACK_BACK_BRIGHTNESS = 0.3f;
        
        public const float WHITE_CLOSED_TILE_SATURATION = 0.58f;
        public const float WHITE_CLOSED_TILE_BRIGHTNESS = 0.43f;
        
        public const float WHITE_OPENED_TILE_SATURATION = 0.05f;
        public const float WHITE_OPENED_TILE_BRIGHTNESS = 0.84f;
        
        public const float WHITE_FLAG_TILE_SATURATION = 0.12f;
        public const float WHITE_FLAG_TILE_BRIGHTNESS = 0.68f;
        
        public const float WHITE_BACK_SATURATION = 0.05f;
        public const float WHITE_BACK_BRIGHTNESS = 0.84f;


        public static Color GetClosedColor(float hue)
        {
            var s = Game.User.Settings.BlackPalette ? BLACK_CLOSED_TILE_SATURATION : WHITE_CLOSED_TILE_SATURATION;
            var b = Game.User.Settings.BlackPalette ? BLACK_CLOSED_TILE_BRIGHTNESS : WHITE_CLOSED_TILE_BRIGHTNESS;

            // if (MapController.Instance?.UserMap.State == UserMapState.Pause)
                // s /= 2f;
            
            return Color.HSVToRGB(hue, s, b);
        }

        public static Color GetOpenedColor(float hue)
        {
            var s = Game.User.Settings.BlackPalette ? BLACK_OPENED_TILE_SATURATION : WHITE_OPENED_TILE_SATURATION;
            var b = Game.User.Settings.BlackPalette ? BLACK_OPENED_TILE_BRIGHTNESS : WHITE_OPENED_TILE_BRIGHTNESS;
            
            // if (MapController.Instance?.UserMap.State == UserMapState.Pause)
                // s /= 2f;
            
            return Color.HSVToRGB(hue, s, b);
        }

        public static Color GetCellColor()
        {
            if(Game.User.Settings.BlackPalette)
                return BasePrefabs.Instance.openCellBlackColor;
            return BasePrefabs.Instance.openCellWhiteColor;
        }

        public static Color GetFlaggedColor(float hue)
        {
            var s = Game.User.Settings.BlackPalette ? BLACK_FLAG_TILE_SATURATION : WHITE_FLAG_TILE_SATURATION;
            var b = Game.User.Settings.BlackPalette ? BLACK_FLAG_TILE_BRIGHTNESS : WHITE_FLAG_TILE_BRIGHTNESS;
            
            // if (MapController.Instance?.UserMap.State == UserMapState.Pause)
                // s /= 2f;
            
            return Color.HSVToRGB(hue, s, b);
        }

        public static Color GetBackColor(float hue)
        {
            var s = Game.User.Settings.BlackPalette ? BLACK_BACK_SATURATION : WHITE_BACK_SATURATION;
            var b = Game.User.Settings.BlackPalette ? BLACK_BACK_BRIGHTNESS : WHITE_BACK_BRIGHTNESS;
            
            // if (MapController.Instance?.UserMap.State == UserMapState.Pause)
                // s /= 2f;
            
            return Color.HSVToRGB(hue, s, b);
        }
    }
}