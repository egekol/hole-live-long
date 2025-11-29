using System;
using UnityEngine;
using UnityEngine.UI;

namespace Lib.Buttons
{
    public abstract class BaseButton : MonoBehaviour
    {
        [SerializeField] protected Button _button;

        protected bool _isInteractable = true;

        public bool IsInteractable => _isInteractable;

        protected virtual void Awake()
        {
            if (_button != null)
            {
                _button.onClick.AddListener(HandleClick);
            }

            ApplyInteractableState();
        }

        protected virtual void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(HandleClick);
            }
        }

        protected abstract void HandleClick();

        public abstract void SetInteractable(bool interactable);
        
        public abstract void ResetAllOnClick();

        protected void ApplyInteractableState()
        {
            if (_button != null)
            {
                _button.interactable = _isInteractable;
            }
        }
    }
}
