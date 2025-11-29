using Cysharp.Threading.Tasks;
using Lib.Animation;
using Lib.Debugger;
using UnityEngine;

namespace Lib.Panel
{
    public interface IPanel
    {
        UniTask Show();
        UniTask Hide();
        void Dismiss();
        UniTask GetHideAwaiter();
        UniTask GetShowAwaiter();
        void Initialize(IPanelCanvas panelCanvas);
        void SetData(IPanelData data);
    }

    public abstract class APanel : MonoBehaviour
    {
        [SerializeField] protected AAnimation _showAnimation;
        [SerializeField] protected AAnimation _hideAnimation;
        private UniTaskCompletionSource _showTcs;
        private UniTaskCompletionSource _hideTcs;
        private bool _isShowing;
        private IPanelCanvas _panelCanvas;

        public void Initialize(IPanelCanvas panelCanvas)
        {
            _panelCanvas = panelCanvas;
            _showTcs = new UniTaskCompletionSource();
            _hideTcs = new UniTaskCompletionSource();
            transform.SetParent(_panelCanvas.Parent);
        }

        public virtual async UniTask Show()
        {
            if (_isShowing)
                return;

            LogHelper.Log($"Show Panel: {name}", "APanel");

            _isShowing = true;

            if (_showAnimation != null)
            {
                await _showAnimation.Play();
            }

            _showTcs.TrySetResult();
        }

        public virtual async UniTask Hide()
        {
            if (!_isShowing)
                return;

            if (_hideAnimation != null)
            {
                await _hideAnimation.Play();
            }

            _isShowing = false;
            _hideTcs.TrySetResult();
        }

        public abstract void SetData(IPanelData data);

        public UniTask GetHideAwaiter()
        {
            return _hideTcs.Task;
        }

        public UniTask GetShowAwaiter()
        {
            return _showTcs.Task;
        }

        public void Dismiss()
        {
            _hideTcs.TrySetResult();
            _showTcs.TrySetResult();
            Destroy(gameObject);
        }
    }

    public interface IPanelData
    {
    }
}