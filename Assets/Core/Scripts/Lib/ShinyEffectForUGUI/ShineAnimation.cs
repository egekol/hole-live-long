using DG.Tweening;
using UnityEngine;

namespace Lib.ShinyEffectForUGUI
{
    [RequireComponent(typeof(ShinyEffectForUGUI))]
    public class ShineAnimation : MonoBehaviour
    {
        [SerializeField] private float _duration = 1f;
        [SerializeField] private float _delay = 1f;
        [SerializeField] private float _interval = 2.5f;
   
        private ShinyEffectForUGUI _effect;
        private Sequence _seq;

        public void SetState(bool state)
        {
            if (state)
            {
                enabled = true;
            }
            else
            {
                _seq?.Kill();
                enabled = false;
            
                if (_effect != null)
                {
                    _effect.location = 0f;
                }
            }
        }
    
        private void Awake()
        {
            _effect = GetComponent<ShinyEffectForUGUI>();
        }
    
        private void OnEnable()
        {
            _effect.location = 0f;
            _seq?.Kill();
            _seq = DOTween.Sequence();
            _seq.Append(DOVirtual.Float(0, 1f, _duration, value => _effect.location = value).SetEase(Ease.Linear));
            _seq.AppendInterval(_interval);
            _seq.SetLoops(-1).SetDelay(_delay, false);
        }

        private void OnDisable()
        {
            _seq?.Kill(true);
        }
    }
}
