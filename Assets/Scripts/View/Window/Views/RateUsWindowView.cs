using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View.Window.Views
{
    public class RateUsWindowView : WindowView
    {
        [SerializeField] public TMP_Text title;
        
        [SerializeField] public TMP_Text yesText;
        [SerializeField] public TMP_Text noText;
        
        [SerializeField] public Button yesButton;
        [SerializeField] public Button noButton;
    }
}