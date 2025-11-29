using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Lib.Addressable;
using Lib.Debugger;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Lib.CacheManager
{
    public class AddressableInstantiator : IInstantiator
    {
        private const string ValidatedKeysPref = "AddressableInstantiator_ValidatedKeys";
        private readonly Dictionary<string,bool> _validatedKeyDict = new();
        
        public AddressableInstantiator()
        {
            LoadValidatedKeysFromPrefs();
        }
        
        public async UniTask<bool> IsValid(IList<string> keys)
        {
            foreach (var key in keys)
            {
                var isValid = await IsValid(key);
                if (!isValid) return false;
            }
            
            SaveValidatedKeysToPrefs();
            return true;
        }

        private async UniTask<bool> IsValid(string key)
        {
            if(string.IsNullOrEmpty(key))
                return false;
            
            if (_validatedKeyDict.TryGetValue(key, out bool isValid))
            {
                return isValid;
            }
                
            var handle = Addressables.LoadResourceLocationsAsync(key);
            await handle.Task;
            if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result.Count == 0)
            {
                _validatedKeyDict[key] = false;
                handle.Release();
                return false;
            }
            _validatedKeyDict[key] = true;
            handle.Release();
            return true;
        }

        public async UniTask<bool> DownloadKeys(IList<string> keys)
        {
            var sizeHandle = Addressables.GetDownloadSizeAsync(keys);
            await sizeHandle.Task;
            
            if (sizeHandle.Status != AsyncOperationStatus.Succeeded)
            {
                sizeHandle.Release();
                LogHelper.LogError($"Failed to get download size for keys: {string.Join(", ", keys)}");
                return false;
            }
            
            long downloadSize = sizeHandle.Result;
            sizeHandle.Release();
            
            // If download size is 0, assets are already up to date
            if (downloadSize == 0)
            {
                return true;
            }
            
            var handle = Addressables.DownloadDependenciesAsync(keys, Addressables.MergeMode.Union);
            await handle.Task;//todo timeout or cancel token 
            
            bool success = handle.Status == AsyncOperationStatus.Succeeded;
            handle.Release();
            return success;
        }

        public async UniTask<T> InstantiateAsync<T>(string key, Transform parent)
        {
            var isValid = await IsValid(key);
            if(!isValid) return default;

            var obj = await new InstantiateAddressableTask<T>(key, parent).ExecuteAsync();
            return obj;
        }

        private void LoadValidatedKeysFromPrefs()//TODO MOVE THIS TO ANOTHER CLASS
        {
            if (!PlayerPrefs.HasKey(ValidatedKeysPref))
                return;
                
            var serializedKeys = PlayerPrefs.GetString(ValidatedKeysPref);
            if (string.IsNullOrEmpty(serializedKeys))
                return;
                
            //"key1,key2,key3" 
            var validKeys = serializedKeys.Split(',');
            foreach (var key in validKeys)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    _validatedKeyDict[key] = true;
                }
            }
        }
        
        private void SaveValidatedKeysToPrefs()
        {
            var validKeys = new List<string>();
            foreach (var kvp in _validatedKeyDict)
            {
                if (kvp.Value) // Only save valid keys
                {
                    validKeys.Add(kvp.Key);
                }
            }
            
            var serializedKeys = string.Join(",", validKeys);
            PlayerPrefs.SetString(ValidatedKeysPref, serializedKeys);
            PlayerPrefs.Save();
        }
    }
}