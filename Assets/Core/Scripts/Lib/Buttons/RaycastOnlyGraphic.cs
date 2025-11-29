using UnityEngine;
using UnityEngine.UI;

namespace Lib.Buttons
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasRenderer))]
    public class RaycastOnlyGraphic : MaskableGraphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }

        public override Texture mainTexture => s_WhiteTexture;
        
        public void SetRaycastTarget(bool isRaycastTarget)
        {
            raycastTarget = isRaycastTarget;
        }
    }
}