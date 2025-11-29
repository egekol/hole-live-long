using Lib.CustomAttributes.Scripts;
using UnityEditor;

namespace Lib.CustomAttributes._Premium_Version.Editor
{
    //[CustomEditor (typeof (ButtonMonoBehavior), true, isFallback = true), CanEditMultipleObjects]
    [CustomEditor(typeof(ButtonMonoBehavior), true), CanEditMultipleObjects]
    public class ProMonoBehaviourEditor : AbstractProEditor
    {
        /// <summary>this is used by Unity, to avoid dragging scene object references into prefab fields</summary>
        protected override bool AllowSceneObjects
        {
            get { return !serializedObject.targetObject.IsPrefab (); }
        }
    }
}