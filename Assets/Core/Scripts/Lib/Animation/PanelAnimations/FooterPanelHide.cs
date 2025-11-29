using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Lib.Animation.PanelAnimations
{
    public class FooterPanelHide : AAnimation
    {
        [SerializeField] private RectTransform _rectParent;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Ease _ease = Ease.InCubic;

        public override async UniTask Play()
        {
            _rectParent.DOKill();
            var screenHeight = Screen.height;
            _rectParent.localPosition = Vector3.zero;
            _canvasGroup.alpha = 1f;
            _=_canvasGroup.DoFade(0f, Duration).SetEase(_ease);
            await _rectParent.DOLocalMoveY(-(screenHeight / 2), Duration).SetEase(_ease).ToUniTask();
            _rectParent.gameObject.SetActive(false);
        }


#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponentInChildren<CanvasGroup>();
            }
        }
        
#endif
    }
}