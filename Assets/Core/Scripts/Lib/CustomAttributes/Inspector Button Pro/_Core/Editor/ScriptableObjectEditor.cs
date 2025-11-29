using UnityEditor;
using UnityEngine;

namespace Lib.CustomAttributes.Editor
{
    //[CustomEditor (typeof (ScriptableObject), true, isFallback = true), CanEditMultipleObjects]
    [CustomEditor (typeof (ScriptableObject), true), CanEditMultipleObjects]
    public class ScriptableObjectEditor : AbstractEditor
    {
        protected override bool AllowSceneObjects
        {
            get { return false; }
        }
    }
}