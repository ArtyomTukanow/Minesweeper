using Assets.Scripts.UI.Utils;
using DG.Tweening;
using UnityEngine;

namespace View.UI
{
    public class TutorHand : MonoBehaviour
    {
        [SerializeField] private bool longClick;
        
        private void Awake()
        {
            StartTween();
        }

        private Vector3 startRotation;

        private void StartTween()
        {
            startRotation = transform.rotation.eulerAngles;

            var addZ = transform.localScale.x < 0 ? 15 : -15;
            
            var toRotation = startRotation.Add(z: addZ);


            DOTween.Sequence()
                .SetLink(gameObject)
                .Append(UpHand())
                .Append(DownHand())
                .Append(Delay())
                .SetLoops(-1, LoopType.Restart);

            Tween UpHand()
            {
                return transform
                    .DORotate(toRotation, 0.6f)
                    .SetLink(gameObject);
            }

            Tween DownHand()
            {
                return transform
                    .DORotate(startRotation, 0.1f)
                    .SetEase(Ease.Flash)
                    .SetLink(gameObject);
            }

            Tween Delay()
            {
                return DOTween.Sequence().SetDelay(longClick ? 0.6f : 0.3f);
            }
        }
    }
}