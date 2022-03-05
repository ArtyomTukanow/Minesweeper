using TMPro;
using UnityEngine;
using UnityEngine.UI;
using View.UI;

namespace View.Window.Views
{
    public class MainMenuView : WindowView
    {
        
        [SerializeField] public SingleSwiper swiper;
        [SerializeField] public TMP_Text targetTypeText;
        
        [SerializeField] public TMP_Text newGameText;
        [SerializeField] public TMP_Text continueText;
        [SerializeField] public TMP_Text unlockByVipText;
        [SerializeField] public TMP_Text lockerText;
        
        [SerializeField] public Button newGameButton;
        [SerializeField] public Button continueButton;
        [SerializeField] public Button unlockByVipButton;
        
        [SerializeField] public Image cover;
        
        [SerializeField] public Button paletteButton;
        [SerializeField] public Button storeButton;
        [SerializeField] public Button localizeButton;
        [SerializeField] public Button ratingButton;
        [SerializeField] public Button exitButton;
        
        [SerializeField] public Transform lockView;
    }
}