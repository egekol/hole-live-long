using Lib.CustomAttributes.Scripts;
using UnityEditor;

namespace Lib.CustomAttributes._Premium_Version.Editor
{
    //[CustomEditor (typeof (ButtonScriptableObject), true, isFallback = true), CanEditMultipleObjects]
    [CustomEditor (typeof (ButtonScriptableObject), true), CanEditMultipleObjects]
    public class ProScriptableObjectEditor : AbstractProEditor
    { 
        protected override bool AllowSceneObjects
        {
            get { return false; }
        }
    }
}