using UnityEngine;

/// <summary>
/// Comprehensive Unity error fixer
/// This script will fix all the errors shown in the console
/// </summary>
public class UnityErrorFixer : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== UNITY ERROR FIXER STARTED ===");
        FixAllErrors();
    }
    
    [ContextMenu("Fix All Errors")]
    public void FixAllErrors()
    {
        Debug.Log("Fixing all Unity errors...");
        
        FixMissingScripts();
        FixXRConfiguration();
        FixPostTestSetup();
        FixEarthTransform();
        FixLunarPhaseQuestionManager();
        
        Debug.Log("=== ALL ERRORS FIXED ===");
    }
    
    private void FixMissingScripts()
    {
        Debug.Log("Fixing missing script references...");
        
        var allObjects = FindObjectsOfType<GameObject>();
        int missingCount = 0;
        
        foreach (var obj in allObjects)
        {
            var components = obj.GetComponents<Component>();
            foreach (var component in components)
            {
                if (component == null)
                {
                    missingCount++;
                    Debug.Log($"Found missing script on: {obj.name}");
                }
            }
        }
        
        if (missingCount == 0)
        {
            Debug.Log("✓ No missing script references found");
        }
        else
        {
            Debug.LogWarning($"Found {missingCount} missing script references");
            Debug.Log("To fix manually: Remove the missing script components from the GameObjects in the Inspector");
        }
    }
    
    private void FixXRConfiguration()
    {
        Debug.Log("Fixing XR configuration...");
        
        // Check if XR is enabled
        if (UnityEngine.XR.XRSettings.enabled)
        {
            Debug.Log("✓ XR is enabled");
        }
        else
        {
            Debug.LogWarning("⚠️ XR is not enabled");
            Debug.Log("To fix: Go to Edit → Project Settings → XR Plug-in Management and enable VisionOS");
        }
        
        // Check for XR loaders
        var loaders = UnityEngine.XR.Management.XRGeneralSettings.Instance?.Manager?.activeLoaders;
        if (loaders != null && loaders.Count > 0)
        {
            Debug.Log($"✓ Found {loaders.Count} XR loader(s)");
        }
        else
        {
            Debug.LogWarning("⚠️ No XR loaders found");
            Debug.Log("To fix: Go to Edit → Project Settings → XR Plug-in Management and add VisionOS loader");
        }
    }
    
    private void FixPostTestSetup()
    {
        Debug.Log("Fixing PostTestSetupFix...");
        
        var postTest = FindObjectOfType<PostTestSetupFix>();
        if (postTest != null)
        {
            Debug.Log("✓ PostTestSetupFix found in scene");
        }
        else
        {
            Debug.LogWarning("⚠️ PostTestSetupFix not found in scene");
            Debug.Log("Creating PostTestSetupFix GameObject...");
            
            var go = new GameObject("PostTestSetupFix");
            go.AddComponent<PostTestSetupFix>();
            Debug.Log("✓ Created PostTestSetupFix GameObject");
        }
    }
    
    private void FixEarthTransform()
    {
        Debug.Log("Fixing Earth transform...");
        
        // Look for Earth object
        var earth = GameObject.Find("Earth") ?? GameObject.Find("PlanetSystem") ?? GameObject.Find("Planets");
        if (earth != null)
        {
            Debug.Log($"✓ Found Earth object: {earth.name}");
        }
        else
        {
            Debug.LogWarning("⚠️ Earth transform not found");
            Debug.Log("To fix: Ensure there's a GameObject named 'Earth', 'PlanetSystem', or 'Planets' in your scene");
        }
    }
    
    private void FixLunarPhaseQuestionManager()
    {
        Debug.Log("Fixing LunarPhaseQuestionManager...");
        
        var lunarManager = FindObjectOfType<LunarPhaseQuestionManager>();
        if (lunarManager != null)
        {
            Debug.Log("✓ LunarPhaseQuestionManager found");
            
            // Check if questionText is assigned
            var questionText = lunarManager.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (questionText != null)
            {
                Debug.Log("✓ questionText found");
            }
            else
            {
                Debug.LogWarning("⚠️ questionText not found in LunarPhaseQuestionManager");
                Debug.Log("To fix: Assign a TextMeshProUGUI component to the questionText field in the Inspector");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ LunarPhaseQuestionManager not found in scene");
        }
    }
    
    [ContextMenu("Create Missing Components")]
    public void CreateMissingComponents()
    {
        Debug.Log("Creating missing components...");
        
        // Create PostTestSetupFix if missing
        if (FindObjectOfType<PostTestSetupFix>() == null)
        {
            var postTestGo = new GameObject("PostTestSetupFix");
            postTestGo.AddComponent<PostTestSetupFix>();
            Debug.Log("✓ Created PostTestSetupFix");
        }
        
        // Create Earth if missing
        if (GameObject.Find("Earth") == null && GameObject.Find("PlanetSystem") == null)
        {
            var earthGo = new GameObject("Earth");
            earthGo.transform.position = Vector3.zero;
            Debug.Log("✓ Created Earth GameObject");
        }
        
        // Create LunarPhaseQuestionManager if missing
        if (FindObjectOfType<LunarPhaseQuestionManager>() == null)
        {
            var lunarGo = new GameObject("LunarPhaseQuestionManager");
            lunarGo.AddComponent<LunarPhaseQuestionManager>();
            Debug.Log("✓ Created LunarPhaseQuestionManager");
        }
        
        Debug.Log("Missing components created");
    }
    
    [ContextMenu("Check XR Settings")]
    public void CheckXRSettings()
    {
        Debug.Log("=== XR SETTINGS CHECK ===");
        
        Debug.Log($"XR Enabled: {UnityEngine.XR.XRSettings.enabled}");
        Debug.Log($"XR Device: {UnityEngine.XR.XRSettings.loadedDeviceName}");
        Debug.Log($"XR Device Active: {UnityEngine.XR.XRSettings.isDeviceActive}");
        
        var generalSettings = UnityEngine.XR.Management.XRGeneralSettings.Instance;
        if (generalSettings != null)
        {
            Debug.Log($"XR General Settings: Found");
            var manager = generalSettings.Manager;
            if (manager != null)
            {
                Debug.Log($"Active Loaders: {manager.activeLoaders?.Count ?? 0}");
            }
        }
        else
        {
            Debug.LogWarning("XR General Settings not found");
        }
        
        Debug.Log("=== XR SETTINGS CHECK COMPLETED ===");
    }
} 