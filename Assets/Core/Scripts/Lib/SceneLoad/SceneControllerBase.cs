
using Cysharp.Threading.Tasks;
using Lib.Debugger;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lib.SceneLoad
{
    public interface ISceneController
    {
        void LoadSceneByBuildIndex(int buildIndex);
        void LoadSceneByAddressableName(string sceneName);
        UniTask LoadSceneByBuildIndexAsync(int buildIndex);
        UniTask<AsyncOperation> LoadSceneOperationAsync(string sceneName);
        UniTask LoadSceneAsync(string sceneName);
        UniTask UnloadSceneAsync(string sceneName);
        string CurrentScene { get; }
        void Init(ISceneSettings sceneSettings);
    }

    public abstract class SceneControllerBase : ISceneController
    {
        private AsyncOperation _sceneChangeOperation;
        private readonly SceneLoadManager _sceneLoadManager = new();
        protected ISceneSettings SceneSettings;
        public string CurrentScene { get; private set; }

        public abstract void Init(ISceneSettings sceneSettings);
        
        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            LogHelper.Log($"Active scene changed from {oldScene.name} to {newScene.name}");
            //todo event signal
        }
        private void OnActiveSceneChanged(string oldScene, string newScene)
        {
            LogHelper.Log($"Active scene changed from {oldScene} to {newScene}");
            //todo event signal
        }

        public void LoadSceneByBuildIndex(int buildIndex)
        {
            LoadSceneByBuildIndexAsync(buildIndex).Forget();
        }

        public void LoadSceneByAddressableName(string sceneName)
        {
            LoadSceneOperationAsync(sceneName).Forget();
        }

        public async UniTask LoadSceneByBuildIndexAsync(int buildIndex)
        {
            var info = SceneSettings.GetBuildInfoByIndex(buildIndex);
            if (info != null )
            {
                await _sceneLoadManager.LoadSceneAsync(buildIndex);
            }
            else
            {
                LogHelper.LogError($"Scene with build index {buildIndex} not found in SceneSettingsSo.", "SceneController");
            }
        }

        public async UniTask<AsyncOperation> LoadSceneOperationAsync(string sceneName)
        {
            var inAddressableList = SceneSettings.IsInAddressableList(sceneName);
            if (inAddressableList)
            {
                var asyncOperation = await _sceneLoadManager.LoadSceneFromAddressableAsync(sceneName);
                SetActiveScene(sceneName);
                return asyncOperation;
            }

            var buildInfo = SceneSettings.GetBuildInfoByName(sceneName);
            if (buildInfo is not null && buildInfo.BuildIndex >= 0)
            {
                var loadSceneAsync = await _sceneLoadManager.LoadSceneAsync(buildInfo.BuildIndex);
                SetActiveScene(sceneName);
                return loadSceneAsync;
            }
            else
            {
                LogHelper.LogError($"Scene with name {sceneName} not found in SceneSettingsSo.", "SceneController");
                return null;
            }
        }

        public async UniTask LoadSceneAsync(string sceneName)
        {
            await LoadSceneOperationAsync(sceneName);
        }

        private void SetCurrentScene(string sceneName)
        {
            LogHelper.Log($"Setting current scene to: {sceneName}", "SceneController");
            CurrentScene = sceneName;
        }

        public async UniTask UnloadSceneAsync(string sceneName)
        {
            var contains = SceneSettings.IsInAddressableList(sceneName);
            if (contains)
            {
                await _sceneLoadManager.UnloadSceneFromAddressableAsync(sceneName);
            }
            else
            {
                await _sceneLoadManager.UnloadSceneAsync(sceneName);
            }
        }
        
        private void SetActiveScene(string sceneName)
        {
            var oldScene = CurrentScene; 
            SetCurrentScene(sceneName);
            OnActiveSceneChanged(oldScene, sceneName);
        }
    
    }
}