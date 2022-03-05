using System;
using AllUtils;
using TMPro;
using UnityEngine;
using UserData.Rating;

namespace View.Window.Views
{
    public class RatingPlaceView : MonoBehaviour
    {
        public TMP_Text place;
        public TMP_Text date;
        public TMP_Text value;
        public Transform selected;

        public void SetEmptyData(int placeNumber)
        {
            place.text = placeNumber + ".";
            date.text = "";
            value.text = "";
            SetSelected(false);
        }
        
        public void SetData(UserRatingPlace userPlace)
        {
            place.text = userPlace.Place + ".";
            date.text = new DateTime(userPlace.Date).ToString("d");
            value.text = userPlace.Time.GetCharTime(2);
        }

        public void SetSelected(bool val)
        {
            if(selected)
                selected.gameObject.SetActive(val);
        }
    }
}