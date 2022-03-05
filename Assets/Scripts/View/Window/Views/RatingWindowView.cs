using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using View.UI;

namespace View.Window.Views
{
    public class RatingWindowView : WindowView
    {
        public TMP_Text typeText;
        public SingleSwiper typeSwiper;
        public RatingPlaceView placePrefab;
        public Transform placesParent;
        public TMP_Text okText;
        [CanBeNull]
        public TMP_Text emptyText;
        [CanBeNull]
        public TMP_Text perfectlyText;
        [CanBeNull]
        public TMP_Text ratingTitleText;
        public int minPlaces = 0;
    }
}