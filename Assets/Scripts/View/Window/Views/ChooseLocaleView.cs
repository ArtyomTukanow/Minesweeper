using TMPro;
using UnityEngine;

namespace View.Window.Views
{
    public class ChooseLocaleView : WindowView
    {
        [SerializeField] public LocalizeButtonView EngButton;
        [SerializeField] public LocalizeButtonView RusButton;
        
        [SerializeField] public TMP_Text Title;
    }
}