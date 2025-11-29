using System;
using Cysharp.Threading.Tasks;
using Lib.Debugger;
using Lib.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Lib.Addressable
{
    public class LoadAddressableTask<T> : ATask<T>
    {
        private readonly string _key;

        public LoadAddressableTask(string key)
        {
            _key = key;
        }
        
        public override async UniTask<T> ExecuteAsync()
        {
            try
            {
                var handle = Addressables.LoadAssetAsync<T>(_key);
                var asset = await handle.Task.AsUniTask();
                
                if (handle.Status != AsyncOperationStatus.Succeeded || asset == null)
                {
                    LogHelper.LogError($"Failed to load addressable asset with key: {_key}", "LoadAddressableTask");
                    Complete();
                    return default;
                }
                
                Complete();
                return asset;
            }
            catch (Exception e)
            {
                LogHelper.LogError($"Exception: {e}", "LoadAddressableTask");
            }
            Complete();
            return default;
        }
    }
}