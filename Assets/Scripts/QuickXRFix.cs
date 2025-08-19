using UnityEngine;

/// <summary>
/// Quick XR fix script
/// </summary>
public class QuickXRFix : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== QUICK XR FIX STARTED ===");
        FixXR();
    }
    
    [ContextMenu("Fix XR")]
    public void FixXR()
    {
        Debug.Log("Fixing XR configuration...");
        
        // Check if XR is enabled
        if (UnityEngine.XR.XRSettings.enabled)
        {
            Debug.Log("✅ XR is already enabled!");
        }
        else
        {
            Debug.LogWarning("⚠️ XR is not enabled");
            Debug.Log("MANUAL FIX REQUIRED:");
            Debug.Log("1. Go to Edit → Project Settings → XR Plug-in Management");
            Debug.Log("2. Check 'VisionOS' under Plug-in Providers");
            Debug.Log("3. Restart Unity");
        }
        
        Debug.Log("=== QUICK XR FIX COMPLETED ===");
    }
} 