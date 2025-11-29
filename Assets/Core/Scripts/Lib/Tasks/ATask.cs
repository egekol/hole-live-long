using System;
using Cysharp.Threading.Tasks;

namespace Lib.Tasks
{
    public abstract class ATask<T>
    {
        public event Action OnStart;
        public event Action OnComplete;

        public UniTask Awaiter => _tcs.Task;
        
        public bool IsCompleted => _tcs.Task.Status.IsCompleted();
        
        public bool IsSuccess => _tcs.Task.Status == UniTaskStatus.Succeeded;
        
        public bool IsCancelled => _tcs.Task.Status == UniTaskStatus.Canceled;
        
        private UniTaskCompletionSource<bool> _tcs = new();

        public abstract UniTask<T> ExecuteAsync();

        public virtual void Run()
        {
            RunAsync().Forget();
        }

        public async UniTask RunAsync()
        {
            OnStart?.Invoke();
            ExecuteAsync().Forget();
            await _tcs.Task;
        }

        public virtual void Cancel()
        {
            if (!IsCompleted)
            {
                _tcs.TrySetCanceled();
                OnComplete?.Invoke();
            }
        }
        
        public virtual void Complete()
        {
            _tcs.TrySetResult(true);
            OnComplete?.Invoke();
        }

        protected virtual void ClearActions()
        {
            OnComplete = null;
            OnStart = null;
        }
        
        public void PrepareForReuse()
        {
            Cancel();
            _tcs = new UniTaskCompletionSource<bool>();
            ClearActions();
        }
    }
}