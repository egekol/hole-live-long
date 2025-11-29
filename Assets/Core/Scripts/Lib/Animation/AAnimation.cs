using Cysharp.Threading.Tasks;
using DG.Tweening;
using Lib.CustomAttributes.Scripts;
using UnityEngine;

namespace Lib.Animation
{
    public abstract class AAnimation : MonoBehaviour
    {
        protected Tween Tween;
        
        [SerializeField] protected float Duration = 0.5f;
        
        [ProButton]
        public abstract UniTask Play();
        
        public virtual void Stop(bool complete = false)
        {
            if (Tween != null && Tween.IsActive())
            {
                Tween.Kill(complete);
            }
        }
    }
}