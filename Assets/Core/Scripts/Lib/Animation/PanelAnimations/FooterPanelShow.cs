using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Lib.Animation.PanelAnimations
{
    public class FooterPanelShow : AAnimation
    {
        [SerializeField] private RectTransform _rectParent;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Ease _ease = Ease.OutCubic;

        public override UniTask Play()
        {
            _rectParent.DOKill();
            var screenHeight = Screen.height;
            _rectParent.anchoredPosition = Vector2.down * screenHeight/2;
            _rectParent.gameObject.SetActive(true);
            _canvasGroup.alpha = 0f;
            _canvasGroup.DoFade(1f, Duration).SetEase(_ease);
            return _rectParent.DoAnchoredPos(Vector2.zero, Duration).SetEase(_ease).ToUniTask();
            // return _rectParent.DOLocalMoveY(0, Duration).SetEase(_ease).ToUniTask();/
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