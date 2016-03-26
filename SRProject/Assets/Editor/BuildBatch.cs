using UnityEngine;
using UnityEditor;
using System;

public class BuildBatch : MonoBehaviour
{
    private static void BuildNull()
    {
        string[] scenePaths = {
      "Assets/Scenes/null.unity"
    };
        string outputPath = Application.dataPath + "/../../out.exe";  // dataPathはAssetを指している
        BuildTarget target = BuildTarget.StandaloneWindows64;
        BuildOptions opt = BuildOptions.None;
        string error = BuildPipeline.BuildPlayer(scenePaths, outputPath, target, opt);
        if (!string.IsNullOrEmpty(error))
            Debug.LogError(error);
        EditorApplication.Exit(string.IsNullOrEmpty(error) ? 0 : 1);
    }
}