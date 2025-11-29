using Cysharp.Threading.Tasks;
using Lib.Debugger;
using UnityEngine;

namespace Lib.Animation.Animator
{
    public class AnimatorPlayer
    {
        private readonly UnityEngine.Animator _animator;
        private readonly IAnimationLibrary _animationInfos;
        private string _currentAnimation;
        private UniTask _transitionTask;
        private float _transitionDuration = 0.25f;

        public AnimatorPlayer(UnityEngine.Animator animator, IAnimationLibrary animationInfos)
        {
            _animator = animator;
            _animationInfos = animationInfos;
        }

        public void PlayAnimationByType(string animationType, bool crossFade = true)
        {
            if (string.IsNullOrEmpty(animationType))
            {
                LogErr("Animation type is null or empty!");
                return;
            }

            string animationName = _animationInfos.GetAnimationNameByType(animationType);

            if (string.IsNullOrEmpty(animationName))
            {
                LogErr($"Animation not found for type: {animationType}");
                return;
            }

            // if (_currentAnimation == animationName) //does this necessary
            // {
            //     Log($"Animation {animationName} is already playing");
            //     return;
            // }

            PlayAnimationByName(animationName, crossFade);
        }

        public void PlayAnimationByName(string animationName, bool crossFade = true)
        {
            if (crossFade)
            {
                _animator.CrossFade(animationName, _transitionDuration);
            }
            else
            {
                _animator.Play(animationName);
            }

            _currentAnimation = animationName;
            Log($"Playing animation: {animationName}");
        }

        public void TransitionToAnimation(string animationType, float transitionDuration = -1f)
        {
            if (string.IsNullOrEmpty(animationType))
            {
                return;
            }

            string animationName = _animationInfos.GetAnimationNameByType(animationType);

            if (string.IsNullOrEmpty(animationName))
            {
                return;
            }

            float duration = transitionDuration > 0 ? transitionDuration : _transitionDuration;

            _transitionTask = TransitionTask(animationName, duration);
        }

        public void StopAnimation()
        {
            _animator.Rebind();
            _animator.Update(0f);
            _currentAnimation = string.Empty;
        }

        public string GetCurrentAnimation()
        {
            return _currentAnimation;
        }

        public bool IsAnimationPlaying(string animationType)
        {
            string animationName = _animationInfos.GetAnimationNameByType(animationType);
            return _currentAnimation == animationName;
        }

        public void SetTransitionDuration(float duration)
        {
            _transitionDuration = Mathf.Max(0f, duration);
        }

        public void SetFloatParameter(string parameterName, float value)
        {
            _animator.SetFloat(parameterName, value);
        }

        public void SetIntParameter(string parameterName, int value)
        {
            _animator.SetInteger(parameterName, value);
        }

        public void SetBoolParameter(string parameterName, bool value)
        {
            _animator.SetBool(parameterName, value);
        }

        public void SetTriggerParameter(string parameterName)
        {
            _animator.SetTrigger(parameterName);
        }

        public void ResetTriggerParameter(string parameterName)
        {
            _animator.ResetTrigger(parameterName);
        }

        public float GetFloatParameter(string parameterName)
        {
            return _animator.GetFloat(parameterName);
        }

        public int GetIntParameter(string parameterName)
        {
            return _animator.GetInteger(parameterName);
        }

        public bool GetBoolParameter(string parameterName)
        {
            return _animator.GetBool(parameterName);
        }

        public void SetBlendTreeParameter(string blendTreeName, float blendValue)
        {
            SetFloatParameter(blendTreeName, blendValue);
        }

        public void SetBlendTree2D(string blendTreeName, float horizontalValue, float verticalValue)
        {
            SetFloatParameter($"{blendTreeName}_Horizontal", horizontalValue);
            SetFloatParameter($"{blendTreeName}_Vertical", verticalValue);
        }

        public void SetBlendTreeDirection(string blendTreeName, Vector2 direction)
        {
            SetBlendTree2D(blendTreeName, direction.x, direction.y);
        }

        public void SetBlendTreeSpeed(string blendTreeName, float speed)
        {
            SetFloatParameter($"{blendTreeName}_Speed", speed);
        }

        private async UniTask TransitionTask(string animationName, float duration)
        {
            _animator.CrossFade(animationName, duration);
            _currentAnimation = animationName;

            await UniTask.Delay((int)(duration * 1000));
        }

        public string GetAnimationName(string name)
        {
            string animationName = _animationInfos.GetAnimationNameByType(name);
            if (string.IsNullOrEmpty(animationName))
            {
                LogErr($"Animation not found for type: {name}");
                return string.Empty;
            }

            return animationName;
        }

        public float GetAnimationLength(string animationType)
        {
            if (string.IsNullOrEmpty(animationType))
            {
                return 0.1f;
            }

            string animationName = _animationInfos.GetAnimationNameByType(animationType);

            if (string.IsNullOrEmpty(animationName))
            {
                return 0.1f;
            }

            AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;

            foreach (AnimationClip clip in clips)
            {
                if (clip.name == animationName)
                {
                    return clip.length;
                }
            }

            LogHelper.LogWarning($"Animation clip not found: {animationName}");
            return 0.1f;
        }

        private static void Log(string msg)
        {
            LogHelper.Log(msg, "AnimatorPlayer");
        }

        private static void LogErr(string msg)
        {
            LogHelper.LogError(msg, "AnimatorPlayer");
        }
    }
}