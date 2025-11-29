using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Lib.Debugger;
using Lib.PriorityQueue;

namespace Lib.CacheManager
{
    public interface ICacheManager
    {
        void Cache(string key, CachePriority priority = CachePriority.Medium, Action<bool> onComplete = null);
        void Cache(List<string> keys, CachePriority priority = CachePriority.Medium, Action<bool> onComplete = null);
        void SetAutorun(bool autorun);
    }

    public abstract class CacheManagerBase : ICacheManager
    {
        private readonly SimplePriorityQueue<CacheItem, int> _cacheQueue = new();
        private bool _autorun = true;
        private bool _isDownloading = false;
        protected IInstantiator Instantiator;
        
        public abstract void Init(IInstantiator instantiator);

        private bool Enqueue(string key, CachePriority priority, Action<bool> onComplete)
        {
            var item = new CacheItem
            {
                Keys = new List<string> { key },
                OnComplete = onComplete
            };
            return Enqueue(item, (int)priority);
        }

        private bool Enqueue(List<string> keys, CachePriority priority, Action<bool> onComplete)
        {
            var item = new CacheItem
            {
                Keys = keys,
                OnComplete = onComplete
            };
            return Enqueue(item, (int)priority);
        }

        private bool Enqueue(CacheItem item, int priority)
        {
            if (_cacheQueue.Contains(item))
            {
                return _cacheQueue.TryUpdatePriority(item, priority);
            }

            {
                return _cacheQueue.EnqueueWithoutDuplicates(item, priority);
            }
        }

        public void Cache(string key, CachePriority priority = CachePriority.Medium, Action<bool> onComplete = null)
        {
            if (Enqueue(key, priority, onComplete))
            {
                if (_autorun && !_isDownloading)
                {
                    StartDownloading();
                }
            }
        }

        public void Cache(List<string> keys, CachePriority priority = CachePriority.Medium, Action<bool> onComplete = null)
        {
            if (Enqueue(keys, priority, onComplete))
            {
                if (_autorun && !_isDownloading)
                {
                    StartDownloading();
                }
            }
        }

        public void SetAutorun(bool autorun)
        {
            _autorun = autorun;
            if (_autorun && !_isDownloading && _cacheQueue.Count > 0)
            {
                StartDownloading();
            }
        }

        private void StartDownloading()
        {
            if (_isDownloading || _cacheQueue.Count == 0)
                return;

            _isDownloading = true;
            ProcessNextCacheItem().Forget();
        }

        private async UniTask ProcessNextCacheItem()
        {
            if (_cacheQueue.Count == 0)
            {
                _isDownloading = false;
                return;
            }

            var item = _cacheQueue.Dequeue();
            if (await IsItemValid(item))
            {
                var result = await DownloadCacheItem(item);
                Log($"Cache item {(result ? "succeeded" : "!failed!")}: [{item}]");
            }
            else
            {
                LogErr($"Items with keys [{item}] are not valid.");
            }

            ProcessNextCacheItem().Forget();
        }

        private UniTask<bool> IsItemValid(CacheItem item)
        {
            if (item.Keys is null || item.Keys.Count <= 0)
            {
                return UniTask.FromResult(false);
            }

            return Instantiator.IsValid(item.Keys);
        }

        private async UniTask<bool> DownloadCacheItem(CacheItem item)
        {
            var result = await Instantiator.DownloadKeys(item.Keys);
            item.OnComplete?.Invoke(result);
            return result;
        }

        protected static void Log(string message)
        {
            LogHelper.Log(message, "CacheManager");
        }

        private static void LogErr(string message)
        {
            LogHelper.LogError(message, "CacheManager");
        }
        
    }

    internal struct CacheItem
    {
        public List<string> Keys;
        public Action<bool> OnComplete;

        public override string ToString()
        {
            return string.Join(", ", Keys);
        }
    }

    public enum CachePriority
    {
        VeryHigh = 0,
        High = 1,
        Medium = 2,
        Low = 3,
        VeryLow = 4
    }
}