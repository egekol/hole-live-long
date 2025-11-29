using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Lib.Debugger;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Lib.SceneLoad
{
    public class SceneLoadManager
    {
        private readonly List<SceneInstance> _loadedAddressableScenes = new();
        public async UniTask<AsyncOperation> LoadSceneFromAddressableAsync(string addressableName, LoadSceneMode mode = LoadSceneMode.Additive)
        {
            var handle = Addressables.LoadSceneAsync(addressableName, mode, activateOnLoad: false);
            var sceneInstance = await handle.Task.AsUniTask();
            _loadedAddressableScenes.Add(sceneInstance);
            return sceneInstance.ActivateAsync();
        }

        public async UniTask UnloadSceneFromAddressableAsync(string sceneName)
        {
            var sceneInstance = _loadedAddressableScenes.Find(s => s.Scene.name == sceneName);
            if (sceneInstance.Scene.IsValid())
            {
                _loadedAddressableScenes.Remove(sceneInstance);
                var handle = Addressables.UnloadSceneAsync(sceneInstance);
                await handle.Task.AsUniTask();
            }
            else
            {
                LogHelper.LogError($"Scene {sceneName} is not loaded or invalid.");
            }
        }

        public async UniTask<AsyncOperation> LoadSceneAsync(int buildIndex, LoadSceneMode mode = LoadSceneMode.Additive)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(buildIndex, mode);
            await asyncOperation.ToUniTask();
            return asyncOperation;
        }

        public UniTask LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Additive)
        {
            return SceneManager.LoadSceneAsync(sceneName, mode).ToUniTask();
        }

        public UniTask UnloadSceneAsync(int buildIndex)
        {
            return SceneManager.UnloadSceneAsync(buildIndex).ToUniTask();
        }
        
        public UniTask UnloadSceneAsync(string sceneName)
        {
            return SceneManager.UnloadSceneAsync(sceneName).ToUniTask();
        }
    }
}