using UnityEngine;

/// <summary>
/// Simple script to help fix Unity cache issues
/// Attach this to any GameObject and use the context menu options
/// </summary>
public class FixUnityCache : MonoBehaviour
{
    [ContextMenu("Fix Missing Scripts")]
    public void FixMissingScripts()
    {
        Debug.Log("=== FIXING MISSING SCRIPTS ===");
        
        // Find all GameObjects with missing scripts
        var allObjects = FindObjectsOfType<GameObject>();
        int fixedCount = 0;
        
        foreach (var obj in allObjects)
        {
            var components = obj.GetComponents<Component>();
            foreach (var component in components)
            {
                if (component == null)
                {
                    Debug.Log($"Found missing script on {obj.name}");
                    fixedCount++;
                }
            }
        }
        
        if (fixedCount == 0)
        {
            Debug.Log("✓ No missing scripts found");
        }
        else
        {
            Debug.Log($"Found {fixedCount} missing script references");
        }
        
        Debug.Log("=== MISSING SCRIPTS CHECK COMPLETED ===");
    }
    
    [ContextMenu("Check Script Files")]
    public void CheckScriptFiles()
    {
        Debug.Log("=== CHECKING SCRIPT FILES ===");
        
        // Check if key scripts exist
        var scripts = new string[] 
        {
            "Moonphases_Manager",
            "PostTestSetupFix", 
            "VisionChecker",
            "DebugWhiteScreen"
        };
        
        foreach (var scriptName in scripts)
        {
            var scriptType = System.Type.GetType(scriptName);
            if (scriptType != null)
            {
                Debug.Log($"✓ {scriptName} script exists");
            }
            else
            {
                Debug.LogError($"❌ {scriptName} script not found");
            }
        }
        
        Debug.Log("=== SCRIPT FILES CHECK COMPLETED ===");
    }
    
    [ContextMenu("Instructions to Fix Cache")]
    public void ShowCacheFixInstructions()
    {
        Debug.Log("=== TO FIX UNITY CACHE ISSUES ===");
        Debug.Log("1. Save your scene and project");
        Debug.Log("2. Close Unity completely");
        Debug.Log("3. Delete the 'Library' folder from your project");
        Debug.Log("4. Reopen Unity");
        Debug.Log("5. Wait for Unity to rebuild the cache");
        Debug.Log("6. The missing script errors should be gone");
        Debug.Log("=== INSTRUCTIONS COMPLETED ===");
    }
} 