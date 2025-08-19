using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Sets up a minimal test scene for debugging Vision Pro issues
/// Updated for visionOS platform version 2.5 compatibility
/// </summary>
public class TestSceneSetup : MonoBehaviour
{
    [Header("Setup Options")]
    public bool setupOnStart = false;
    public bool createMinimalScene = true;
    public bool addDiagnostics = true;
    public bool addDebugManager = true;
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupTestScene();
        }
    }
    
    [ContextMenu("Setup Test Scene")]
    public void SetupTestScene()
    {
        Debug.Log("[TestSceneSetup] Setting up minimal test scene (v2.5)...");
        
        // Create a manager GameObject
        var manager = new GameObject("TestSceneManager");
        
        if (addDebugManager)
        {
            var debugManager = manager.AddComponent<DebugSceneManager>();
            debugManager.createMinimalScene = createMinimalScene;
            debugManager.disableAllPostTestScripts = true;
            debugManager.useSimpleCamera = true;
        }
        
        if (addDiagnostics)
        {
            var diagnostics = manager.AddComponent<VisionProDiagnostics>();
            diagnostics.runDiagnosticsOnStart = true;
            diagnostics.logToFile = true;
        }
        
        // Create a simple camera if none exists
        if (Camera.main == null)
        {
            var cameraObj = new GameObject("Main Camera");
            var camera = cameraObj.AddComponent<Camera>();
            cameraObj.tag = "MainCamera";
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
            camera.transform.position = new Vector3(0, 0, -10);
        }
        
        // Create a simple light if none exists
        if (FindObjectOfType<Light>() == null)
        {
            var lightObj = new GameObject("Directional Light");
            var light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1f;
            lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);
        }
        
        Debug.Log("[TestSceneSetup] Test scene setup completed (v2.5)");
        Debug.Log("[TestSceneSetup] Use the DebugSceneManager context menu to enable features gradually");
    }
    
    [ContextMenu("Create Hello World Scene")]
    public void CreateHelloWorldScene()
    {
        Debug.Log("[TestSceneSetup] Creating Hello World scene (v2.5)...");
        
        // Clear the scene
        var allObjects = FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            if (obj != this.gameObject)
            {
                DestroyImmediate(obj);
            }
        }
        
        // Create minimal setup
        var manager = new GameObject("HelloWorldManager");
        var minimalTest = manager.AddComponent<MinimalTest>();
        minimalTest.enableBasicText = true;
        minimalTest.enableBackground = false;
        minimalTest.enable3DObjects = false;
        minimalTest.enableXRComponents = false;
        
        // Create camera
        var cameraObj = new GameObject("Main Camera");
        var camera = cameraObj.AddComponent<Camera>();
        cameraObj.tag = "MainCamera";
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.black;
        camera.transform.position = new Vector3(0, 0, -10);
        
        Debug.Log("[TestSceneSetup] Hello World scene created (v2.5)");
        Debug.Log("[TestSceneSetup] This should show basic text on Vision Pro");
    }
    
    [ContextMenu("Reset to Original Scene")]
    public void ResetToOriginalScene()
    {
        Debug.Log("[TestSceneSetup] Resetting to original scene (v2.5)...");
        
        // Re-enable all scripts
        var allComponents = FindObjectsOfType<MonoBehaviour>();
        foreach (var component in allComponents)
        {
            component.enabled = true;
        }
        
        // Remove test objects
        var testObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (var obj in testObjects)
        {
            if (obj.name.Contains("Test") || obj.name.Contains("Debug") || obj.name.Contains("HelloWorld"))
            {
                DestroyImmediate(obj);
            }
        }
        
        Debug.Log("[TestSceneSetup] Scene reset to original state (v2.5)");
    }
    
    [ContextMenu("Save Test Scene")]
    public void SaveTestScene()
    {
        Debug.Log("[TestSceneSetup] Saving test scene (v2.5)...");
        
        // Save the current scene
        #if UNITY_EDITOR
        bool saved = UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        
        if (saved)
        {
            Debug.Log("[TestSceneSetup] Test scene saved successfully (v2.5)");
        }
        else
        {
            Debug.LogError("[TestSceneSetup] Failed to save test scene (v2.5)");
        }
        #else
        Debug.Log("[TestSceneSetup] Scene saving not available in build (v2.5)");
        #endif
    }
} 