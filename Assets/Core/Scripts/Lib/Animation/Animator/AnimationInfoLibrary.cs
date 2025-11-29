using System.Collections.Generic;

namespace Lib.Animation.Animator
{
    public interface IAnimationLibrary
    {
        string GetAnimationNameByType(string animationType);
        Dictionary<string, string> GetDictionary();
    }

    public abstract class AnimationInfoLibrary : IAnimationLibrary
    {
        protected Dictionary<string, string> AnimationDictionary;

        protected AnimationInfoLibrary()
        {
            AnimationDictionary = new Dictionary<string, string>();
            Init();
        }

        private void Init()
        {
            InitializeAnimationDictionary();
        }

        protected abstract void InitializeAnimationDictionary();

        public string GetAnimationNameByType(string animationType)
        {
            if (AnimationDictionary.ContainsKey(animationType))
                return AnimationDictionary[animationType];
            return string.Empty;
        }

        public Dictionary<string, string> GetDictionary()
        {
            return AnimationDictionary;
        }
    }
} 