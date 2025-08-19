using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

/// <summary>
/// Diagnostic script to identify Vision Pro specific issues
/// Updated for visionOS platform version 2.5 compatibility
/// </summary>
public class VisionProDiagnostics : MonoBehaviour
{
    [Header("Diagnostic Settings")]
    public bool runDiagnosticsOnStart = true;
    public bool logToFile = true;
    
    [Header("Test Results")]
    public bool xrDeviceActive = false;
    public bool polySpatialAvailable = false;
    public bool cameraConfigured = false;
    public bool materialsCompatible = false;
    public bool canvasConfigured = false;
    
    private List<string> diagnosticLog = new List<string>();
    
    void Start()
    {
        if (runDiagnosticsOnStart)
        {
            RunFullDiagnostics();
        }
    }
    
    [ContextMenu("Run Full Diagnostics")]
    public void RunFullDiagnostics()
    {
        diagnosticLog.Clear();
        diagnosticLog.Add("=== Vision Pro Diagnostics Started (v2.5) ===");
        
        CheckXRDevice();
        CheckPolySpatial();
        CheckCamera();
        CheckMaterials();
        CheckCanvas();
        CheckPerformance();
        
        diagnosticLog.Add("=== Vision Pro Diagnostics Completed ===");
        
        // Log all results
        foreach (var log in diagnosticLog)
        {
            Debug.Log(log);
        }
        
        // Save to file if enabled
        if (logToFile)
        {
            SaveDiagnosticsToFile();
        }
    }
    
    void CheckXRDevice()
    {
        diagnosticLog.Add("--- XR Device Check (v2.5) ---");
        
        xrDeviceActive = XRSettings.isDeviceActive;
        diagnosticLog.Add($"XR Device Active: {xrDeviceActive}");
        
        if (xrDeviceActive)
        {
            diagnosticLog.Add($"XR Device Name: {XRSettings.loadedDeviceName}");
            diagnosticLog.Add($"XR Device Active: {xrDeviceActive}");
        }
        else
        {
            diagnosticLog.Add("WARNING: XR device not active - this may cause issues on Vision Pro");
        }
        
        // Check for XR Interaction Toolkit (using reflection to avoid compilation issues)
        try
        {
            var xrInteractionManagerType = System.Type.GetType("UnityEngine.XR.Interaction.Toolkit.InteractionManager");
            if (xrInteractionManagerType != null)
            {
                var xrInteractionManager = FindObjectOfType(xrInteractionManagerType);
                if (xrInteractionManager != null)
                {
                    diagnosticLog.Add("XR Interaction Manager: Found");
                }
                else
                {
                    diagnosticLog.Add("WARNING: XR Interaction Manager not found");
                }
            }
            else
            {
                diagnosticLog.Add("WARNING: XR Interaction Toolkit not available");
            }
        }
        catch (System.Exception e)
        {
            diagnosticLog.Add($"WARNING: Could not check XR Interaction Manager: {e.Message}");
        }
    }
    
    void CheckPolySpatial()
    {
        diagnosticLog.Add("--- PolySpatial Check (v2.5) ---");
        
        // Check if PolySpatial types are available
        var polySpatialType = System.Type.GetType("Unity.PolySpatial.PolySpatialSettings");
        polySpatialAvailable = polySpatialType != null;
        
        diagnosticLog.Add($"PolySpatial Available: {polySpatialAvailable}");
        
        if (polySpatialAvailable)
        {
            diagnosticLog.Add("PolySpatial package is installed");
        }
        else
        {
            diagnosticLog.Add("ERROR: PolySpatial package not found - required for Vision Pro");
        }
        
        // Check PolySpatial settings
        var polySpatialSettings = Resources.Load("PolySpatialSettings");
        if (polySpatialSettings != null)
        {
            diagnosticLog.Add("PolySpatial Settings: Found");
        }
        else
        {
            diagnosticLog.Add("WARNING: PolySpatial Settings not found in Resources");
        }
    }
    
    void CheckCamera()
    {
        diagnosticLog.Add("--- Camera Check (v2.5) ---");
        
        var mainCamera = Camera.main;
        if (mainCamera != null)
        {
            diagnosticLog.Add($"Main Camera: {mainCamera.name}");
            diagnosticLog.Add($"Camera Position: {mainCamera.transform.position}");
            diagnosticLog.Add($"Camera Clear Flags: {mainCamera.clearFlags}");
            diagnosticLog.Add($"Camera Background Color: {mainCamera.backgroundColor}");
            diagnosticLog.Add($"Camera Field of View: {mainCamera.fieldOfView}");
            
            cameraConfigured = mainCamera.clearFlags != CameraClearFlags.Nothing;
            
            if (cameraConfigured)
            {
                diagnosticLog.Add("Camera: Properly configured");
            }
            else
            {
                diagnosticLog.Add("WARNING: Camera clear flags set to Nothing - may cause rendering issues");
            }
        }
        else
        {
            diagnosticLog.Add("ERROR: No main camera found");
            cameraConfigured = false;
        }
    }
    
