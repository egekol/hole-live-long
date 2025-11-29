using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lib.Buttons
{
    public class CommonButton : BaseButton
    {
        [SerializeField] private TextMeshProUGUI _tmp;
        [SerializeField] private Image _bgEnabled;
        [SerializeField] private Image _bgDisabled;

        private event Action<bool> _onClick;

        private bool _isEnabled = true;

        public bool IsEnabled => _isEnabled;

        protected override void Awake()
        {
            base.Awake();
            ApplyStateToUnityComponents();
            UpdateVisuals();
        }

        protected override void HandleClick()
        {
            if (!_isInteractable)
            {
                return;
            }

            _onClick?.Invoke(_isEnabled);
        }

        public void OnClick(Action<bool> onClick)
        {
            if (onClick != null)
            {
                _onClick += onClick;
            }
        }

        public void RemoveOnClick(Action<bool> onClick)
        {
            if (onClick != null)
            {
                _onClick -= onClick;
            }
        }

        public override void SetInteractable(bool interactable)
        {
            _isInteractable = interactable;
            if (_button != null)
            {
                _button.interactable = interactable;
            }
            UpdateVisuals();
        }

        public override void ResetAllOnClick()
        {
            _onClick = null;
        }

        public void SetEnabled(bool isEnabled)
        {
            _isEnabled = isEnabled;
            ApplyStateToUnityComponents();
            UpdateVisuals();
        }

        public void SetText(string text)
        {
            if (_tmp != null)
            {
                _tmp.text = text;
            }
        }

        private void ApplyStateToUnityComponents()
        {
            if (_button != null)
            {
                _button.interactable = _isInteractable;
            }
        }

        private void UpdateVisuals()
        {
            var showEnabledBg = _isEnabled;

            if (_bgEnabled != null)
            {
                _bgEnabled.gameObject.SetActive(showEnabledBg);
            }

            if (_bgDisabled != null)
            {
                _bgDisabled.gameObject.SetActive(!showEnabledBg);
            }
        }
    }
}