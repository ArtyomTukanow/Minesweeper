using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View.Window.Views
{
    public class InfoView : WindowView
    {
        [SerializeField] public TMP_Text title;
        [SerializeField] public TMP_Text description;
        
        [SerializeField] public TMP_Text okText;
        [SerializeField] public TMP_Text yesText;
        [SerializeField] public TMP_Text noText;
        
        [SerializeField] public Button okButton;
        [SerializeField] public Button yesButton;
        [SerializeField] public Button noButton;
    }
}