using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Lib.SceneLoad
{
    [CreateAssetMenu(fileName = "SceneSettings", menuName = "GameSettings/SceneSettings", order = 0)]
    public class SceneSettingsSo : ScriptableObject, ISceneSettings
    {
        [SerializeField] private SceneBuildInfoData[] sceneBuildInfosList = Array.Empty<SceneBuildInfoData>();
        [SerializeField] private SceneAddressableInfoData[] sceneAddressableInfoList = Array.Empty<SceneAddressableInfoData>();

        public IReadOnlyList<SceneBuildInfoData> SceneBuildInfosList => sceneBuildInfosList;
        public IReadOnlyList<SceneAddressableInfoData> SceneAddressableInfoList => sceneAddressableInfoList;
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            var buildScenes = EditorBuildSettings.scenes.Where(s => s.enabled).ToArray();
            foreach (var info in sceneBuildInfosList)
            {
                if (info.SceneAsset != null)
                {
                    string scenePath = AssetDatabase.GetAssetPath(info.SceneAsset);
                    int buildIndex = Array.FindIndex(buildScenes, s => s.path == scenePath);
                    info.BuildIndex = buildIndex;
                    info.SceneName = info.SceneAsset.name;
                }
                else
                {
                    info.BuildIndex = -1;
                    info.SceneName = string.Empty;
                }
            }
            
            foreach (var info in sceneAddressableInfoList)
            {
                if (info.SceneAsset != null)
                {
                    info.SceneName = info.SceneAsset.name;
                }
                else
                {
                    info.SceneName = string.Empty;
                }
            }
        }
#endif
        public bool IsInAddressableList(string sceneName)
        {
            return System.Array.Exists(sceneAddressableInfoList, x => x.SceneName == sceneName);
        }

        public SceneBuildInfoData GetBuildInfoByName(string sceneName)
        {
            return System.Array.Find(sceneBuildInfosList, x => x.SceneName == sceneName);
        }

        public SceneBuildInfoData GetBuildInfoByIndex(int buildIndex)
        {
            return System.Array.Find(sceneBuildInfosList, x => x.BuildIndex == buildIndex);
        }
    }

    public interface ISceneSettings
    {
        bool IsInAddressableList(string sceneName);
        SceneBuildInfoData GetBuildInfoByName(string sceneName);
        SceneBuildInfoData GetBuildInfoByIndex(int buildIndex);
    }

    [Serializable]
    public class SceneBuildInfoData
    {
#if UNITY_EDITOR
        public SceneAsset SceneAsset;
#endif
        public int BuildIndex;
        public string SceneName;
    }

    [Serializable]
    public class SceneAddressableInfoData
    {
#if UNITY_EDITOR
        public SceneAsset SceneAsset;
#endif
        public string SceneName;
    }
}