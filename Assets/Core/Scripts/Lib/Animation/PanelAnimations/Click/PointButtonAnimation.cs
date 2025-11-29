using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using Lib.Buttons;
using UnityEngine.UI;

namespace Lib.Animation.PanelAnimations.Click
{
    [RequireComponent(typeof(RaycastOnlyGraphic))]
    public class PointButtonAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] protected Button _button;
        [SerializeField] private Transform _animationParent;
        [SerializeField] private float _scaleDownAmount = 0.9f;
        [SerializeField] private float _animationDuration = 0.1f;

        private Vector3 _originalScale;
        private Tween _currentTween;

        protected void Awake()
        {

            if (_animationParent == null)
            {
                _animationParent = transform;
            }

            _originalScale = _animationParent.localScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsInteractable) return;

            _currentTween?.Kill();
            _animationParent.localScale = _originalScale * _scaleDownAmount;
        }

        public bool IsInteractable => _button.interactable;

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!IsInteractable) return;

            _currentTween?.Kill();
            _currentTween = _animationParent.DOScale(_originalScale, _animationDuration)
                .SetEase(Ease.OutBack);
        }


        protected void OnDestroy()
        {
            _currentTween?.Kill();
        }
    }
}