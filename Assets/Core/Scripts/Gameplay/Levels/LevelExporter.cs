using System.Collections.Generic;
using UnityEngine;

namespace Core.Scripts.Gameplay.Levels
{
    public class LevelExporter : MonoBehaviour
    {
        [SerializeField] private List<LevelDataSo> _levelDataList;
        public List<LevelDataSo> LevelDataList => _levelDataList;
        public const string LevelsCsvFolderPath = "Assets/Core/LevelCsv";
        public const string LevelsSoFolderPath = "Assets/Core/ScriptableObjects/LevelDataList";
        public const string LevelsDataName = "LevelData_{0}";
        
        
    }
}