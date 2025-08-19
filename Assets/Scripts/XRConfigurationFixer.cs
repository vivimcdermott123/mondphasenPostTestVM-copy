using UnityEngine;
using UnityEngine.XR.Management;

/// <summary>
/// Fixes XR configuration issues for Vision Pro
/// </summary>
public class XRConfigurationFixer : MonoBehaviour
{
    void Start()
    {
        FixXRConfiguration();
    }
    
    [ContextMenu("Fix XR Configuration")]
    public void FixXRConfiguration()
    {
        Debug.Log("=== FIXING XR CONFIGURATION ===");
        
        // Check current XR settings
        CheckCurrentXRSettings();
        
        // Try to fix XR configuration
        FixXRSettings();
        
        // Verify the fix
        VerifyXRFix();
        
        Debug.Log("=== XR CONFIGURATION FIX COMPLETED ===");
    }
    
    private void CheckCurrentXRSettings()
    {
        Debug.Log("Checking current XR settings...");
        
        // Check if XR is enabled
        if (UnityEngine.XR.XRSettings.enabled)
        {
            Debug.Log("✓ XR is enabled");
        }
        else
        {
            Debug.LogWarning("⚠️ XR is not enabled");
        }
        
        // Check XR General Settings
        var generalSettings = XRGeneralSettings.Instance;
        if (generalSettings != null)
        {
            Debug.Log("✓ XRGeneralSettings found");
            
            var manager = generalSettings.Manager;
            if (manager != null)
            {
                Debug.Log($"✓ XR Manager found with {manager.activeLoaders?.Count ?? 0} active loaders");
                
                if (manager.activeLoaders != null)
                {
                    foreach (var loader in manager.activeLoaders)
                    {
                        if (loader != null)
                        {
                            Debug.Log($"  - Active loader: {loader.name}");
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("⚠️ XR Manager is null");
            }
        }
        else
        {
            Debug.LogError("❌ XRGeneralSettings not found");
        }
        
        // Check if we're in the editor or runtime
        if (Application.isEditor)
        {
            Debug.Log("Running in Unity Editor");
        }
        else
        {
            Debug.Log("Running in build");
        }
    }
    
    private void FixXRSettings()
    {
        Debug.Log("Attempting to fix XR settings...");
        
        // Get XR General Settings
        var generalSettings = XRGeneralSettings.Instance;
        if (generalSettings == null)
        {
            Debug.LogError("❌ Cannot fix XR settings - XRGeneralSettings not found");
            Debug.Log("MANUAL FIX REQUIRED:");
            Debug.Log("1. Go to Edit → Project Settings → XR Plug-in Management");
            Debug.Log("2. Check 'VisionOS' under Plug-in Providers");
            Debug.Log("3. Go to Edit → Project Settings → XR → VisionOS Settings");
            Debug.Log("4. Ensure settings are properly configured");
            return;
        }
        
        var manager = generalSettings.Manager;
        if (manager == null)
        {
            Debug.LogError("❌ Cannot fix XR settings - XR Manager not found");
            return;
        }
        
        // Check if we have active loaders
        if (manager.activeLoaders == null || manager.activeLoaders.Count == 0)
        {
            Debug.LogWarning("⚠️ No active XR loaders found");
            Debug.Log("Attempting to initialize XR...");
            
            // Try to initialize XR
            try
            {
                manager.InitializeLoaderSync();
                Debug.Log("✓ XR initialized successfully");
                
                manager.StartSubsystems();
                Debug.Log("✓ XR subsystems started successfully");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Failed to initialize XR: {e.Message}");
                Debug.Log("MANUAL FIX REQUIRED:");
                Debug.Log("1. Go to Edit → Project Settings → XR Plug-in Management");
                Debug.Log("2. Check 'VisionOS' under Plug-in Providers");
                Debug.Log("3. Restart Unity");
            }
        }
        else
        {
            Debug.Log("✓ XR loaders are already active");
        }
    }
    
    private void VerifyXRFix()
    {
        Debug.Log("Verifying XR fix...");
        
        // Check if XR is now enabled
        if (UnityEngine.XR.XRSettings.enabled)
        {
            Debug.Log("✅ XR is now enabled!");
        }
        else
        {
            Debug.LogWarning("⚠️ XR is still not enabled");
        }
        
        // Check for active loaders
        var generalSettings = XRGeneralSettings.Instance;
        if (generalSettings?.Manager?.activeLoaders != null)
        {
            var loaderCount = generalSettings.Manager.activeLoaders.Count;
            if (loaderCount > 0)
            {
                Debug.Log($"✅ Found {loaderCount} active XR loader(s)");
                
                foreach (var loader in generalSettings.Manager.activeLoaders)
                {
                    if (loader != null)
                    {
                        Debug.Log($"  - {loader.name} is active");
                    }
                }
            }
            else
            {
                Debug.LogWarning("⚠️ Still no active XR loaders");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ XR Manager or loaders not available");
        }
    }
    
    [ContextMenu("Force XR Initialization")]
    public void ForceXRInitialization()
    {
        Debug.Log("=== FORCING XR INITIALIZATION ===");
        
        var generalSettings = XRGeneralSettings.Instance;
        if (generalSettings?.Manager != null)
        {
            // Stop any existing subsystems
            generalSettings.Manager.StopSubsystems();
            generalSettings.Manager.DeinitializeLoader();
            
            Debug.Log("Stopped existing XR subsystems");
            
            // Initialize again
            try
            {
                generalSettings.Manager.InitializeLoaderSync();
                Debug.Log("✓ XR re-initialized successfully");
                
                generalSettings.Manager.StartSubsystems();
                Debug.Log("✓ XR subsystems restarted successfully");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Failed to re-initialize XR: {e.Message}");
            }
        }
        else
        {
            Debug.LogError("❌ XRGeneralSettings or Manager not found");
        }
        
        Debug.Log("=== FORCE INITIALIZATION COMPLETED ===");
    }
    
    [ContextMenu("Show XR Status")]
    public void ShowXRStatus()
    {
        Debug.Log("=== XR STATUS REPORT ===");
        
        Debug.Log($"XR Enabled: {UnityEngine.XR.XRSettings.enabled}");
        Debug.Log($"XR Device: {UnityEngine.XR.XRSettings.loadedDeviceName}");
        Debug.Log($"Is Device Active: {UnityEngine.XR.XRSettings.isDeviceActive}");
        
        var generalSettings = XRGeneralSettings.Instance;
        if (generalSettings != null)
        {
            Debug.Log("✓ XRGeneralSettings found");
            
            var manager = generalSettings.Manager;
            if (manager != null)
            {
                Debug.Log($"Active Loaders: {manager.activeLoaders?.Count ?? 0}");
                
                if (manager.activeLoaders != null)
                {
                    foreach (var loader in manager.activeLoaders)
                    {
                        Debug.Log($"  - {loader?.name ?? "null"}");
                    }
                }
            }
            else
            {
                Debug.Log("❌ XR Manager not found");
            }
        }
        else
        {
            Debug.Log("❌ XRGeneralSettings not found");
        }
        
        Debug.Log("=== XR STATUS REPORT COMPLETED ===");
    }
} 