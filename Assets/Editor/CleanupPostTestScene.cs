using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Linq;

public class CleanupPostTestScene
{
    [MenuItem("Tools/Cleanup Post-Test Scene")]
    public static void CleanupScene()
    {
        // Open the PostTestScene
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/PostTestScene.unity");

        // List of names to remove (add more as needed)
        string[] namesToRemove = { "Moonphases_Manager", "Tutorial", "Intro", "Zurück", "Weiter" };

        // Remove unwanted GameObjects
        foreach (var go in Object.FindObjectsOfType<GameObject>())
        {
            if (namesToRemove.Any(n => go.name.Contains(n)))
            {
                Object.DestroyImmediate(go);
            }
        }

        // Set PostTestScene as the first scene in Build Settings
        var buildScenes = EditorBuildSettings.scenes.ToList();
        string postTestPath = "Assets/Scenes/PostTestScene.unity";
        if (!buildScenes.Any(s => s.path == postTestPath))
        {
            buildScenes.Add(new EditorBuildSettingsScene(postTestPath, true));
        }
        // Move PostTestScene to the top
        int idx = buildScenes.FindIndex(s => s.path == postTestPath);
        if (idx > 0)
        {
            var postTestScene = buildScenes[idx];
            buildScenes.RemoveAt(idx);
            buildScenes.Insert(0, postTestScene);
        }
        EditorBuildSettings.scenes = buildScenes.ToArray();

        // Save the scene
        EditorSceneManager.SaveScene(scene);

        Debug.Log("Post-Test Scene cleaned and set as startup scene!");
    }
} 