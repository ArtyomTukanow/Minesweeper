using System;
using Assets.Scripts.UI.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace View.UI
{
    public class SingleSwiper : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField][Range(0.0f, 1.0f)] private float _sensitivity;
        [SerializeField][Range(0.0f, 1.0f)] private float _elastic = 0.7f;
        
        [SerializeField] private RectTransform _content;
        [SerializeField] private CanvasGroup _contentCanvasGroup;

        /// <summary>
        /// Открепляем дистанцию для начала свайпа от разрешения экрана
        /// </summary>
        private float ScaledDistanceToStart => (1f - _sensitivity) * Screen.width;

        private Vector2 _startPosition;

        public Func<bool> CanSwipeToLeft;
        public Func<bool> CanSwipeToRight;

        public Button leftArrow;
        public Button rightArrow;
        
        public Action OnSwipeToLeft;
        public Action OnSwipeToRight;

        private const float ANIMATION_RETURN_TIME = .25f;
        private const float ANIMATION_LEAVE_TIME = .25f;

        private const float ANIMATION_APPEAR_NEXT_TIME = .5f;

        private Tween _moveTween;
        private Tween _alphaTween;
        private Tween _nextTween;

        public bool IsAnimationInProgress => 
            _moveTween?.active == true ||
            _nextTween?.active == true ||
            _alphaTween?.active == true;

        public bool IsDragging { get; private set; }

        private void OnFail()
        {
            KillTweens();

            _moveTween = _content.DOAnchorPos(_startPosition, ANIMATION_RETURN_TIME).SetEase(Ease.InOutQuad);
            _alphaTween = _contentCanvasGroup.DOFade(1, ANIMATION_RETURN_TIME);
            _nextTween = DOVirtual.DelayedCall(ANIMATION_RETURN_TIME, KillTweens);
        }

        private void Start()
        {
            CheckStartPosition();
            
            leftArrow.onClick.RemoveAllListeners();
            leftArrow.onClick.AddListener(() => MoveToSide(false));
            
            rightArrow.onClick.RemoveAllListeners();
            rightArrow.onClick.AddListener(() => MoveToSide(true));
        }

        private void CheckStartPosition()
        {
            if (_content)
                _startPosition = _content.anchoredPosition;
        }

        private float DistanceCalculate(float x) => x > 0 ? Mathf.Pow(x, _elastic) : -Mathf.Pow(-x, _elastic);

        public void MoveToSide(bool toLeft)
        {
            KillTweens();

            var fromPos = DistanceCalculate(toLeft ? Screen.width / 2 : -Screen.width / 2);
            var toPos = DistanceCalculate(toLeft ? -Screen.width / 2 : Screen.width / 2);
            
            _moveTween = _content.DOAnchorPosX(toPos, ANIMATION_LEAVE_TIME);
            _alphaTween = _contentCanvasGroup.DOFade(0, ANIMATION_LEAVE_TIME);

            _nextTween = DOVirtual.DelayedCall(ANIMATION_LEAVE_TIME, () =>
            {
                if (toLeft)
                    OnSwipeToLeft?.Invoke();
                else
                    OnSwipeToRight?.Invoke();

                _content.anchoredPosition = _content.anchoredPosition.Set(fromPos);

                _moveTween = _content.DOAnchorPos(_startPosition, ANIMATION_APPEAR_NEXT_TIME);
                _alphaTween = _contentCanvasGroup.DOFade(1, ANIMATION_APPEAR_NEXT_TIME);
            });
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!IsDragging || IsAnimationInProgress) return;

            var distX = eventData.position.x - eventData.pressPosition.x;
            
            _content.anchoredPosition = _content.anchoredPosition.Set(DistanceCalculate(distX));
            
            if (!ScaledDistanceToStart.CloseTo(0)) 
                _contentCanvasGroup.alpha = Mathf.Clamp(1 - 2 * Mathf.Abs(distX) / Mathf.Abs(ScaledDistanceToStart), .5f, 1); 
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!IsAnimationInProgress) 
                IsDragging = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!IsDragging || IsAnimationInProgress) return;
            
            IsDragging = false;

            var distX = eventData.position.x - eventData.pressPosition.x;
            var swipeToLeft = distX < 0;
            
            var canSwipe = swipeToLeft ?
                CanSwipeToLeft?.Invoke() != false :
                CanSwipeToRight?.Invoke() != false;

            if (Mathf.Abs(distX) > Mathf.Abs(ScaledDistanceToStart) && canSwipe)
            {
                MoveToSide(swipeToLeft);
                return;
            }

            OnFail();
        }

        private void OnDisable()
        {
            KillTweens();
        }


        private void OnDestroy()
        {
            KillTweens();
        }

        private void KillTweens()
        {
            _nextTween?.Kill();
            _moveTween?.Kill();
            _alphaTween?.Kill();

            _nextTween = null;
            _moveTween = null;
            _alphaTween = null;
        }
    }
}