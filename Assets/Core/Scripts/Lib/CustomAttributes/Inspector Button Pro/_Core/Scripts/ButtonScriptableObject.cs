//using DefaultNamespace;

using UnityEngine;

namespace Lib.CustomAttributes.Scripts
{

    /// <summary>
    /// previous called ProScriptableObject
    /// </summary>
    public class ButtonScriptableObject : ScriptableObject
    {
        #if UNITY_EDITOR
        [SerializeField, HideInInspector]
        private SerializedMethod[] serializedMethods;
        #endif
    }
}