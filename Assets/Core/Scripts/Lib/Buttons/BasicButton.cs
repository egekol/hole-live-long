using System;
using UnityEngine;

namespace Lib.Buttons
{
    public class BasicButton : BaseButton
    {
        private event Action _onClick;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _onClick = null;
        }

        protected override void HandleClick()
        {
            if (!_isInteractable)
            {
                return;
            }

            _onClick?.Invoke();
        }

        public void OnClick(Action onClick)
        {
            if (onClick != null)
            {
                _onClick += onClick;
            }
        }

        public void RemoveOnClick(Action onClick)
        {
            if (onClick != null)
            {
                _onClick -= onClick;
            }
        }

        public override void SetInteractable(bool interactable)
        {
            _isInteractable = interactable;
            ApplyInteractableState();
        }

        public override void ResetAllOnClick()
        {
            _onClick = null;
        }
    }
}

