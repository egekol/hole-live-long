using Cysharp.Threading.Tasks;
using Lib.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Lib.Addressable
{
    public class InstantiateAddressableTask<T> : ATask<T>
    {
        private readonly string _key;
        private readonly Transform _parent;

        public InstantiateAddressableTask(string key, Transform parent)
        {
            _key = key;
            _parent = parent;
        }

        public override async UniTask<T> ExecuteAsync()
        {
            var handle = Addressables.InstantiateAsync(_key, _parent);
            var go = await handle.Task.AsUniTask();
            if (handle.Status != AsyncOperationStatus.Succeeded || go == null)
            {
                handle.Release();
                Complete();
                return default;
            }

            var obj = go.GetComponent<T>();
            Complete();
            return obj;

        }
    }
}