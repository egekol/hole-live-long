using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Lib.Animation.PanelAnimations
{
    public class FadeOut : AAnimation
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        
        public override async UniTask Play()
        {
            await _canvasGroup.DoFade(0, Duration).ToUniTask();
            _canvasGroup.gameObject.SetActive(false);
        }
    }
}