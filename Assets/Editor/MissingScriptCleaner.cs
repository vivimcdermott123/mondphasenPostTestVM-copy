using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MissingScriptCleaner : EditorWindow
{
    private List<GameObject> gameObjectsWithMissingScripts = new List<GameObject>();
    private Vector2 scrollPosition;
    private bool showDetails = false;

    [MenuItem("Tools/Missing Script Cleaner")]
    public static void ShowWindow()
    {
        GetWindow<MissingScriptCleaner>("Missing Script Cleaner");
    }

    void OnEnable()
    {
        FindGameObjectsWithMissingScripts();
    }

    void OnGUI()
    {
        GUILayout.Label("Missing Script Cleaner", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (gameObjectsWithMissingScripts.Count == 0)
        {
            EditorGUILayout.HelpBox("No GameObjects with missing scripts found!", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox($"Found {gameObjectsWithMissingScripts.Count} GameObjects with missing scripts.", MessageType.Warning);
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Remove All Missing Scripts"))
            {
                if (EditorUtility.DisplayDialog("Confirm", 
                    $"Are you sure you want to remove missing scripts from {gameObjectsWithMissingScripts.Count} GameObjects?", 
                    "Yes", "Cancel"))
                {
                    RemoveAllMissingScripts();
                }
            }
            
            GUILayout.Space(10);
            
            showDetails = EditorGUILayout.Foldout(showDetails, "Show Details");
            
            if (showDetails)
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                
                foreach (GameObject obj in gameObjectsWithMissingScripts)
                {
                    EditorGUILayout.BeginHorizontal();
                    
                    EditorGUILayout.ObjectField(obj, typeof(GameObject), true);
                    
                    if (GUILayout.Button("Remove", GUILayout.Width(80)))
                    {
                        RemoveMissingScriptsFromObject(obj);
                        FindGameObjectsWithMissingScripts(); // Refresh immediately
                    }
                    
                    if (GUILayout.Button("Select", GUILayout.Width(80)))
                    {
                        Selection.activeGameObject = obj;
                        EditorGUIUtility.PingObject(obj);
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
                
                EditorGUILayout.EndScrollView();
            }
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Refresh"))
        {
            FindGameObjectsWithMissingScripts();
        }
        
        // Auto-refresh every few seconds
        if (Event.current.type == EventType.Repaint)
        {
            Repaint();
        }
    }

    void FindGameObjectsWithMissingScripts()
    {
        gameObjectsWithMissingScripts.Clear();
        
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            if (HasMissingScripts(obj))
            {
                gameObjectsWithMissingScripts.Add(obj);
            }
        }
    }

    bool HasMissingScripts(GameObject obj)
    {
        Component[] components = obj.GetComponents<Component>();
        
        foreach (Component component in components)
        {
            if (component == null)
            {
                return true;
            }
        }
        
        return false;
    }

    void RemoveAllMissingScripts()
    {
        int removedCount = 0;
        
        // Use a more reliable method to remove missing scripts
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            removedCount += RemoveMissingScriptsFromObject(obj);
        }
        
        FindGameObjectsWithMissingScripts();
        
        EditorUtility.DisplayDialog("Complete", 
            $"Removed missing scripts from {removedCount} components.", "OK");
    }

    int RemoveMissingScriptsFromObject(GameObject obj)
    {
        int removedCount = 0;
        
        // Use Unity's built-in method to remove missing scripts
        int missingScripts = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
        removedCount += missingScripts;
        
        // Also check for any remaining null components
        Component[] components = obj.GetComponents<Component>();
        List<Component> componentsToRemove = new List<Component>();
        
        foreach (Component component in components)
        {
            if (component == null)
            {
                componentsToRemove.Add(component);
            }
        }
        
        foreach (Component component in componentsToRemove)
        {
            DestroyImmediate(component);
            removedCount++;
        }
        
        return removedCount;
    }
} 