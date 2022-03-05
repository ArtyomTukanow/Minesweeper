using System;
using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View.Window.Views
{
    public class LocalizeButtonView : MonoBehaviour
    {
        [SerializeField] public Button Button;
        [SerializeField] public TMP_Text ButtonText;
        [SerializeField] public Transform Galka;
        
        [SerializeField] public SystemLanguage Locale;

        public void Init(Action<SystemLanguage> onButtonClick)
        {
            Button.onClick.RemoveAllListeners();
            Button.onClick.AddListener(() => onButtonClick?.Invoke(Locale));

            Redraw();
        }

        public void Redraw()
        {
            ButtonText.text = Game.Static.Localization.GetLocale(Locale, Locale.ToString());
            Galka.gameObject.SetActive(Game.User.Settings.Language == Locale);
        }
    }
}