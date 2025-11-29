using Lib.CustomAttributes.Scripts;
using UnityEditor;
using UnityEngine;

namespace Lib.CustomAttributes.Editor
{
    //[CustomEditor(typeof(MonoBehaviour), true, isFallback = true), CanEditMultipleObjects]


    [CustomEditor(typeof(MonoBehaviour), true, isFallback = true), CanEditMultipleObjects]
    public class MonoBehaviourEditor : AbstractEditor
    {
        /// <summary>this is used by Unity, to avoid dragging scene object references into prefab fields</summary>
        protected override bool AllowSceneObjects
        {
            get { return !serializedObject.targetObject.IsPrefab (); }
        }
    } 
}