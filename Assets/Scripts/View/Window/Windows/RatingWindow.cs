using System.Collections.Generic;
using AllUtils;
using Assets.Scripts.UI.Utils;
using Core;
using ModelData.TileMap;
using UnityEngine;
using UserData.Rating;
using Utils;
using View.Window.Settings;
using View.Window.Views;

namespace View.Window.Windows
{
    public class RatingWindow : Window<RatingWindowView>
    {
        public override string PathOfView => isShort ? "Windows/ShortRatingWindow" : "Windows/RatingWindow";
        
        private List<RatingPlaceView> placesPrefabs = new List<RatingPlaceView>();

        private bool isShort;
        private MapDataType mapType;
        private UserRatingByType ratingByType;
        private UserRatingPlace selectedPlace;

        public RatingWindow(WindowSettings settings) : base(settings)
        {
        }
        
        public static RatingWindow Of(MapDataType mapType = MapDataType.beginner, bool isShort = false, UserRatingPlace selectedPlace = null)
        {
            var settings = new WindowSettings.Builder
            {
                Priory = 150,
                ShowInQueue = false
            }.Build();
            
            var wnd = new RatingWindow(settings);
            wnd.mapType = mapType;
            wnd.isShort = isShort;
            wnd.selectedPlace = selectedPlace;
            WindowsController.Instance.AddWindow(wnd);

            return wnd;
        }

        protected override void OnShow()
        {
            base.OnShow();

            if (View.perfectlyText != null)
                View.perfectlyText.text = "perfectly".Localize();

            if (View.emptyText != null)
                View.emptyText.text = "empty_rating".Localize();

            if (View.ratingTitleText != null)
                View.ratingTitleText.text = "rating".Localize();
            
            View.okText.text = "ok".Localize();

            AddListeners();
            DrawByType(mapType);
        }

        private void AddListeners()
        {
            if (View.typeSwiper)
            {
                View.typeSwiper.OnSwipeToLeft += () => DrawByType(mapType.NextTimer());
                View.typeSwiper.OnSwipeToRight += () => DrawByType(mapType.PrevTimer());
            }
        }

        private void DrawByType(MapDataType mapType)
        {
            this.mapType = mapType;
            View.typeText.text = (isShort ? "difficult".Localize() + ": " : "") + this.mapType.ToString().Localize();
            ratingByType = Game.User.Rating.GetRatingByType(mapType);
            CreateOrHidePrefabs();

            for (var i = 0; i < ratingByType.Places.Count; i++)
            {
                placesPrefabs[i].SetData(ratingByType.Places[i]);
                var isSelected = ratingByType.Places[i] == selectedPlace;
                placesPrefabs[i].SetSelected(isSelected);
                if (isSelected)
                {
                    var place = placesPrefabs[i];
                    TimerUtils.NextFrame().Then(() => MoveToPlace(place));
                }
            }

            for (var i = ratingByType.Places.Count; i < View.minPlaces; i++)
                placesPrefabs[i].SetEmptyData(i + 1);

            if (View.emptyText)
                View.emptyText.gameObject.SetActive(ratingByType.Places.Count == 0);
        }

        private void MoveToPlace(RatingPlaceView placeView)
        {
            if(!placeView || !(placeView.transform is RectTransform rectTransform))
                return;

            var newX = - rectTransform.rect.height / 2 - rectTransform.localPosition.y;
            
            placeView.transform.parent.localPosition = placeView.transform.parent.localPosition.Set(y: newX);
        }

        private void CreateOrHidePrefabs()
        {
            while (placesPrefabs.Count < ratingByType.Places.Count || placesPrefabs.Count < View.minPlaces)
                placesPrefabs.Add(Object.Instantiate(View.placePrefab, View.placesParent));

            for (var i = 0; i < placesPrefabs.Count; i++)
                placesPrefabs[i].gameObject.SetActive(i < ratingByType.Places.Count || i < View.minPlaces);
        }
    }
}