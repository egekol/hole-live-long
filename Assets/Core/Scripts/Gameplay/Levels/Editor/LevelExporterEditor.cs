#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Core.Scripts.Gameplay.Levels.Editor
{
    public static class LevelExporterEditor
    {
        [MenuItem("Tools/Levels/Import All CSVs")]
        public static void ImportAllCsvs()
        {
            var csvFiles = LevelExporter.GetAllCsvFiles();

            if (csvFiles.Length == 0)
            {
                EditorUtility.DisplayDialog("Import CSVs", "No CSV files found in the folder.", "OK");
                return;
            }

            // Ensure output folder exists
            if (!Directory.Exists(LevelExporter.LevelsSoFolderPath))
            {
                Directory.CreateDirectory(LevelExporter.LevelsSoFolderPath);
            }

            int successCount = 0;
            int failCount = 0;

            for (int i = 0; i < csvFiles.Length; i++)
            {
                var csvPath = csvFiles[i];
                var fileName = Path.GetFileNameWithoutExtension(csvPath);

                EditorUtility.DisplayProgressBar("Importing CSVs", $"Processing: {fileName}", (float)i / csvFiles.Length);

                try
                {
                    var csvContent = File.ReadAllText(csvPath);
                    var levelData = LevelExporter.ParseCsvToLevelData(csvContent, fileName, i);

                    if (levelData != null)
                    {
                        var assetPath = Path.Combine(LevelExporter.LevelsSoFolderPath, 
                            string.Format(LevelExporter.LevelsDataName, fileName) + ".asset");

                        // Check if asset already exists
                        var existingAsset = AssetDatabase.LoadAssetAtPath<LevelDataSo>(assetPath);
                        if (existingAsset != null)
                        {
                            // Update existing asset
                            EditorUtility.CopySerialized(levelData, existingAsset);
                            EditorUtility.SetDirty(existingAsset);
                            Object.DestroyImmediate(levelData);
                            Debug.Log($"Updated existing level: {assetPath}");
                        }
                        else
                        {
                            // Create new asset
                            AssetDatabase.CreateAsset(levelData, assetPath);
                            Debug.Log($"Created new level: {assetPath}");
                        }

                        successCount++;
                    }
                    else
                    {
                        failCount++;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to import {fileName}: {e.Message}");
                    failCount++;
                }
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Import Complete", 
                $"Successfully imported: {successCount}\nFailed: {failCount}", "OK");
        }

        [MenuItem("Tools/Levels/Import Single CSV")]
        public static void ImportSingleCsv()
        {
            var csvPath = EditorUtility.OpenFilePanel("Select CSV File", LevelExporter.LevelsCsvFolderPath, "csv");

            if (string.IsNullOrEmpty(csvPath))
                return;

            // Ensure output folder exists
            if (!Directory.Exists(LevelExporter.LevelsSoFolderPath))
            {
                Directory.CreateDirectory(LevelExporter.LevelsSoFolderPath);
            }

            var fileName = Path.GetFileNameWithoutExtension(csvPath);

            try
            {
                var csvContent = File.ReadAllText(csvPath);
                var levelData = LevelExporter.ParseCsvToLevelData(csvContent, fileName, 0);

                if (levelData != null)
                {
                    var assetPath = Path.Combine(LevelExporter.LevelsSoFolderPath, 
                        string.Format(LevelExporter.LevelsDataName, fileName) + ".asset");

                    // Check if asset already exists
                    var existingAsset = AssetDatabase.LoadAssetAtPath<LevelDataSo>(assetPath);
                    if (existingAsset != null)
                    {
                        EditorUtility.CopySerialized(levelData, existingAsset);
                        EditorUtility.SetDirty(existingAsset);
                        Object.DestroyImmediate(levelData);
                        Debug.Log($"Updated existing level: {assetPath}");
                    }
                    else
                    {
                        AssetDatabase.CreateAsset(levelData, assetPath);
                        Debug.Log($"Created new level: {assetPath}");
                    }

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    EditorUtility.DisplayDialog("Import Complete", $"Successfully imported: {fileName}", "OK");
                    
                    // Select the newly created asset
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath<LevelDataSo>(assetPath);
                }
                else
                {
                    EditorUtility.DisplayDialog("Import Failed", $"Failed to parse CSV: {fileName}", "OK");
                }
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("Import Failed", $"Error: {e.Message}", "OK");
                Debug.LogError($"Failed to import {fileName}: {e.Message}");
            }
        }

        [MenuItem("Tools/Levels/Open CSV Folder")]
        public static void OpenCsvFolder()
        {
            if (Directory.Exists(LevelExporter.LevelsCsvFolderPath))
            {
                EditorUtility.RevealInFinder(LevelExporter.LevelsCsvFolderPath);
            }
            else
            {
                Directory.CreateDirectory(LevelExporter.LevelsCsvFolderPath);
                EditorUtility.RevealInFinder(LevelExporter.LevelsCsvFolderPath);
            }
        }

        [MenuItem("Tools/Levels/Open LevelData Folder")]
        public static void OpenLevelDataFolder()
        {
            if (Directory.Exists(LevelExporter.LevelsSoFolderPath))
            {
                EditorUtility.RevealInFinder(LevelExporter.LevelsSoFolderPath);
            }
            else
            {
                Directory.CreateDirectory(LevelExporter.LevelsSoFolderPath);
                EditorUtility.RevealInFinder(LevelExporter.LevelsSoFolderPath);
            }
        }
    }
}
#endif

