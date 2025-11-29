using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;
using Lib.Debugger;
using Lib.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Lib.Addressable
{
    public class BatchLoadAddressableTask<T> : ATask<IReadOnlyDictionary<string, T>>
    {
        private readonly IList<string> _keys;

        public BatchLoadAddressableTask(IList<string> keys)
        {
            _keys = keys ?? throw new ArgumentNullException(nameof(keys));
        }

        public override async UniTask<IReadOnlyDictionary<string, T>> ExecuteAsync()
        {
            try
            {
                // Create parallel loading tasks
                var loadingTasks = new List<UniTask<KeyValuePair<string, T>>>();
                foreach (string key in _keys)
                {
                    loadingTasks.Add(LoadSingleAsync(key));
                }

                // Wait for all loads to complete in parallel
                var results = await UniTask.WhenAll(loadingTasks);

                // Convert results to dictionary
                var resultDictionary = new Dictionary<string, T>();
                foreach (var result in results)
                {
                    resultDictionary[result.Key] = result.Value;
                }

                Complete();
                return resultDictionary;
            }
            catch (Exception e)
            {
                LogHelper.LogError($"Exception in batch loading: {e}", "BatchLoadAddressableTask");
                Complete();
                return new Dictionary<string, T>();
            }
        }

        private async UniTask<KeyValuePair<string, T>> LoadSingleAsync(string key)
        {
            try
            {
                var handle = Addressables.LoadAssetAsync<T>(key);
                var asset = await handle.Task.AsUniTask();

                if (handle.Status != AsyncOperationStatus.Succeeded || asset == null)
                {
                    LogHelper.LogError($"Failed to load addressable asset with key: {key}", "BatchLoadAddressableTask");
                    return new KeyValuePair<string, T>(key, default);
                }

                return new KeyValuePair<string, T>(key, asset);
            }
            catch (Exception e)
            {
                LogHelper.LogError($"Exception loading {key}: {e}", "BatchLoadAddressableTask");
                return new KeyValuePair<string, T>(key, default);
            }
        }
    }
}