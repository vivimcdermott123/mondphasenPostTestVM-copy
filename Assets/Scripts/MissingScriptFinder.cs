using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Finds and lists all GameObjects with missing script references
/// </summary>
public class MissingScriptFinder : MonoBehaviour
{
    void Start()
    {
        FindMissingScripts();
    }
    
    [ContextMenu("Find Missing Scripts")]
    public void FindMissingScripts()
    {
        Debug.Log("=== SEARCHING FOR MISSING SCRIPTS ===");
        
        var allObjects = FindObjectsOfType<GameObject>();
        var objectsWithMissingScripts = new List<GameObject>();
        
        foreach (var obj in allObjects)
        {
            var components = obj.GetComponents<Component>();
            bool hasMissingScript = false;
            
            foreach (var component in components)
            {
                if (component == null)
                {
                    hasMissingScript = true;
                    break;
                }
            }
            
            if (hasMissingScript)
            {
                objectsWithMissingScripts.Add(obj);
            }
        }
        
        if (objectsWithMissingScripts.Count == 0)
        {
            Debug.Log("✅ NO MISSING SCRIPTS FOUND!");
            Debug.Log("All GameObjects are clean.");
        }
        else
        {
            Debug.LogWarning($"❌ FOUND {objectsWithMissingScripts.Count} GAMEOBJECT(S) WITH MISSING SCRIPTS:");
            Debug.Log("==========================================");
            
            for (int i = 0; i < objectsWithMissingScripts.Count; i++)
            {
                var obj = objectsWithMissingScripts[i];
                Debug.LogError($"MISSING SCRIPT #{i + 1}: {obj.name}");
                Debug.Log($"   Path: {GetFullPath(obj)}");
                Debug.Log($"   Position: {obj.transform.position}");
                Debug.Log($"   Active: {obj.activeInHierarchy}");
                Debug.Log("   ---");
            }
            
            Debug.Log("==========================================");
            Debug.Log("HOW TO FIX:");
            Debug.Log("1. In the Hierarchy, find each GameObject listed above");
            Debug.Log("2. Select the GameObject");
            Debug.Log("3. In the Inspector, look for red 'Missing Script' component(s)");
            Debug.Log("4. Click the gear icon (⚙️) next to 'Missing Script'");
            Debug.Log("5. Select 'Remove Component'");
            Debug.Log("6. Repeat for each GameObject listed above");
            Debug.Log("==========================================");
        }
        
        Debug.Log("=== MISSING SCRIPT SEARCH COMPLETED ===");
    }
    
    private string GetFullPath(GameObject obj)
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
    
    [ContextMenu("Highlight Missing Scripts in Hierarchy")]
    public void HighlightMissingScripts()
    {
        Debug.Log("=== HIGHLIGHTING MISSING SCRIPTS ===");
        
        var allObjects = FindObjectsOfType<GameObject>();
        var objectsWithMissingScripts = new List<GameObject>();
        
        foreach (var obj in allObjects)
        {
            var components = obj.GetComponents<Component>();
            bool hasMissingScript = false;
            
            foreach (var component in components)
            {
                if (component == null)
                {
                    hasMissingScript = true;
                    break;
                }
            }
            
            if (hasMissingScript)
            {
                objectsWithMissingScripts.Add(obj);
            }
        }
        
        if (objectsWithMissingScripts.Count == 0)
        {
            Debug.Log("✅ No missing scripts to highlight!");
            return;
        }
        
        Debug.Log($"Found {objectsWithMissingScripts.Count} GameObject(s) with missing scripts:");
        
        foreach (var obj in objectsWithMissingScripts)
        {
            Debug.Log($"🔍 SELECT THIS: {obj.name}");
            Debug.Log($"   Full Path: {GetFullPath(obj)}");
            
            // Try to select the object in the hierarchy
            #if UNITY_EDITOR
            UnityEditor.Selection.activeGameObject = obj;
            Debug.Log($"   ✅ Selected in Hierarchy: {obj.name}");
            #endif
        }
        
        Debug.Log("=== HIGHLIGHTING COMPLETED ===");
    }
    
    [ContextMenu("Count All Missing Scripts")]
    public void CountMissingScripts()
    {
        Debug.Log("=== COUNTING MISSING SCRIPTS ===");
        
        var allObjects = FindObjectsOfType<GameObject>();
        int totalMissingScripts = 0;
        var objectsWithMissingScripts = new List<GameObject>();
        
        foreach (var obj in allObjects)
        {
            var components = obj.GetComponents<Component>();
            int missingCount = 0;
            
            foreach (var component in components)
            {
                if (component == null)
                {
                    missingCount++;
                }
            }
            
            if (missingCount > 0)
            {
                totalMissingScripts += missingCount;
                objectsWithMissingScripts.Add(obj);
                Debug.LogWarning($"Object '{obj.name}' has {missingCount} missing script(s)");
            }
        }
        
        Debug.Log($"Total missing scripts found: {totalMissingScripts}");
        Debug.Log($"Objects affected: {objectsWithMissingScripts.Count}");
        
        if (totalMissingScripts == 0)
        {
            Debug.Log("✅ NO MISSING SCRIPTS FOUND!");
        }
        
        Debug.Log("=== COUNTING COMPLETED ===");
    }
} 