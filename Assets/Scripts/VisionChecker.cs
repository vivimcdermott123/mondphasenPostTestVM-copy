using UnityEngine;

public class VisionChecker : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== VISION CHECKER STARTED ===");
        CheckVisionOS();
    }
    
    [ContextMenu("Check VisionOS")]
    public void CheckVisionOS()
    {
        Debug.Log("Checking VisionOS compatibility...");
        
        // Check main manager
        var manager = FindObjectOfType<Moonphases_Manager>();
        if (manager != null)
        {
            Debug.Log("✓ Moonphases_Manager found");
        }
        else
        {
            Debug.LogError("❌ Moonphases_Manager not found!");
        }
        
        // Check post test - try multiple ways to find it
        var postTest = FindObjectOfType<PostTestSetupFix>();
        if (postTest != null)
        {
            Debug.Log("✓ PostTestSetupFix found");
        }
        else
        {
            Debug.LogWarning("⚠️ PostTestSetupFix not found in scene - checking if script exists...");
            
            // Check if the script file exists
            var scriptType = System.Type.GetType("PostTestSetupFix");
            if (scriptType != null)
            {
                Debug.Log("✓ PostTestSetupFix script exists but not in scene");
            }
            else
            {
                Debug.LogError("❌ PostTestSetupFix script not found - check if file exists");
            }
        }
        
        // Check camera
        var camera = Camera.main;
        if (camera != null)
        {
            Debug.Log("✓ Main camera found");
        }
        else
        {
            Debug.LogWarning("⚠️ No main camera found");
        }
        
        // Check canvases
        var canvases = FindObjectsOfType<Canvas>();
        Debug.Log($"Found {canvases.Length} Canvas components");
        
        // Check for VolumeCamera
        var allMonoBehaviours = FindObjectsOfType<MonoBehaviour>();
        bool foundVolumeCamera = false;
        foreach (var mb in allMonoBehaviours)
        {
            if (mb.GetType().Name.Contains("VolumeCamera"))
            {
                foundVolumeCamera = true;
                break;
            }
        }
        
        if (foundVolumeCamera)
        {
            Debug.Log("✓ VolumeCamera found");
        }
        else
        {
            Debug.LogError("❌ No VolumeCamera found - required for visionOS");
        }
        
        Debug.Log("=== VISION CHECK COMPLETED ===");
    }
    
    [ContextMenu("Emergency Recovery")]
    public void EmergencyRecovery()
    {
        var manager = FindObjectOfType<Moonphases_Manager>();
        if (manager != null)
        {
            manager.EmergencyRecovery();
            Debug.Log("Emergency recovery executed");
        }
        else
        {
            Debug.LogError("Cannot find Moonphases_Manager");
        }
    }
    
    [ContextMenu("Clear Unity Cache")]
    public void ClearUnityCache()
    {
        Debug.Log("To clear Unity cache:");
        Debug.Log("1. Close Unity");
        Debug.Log("2. Delete the 'Library' folder");
        Debug.Log("3. Reopen Unity (it will rebuild the cache)");
        Debug.Log("This will fix the missing script references.");
    }
} 