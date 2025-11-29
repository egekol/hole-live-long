using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Lib.Animation.PanelAnimations
{
    public class FadeIn : AAnimation
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        
        public override UniTask Play()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.gameObject.SetActive(true);
            return _canvasGroup.DoFade(1, Duration).ToUniTask();
        }
    }
}