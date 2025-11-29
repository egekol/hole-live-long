using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace Lib.Animation
{
    public static class DoTweenExtensions
    {
        public static TweenerCore<Vector3, Vector3, VectorOptions> DoAnchoredPos(this RectTransform target,
            Vector2 endValue, float duration, bool snapping = false)
        {
            TweenerCore<Vector3, Vector3, VectorOptions> t = DOTween.To(() => target.anchoredPosition,
                (DOSetter<Vector3>)(x => target.anchoredPosition = x), endValue, duration);
            t.SetOptions(snapping).SetTarget(target);
            return t;
        }

        public static TweenerCore<float, float, FloatOptions> DoAlpha(this Graphic target, float endValue,
            float duration)
        {
            return DOTween.To(
                () => target.color.a,
                x =>
                {
                    var c = target.color;
                    c.a = x;
                    target.color = c;
                },
                endValue,
                duration
            ).SetTarget(target);
        }
        public static TweenerCore<Color, Color, ColorOptions> DoColor(this Graphic target, Color endValue, float duration)
        {
            return DOTween.To(
                () => target.color,
                x => target.color = x,
                endValue,
                duration
            ).SetTarget(target);
        }
        
        public static TweenerCore<float, float, FloatOptions> DoFade(this CanvasGroup target, float endValue,
            float duration)
        {
            return DOTween.To(
                () => target.alpha,
                x => target.alpha = x,
                endValue,
                duration
            ).SetTarget(target);
        }
    }
}