    void CheckMaterials()
    {
        diagnosticLog.Add("--- Materials Check (v2.5) ---");
        
        var allRenderers = FindObjectsOfType<Renderer>();
        diagnosticLog.Add($"Total Renderers: {allRenderers.Length}");
        
        int compatibleMaterials = 0;
        int incompatibleMaterials = 0;
        
        foreach (var renderer in allRenderers)
        {
            foreach (var material in renderer.materials)
            {
                if (material != null && material.shader != null)
                {
                    string shaderName = material.shader.name;
                    if (shaderName.Contains("Universal Render Pipeline") || 
                        shaderName.Contains("URP") ||
                        shaderName.Contains("Unlit") ||
                        shaderName.Contains("Standard"))
                    {
                        compatibleMaterials++;
                    }
                    else
                    {
                        incompatibleMaterials++;
                        diagnosticLog.Add($"WARNING: Potentially incompatible shader on {renderer.name}: {shaderName}");
                    }
                }
            }
        }
        
        diagnosticLog.Add($"Compatible Materials: {compatibleMaterials}");
        diagnosticLog.Add($"Potentially Incompatible Materials: {incompatibleMaterials}");
        
        materialsCompatible = incompatibleMaterials == 0;
        
        if (materialsCompatible)
        {
            diagnosticLog.Add("Materials: All appear compatible");
        }
        else
        {
            diagnosticLog.Add("WARNING: Some materials may be incompatible with Vision Pro");
        }
    }
    
    void CheckCanvas()
    {
        diagnosticLog.Add("--- Canvas Check (v2.5) ---");
        
        var allCanvases = FindObjectsOfType<Canvas>();
        diagnosticLog.Add($"Total Canvases: {allCanvases.Length}");
        
        int properlyConfigured = 0;
        
        foreach (var canvas in allCanvases)
        {
            diagnosticLog.Add($"Canvas: {canvas.name}");
            diagnosticLog.Add($"  Render Mode: {canvas.renderMode}");
            diagnosticLog.Add($"  World Camera: {(canvas.worldCamera != null ? canvas.worldCamera.name : "None")}");
            diagnosticLog.Add($"  Active: {canvas.gameObject.activeInHierarchy}");
            
            if (canvas.renderMode == RenderMode.WorldSpace && canvas.worldCamera != null)
            {
                properlyConfigured++;
            }
            else
            {
                diagnosticLog.Add($"  WARNING: Canvas {canvas.name} may not be properly configured for Vision Pro");
            }
        }
        
        canvasConfigured = properlyConfigured == allCanvases.Length;
        
        if (canvasConfigured)
        {
            diagnosticLog.Add("Canvases: All properly configured");
        }
        else
        {
            diagnosticLog.Add("WARNING: Some canvases may not be properly configured for Vision Pro");
        }
    }
    
    void CheckPerformance()
    {
        diagnosticLog.Add("--- Performance Check (v2.5) ---");
        
        var allGameObjects = FindObjectsOfType<GameObject>();
        diagnosticLog.Add($"Total GameObjects: {allGameObjects.Length}");
        
        var allMeshRenderers = FindObjectsOfType<MeshRenderer>();
        diagnosticLog.Add($"Total Mesh Renderers: {allMeshRenderers.Length}");
        
        var allLights = FindObjectsOfType<Light>();
        diagnosticLog.Add($"Total Lights: {allLights.Length}");
        
        // Check for potential performance issues
        if (allGameObjects.Length > 1000)
        {
            diagnosticLog.Add("WARNING: Large number of GameObjects may impact performance");
        }
        
        if (allMeshRenderers.Length > 100)
        {
            diagnosticLog.Add("WARNING: Large number of Mesh Renderers may impact performance");
        }
        
        if (allLights.Length > 10)
        {
            diagnosticLog.Add("WARNING: Large number of Lights may impact performance");
        }
        
        // Check frame rate
        float fps = 1.0f / Time.deltaTime;
        diagnosticLog.Add($"Current FPS: {fps:F1}");
        
        if (fps < 60)
        {
            diagnosticLog.Add("WARNING: Low frame rate detected");
        }
    }
    
    void SaveDiagnosticsToFile()
    {
        string logPath = Application.persistentDataPath + "/visionpro_diagnostics_v2.5.txt";
        string logContent = string.Join("\n", diagnosticLog);
        
        try
        {
            System.IO.File.WriteAllText(logPath, logContent);
            Debug.Log($"Diagnostics saved to: {logPath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save diagnostics: {e.Message}");
        }
    }
    
    [ContextMenu("Get Summary")]
    public void GetSummary()
    {
        Debug.Log("=== VISION PRO DIAGNOSTICS SUMMARY (v2.5) ===");
        Debug.Log($"XR Device Active: {xrDeviceActive}");
        Debug.Log($"PolySpatial Available: {polySpatialAvailable}");
        Debug.Log($"Camera Configured: {cameraConfigured}");
        Debug.Log($"Materials Compatible: {materialsCompatible}");
        Debug.Log($"Canvas Configured: {canvasConfigured}");
        
        if (!xrDeviceActive || !polySpatialAvailable || !cameraConfigured || !materialsCompatible || !canvasConfigured)
        {
            Debug.LogWarning("Some issues detected - check full diagnostics for details");
        }
        else
        {
            Debug.Log("All basic checks passed");
        }
    }
} 