using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Lib.Animation
{
    public class SetEnable : AAnimation
    {
        [SerializeField] private bool _isEnable;
        
        public override UniTask Play()
        {
            gameObject.SetActive(_isEnable);
            return UniTask.CompletedTask;
        }
    }
}