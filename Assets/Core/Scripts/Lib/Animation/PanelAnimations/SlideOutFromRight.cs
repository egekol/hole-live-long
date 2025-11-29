using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Lib.Animation.PanelAnimations
{
    public class SlideOutFromRight : AAnimation
    {
        [SerializeField] private RectTransform _rectParent;
        
        private Vector2 ChoiceDisabledPos =>
            Vector2.right * _rectParent.rect.width * 1.5f + Vector2.up * _rectParent.anchoredPosition;        
        private Vector2 ChoiceEnabledPos => Vector2.zero + Vector2.up * _rectParent.anchoredPosition;
        
        public override async UniTask Play()
        {
            _rectParent.DOKill();
            _rectParent.anchoredPosition = ChoiceEnabledPos;
            await _rectParent.DoAnchoredPos(ChoiceDisabledPos, Duration).SetEase(Ease.InCubic).ToUniTask();
        }
        
        public void SetPosition()
        {
            _rectParent.anchoredPosition = ChoiceDisabledPos;
        }
    }
}