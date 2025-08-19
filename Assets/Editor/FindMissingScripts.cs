using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FindMissingScripts : EditorWindow
{
    [MenuItem("Tools/Find Missing Scripts")]
    public static void ShowWindow()
    {
        GetWindow<FindMissingScripts>("Find Missing Scripts");
    }

    void OnGUI()
    {
        GUILayout.Label("Find Missing Scripts", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Scan Scene for Missing Scripts"))
        {
            ScanForMissingScripts();
        }

        if (GUILayout.Button("Remove All Missing Scripts"))
        {
            RemoveAllMissingScripts();
        }
    }

    void ScanForMissingScripts()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        List<GameObject> objectsWithMissingScripts = new List<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            Component[] components = obj.GetComponents<Component>();
            
            foreach (Component component in components)
            {
                if (component == null)
                {
                    objectsWithMissingScripts.Add(obj);
                    break;
                }
            }
        }

        if (objectsWithMissingScripts.Count == 0)
        {
            Debug.Log("✅ No GameObjects with missing scripts found!");
        }
        else
        {
            Debug.LogWarning($"❌ Found {objectsWithMissingScripts.Count} GameObjects with missing scripts:");
            
            foreach (GameObject obj in objectsWithMissingScripts)
            {
                Debug.LogWarning($"   - {obj.name} (Path: {GetGameObjectPath(obj)})");
            }
        }
    }

    void RemoveAllMissingScripts()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int totalRemoved = 0;

        foreach (GameObject obj in allObjects)
        {
            int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
            if (removed > 0)
            {
                Debug.Log($"Removed {removed} missing scripts from {obj.name}");
                totalRemoved += removed;
            }
        }

        Debug.Log($"✅ Total missing scripts removed: {totalRemoved}");
        
        // Scan again to confirm
        ScanForMissingScripts();
    }

    string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        Transform parent = obj.transform.parent;
        
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        
        return path;
    }
} 