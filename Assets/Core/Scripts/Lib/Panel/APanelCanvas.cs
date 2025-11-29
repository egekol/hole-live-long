using UnityEngine;

namespace Lib.Panel
{
    public abstract class APanelCanvas : MonoBehaviour, IPanelCanvas
    {
        [SerializeField] protected Canvas _canvas;
        [SerializeField] protected Transform _parent;
        
        
        public Canvas Canvas => _canvas;
        public Transform Parent => _parent;
    }

    public interface IPanelCanvas
    {
        Canvas Canvas { get; }
        Transform Parent { get; }
    }
}