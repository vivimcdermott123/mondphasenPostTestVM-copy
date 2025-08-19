using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the creation of a minimal debug scene
/// Updated for visionOS platform version 2.5 compatibility
/// </summary>
public class DebugSceneManager : MonoBehaviour
{
    [Header("Debug Scene Settings")]
    public bool createMinimalScene = false;
    public bool disableAllPostTestScripts = true;
    public bool useSimpleCamera = true;
    
    void Start()
    {
        if (createMinimalScene)
        {
            CreateMinimalDebugScene();
        }
    }
    
    void CreateMinimalDebugScene()
    {
        Debug.Log("[DebugSceneManager] Creating minimal debug scene (v2.5)...");
        
        // Disable all complex scripts
        if (disableAllPostTestScripts)
        {
            DisableComplexScripts();
        }
        
        // Setup simple camera
        if (useSimpleCamera)
        {
            SetupSimpleCamera();
        }
        
        // Add minimal test component
        var minimalTest = gameObject.AddComponent<MinimalTest>();
        minimalTest.enableBasicText = true;
        minimalTest.enableBackground = false;
        minimalTest.enable3DObjects = false;
        minimalTest.enableXRComponents = false;
        
        Debug.Log("[DebugSceneManager] Minimal debug scene created (v2.5)");
    }
    
    void DisableComplexScripts()
    {
        Debug.Log("[DebugSceneManager] Disabling complex scripts (v2.5)...");
        
        // Disable all PostTest scripts
        var scriptsToDisable = new string[] {
            "MoonDraggerTester",
            "MoonDragger",
            "LunarPhaseQuestionManager",
            "PostTestSetupFix",
            "BlackboardPostTestTheme",
            "CanvasBackgroundFix",
            "SimplePostTestSetup"
        };
        
        foreach (var scriptName in scriptsToDisable)
        {
            var components = FindObjectsOfType<MonoBehaviour>();
            foreach (var component in components)
            {
                if (component.GetType().Name == scriptName)
                {
                    component.enabled = false;
                    Debug.Log($"[DebugSceneManager] Disabled {scriptName} (v2.5)");
                }
            }
        }
    }
    
    void SetupSimpleCamera()
    {
        Debug.Log("[DebugSceneManager] Setting up simple camera (v2.5)...");
        
        // Find or create main camera
        var mainCamera = Camera.main;
        if (mainCamera == null)
        {
            var cameraObj = new GameObject("Main Camera");
            mainCamera = cameraObj.AddComponent<Camera>();
            cameraObj.tag = "MainCamera";
        }
        
        // Reset camera to simple settings
        mainCamera.clearFlags = CameraClearFlags.SolidColor;
        mainCamera.backgroundColor = Color.black;
        mainCamera.transform.position = new Vector3(0, 0, -10);
        mainCamera.transform.rotation = Quaternion.identity;
        mainCamera.fieldOfView = 60f;
        
        // Remove any complex camera components
        var audioListener = mainCamera.GetComponent<AudioListener>();
        if (audioListener != null)
        {
            DestroyImmediate(audioListener);
        }
        
        Debug.Log("[DebugSceneManager] Simple camera setup completed (v2.5)");
    }
    
    [ContextMenu("Create Minimal Scene")]
    public void CreateMinimalScene()
    {
        createMinimalScene = true;
        CreateMinimalDebugScene();
    }
    
    [ContextMenu("Enable Basic Text")]
    public void EnableBasicText()
    {
        var minimalTest = GetComponent<MinimalTest>();
        if (minimalTest != null)
        {
            minimalTest.EnableBasicText();
        }
    }
    
    [ContextMenu("Enable Background")]
    public void EnableBackground()
    {
        var minimalTest = GetComponent<MinimalTest>();
        if (minimalTest != null)
        {
            minimalTest.EnableBackground();
        }
    }
    
    [ContextMenu("Enable 3D Objects")]
    public void Enable3DObjects()
    {
        var minimalTest = GetComponent<MinimalTest>();
        if (minimalTest != null)
        {
            minimalTest.Enable3DObjects();
        }
    }
    
    [ContextMenu("Enable XR Components")]
    public void EnableXRComponents()
    {
        var minimalTest = GetComponent<MinimalTest>();
        if (minimalTest != null)
        {
            minimalTest.EnableXRComponents();
        }
    }
    
    [ContextMenu("Reset Scene")]
    public void ResetScene()
    {
        var minimalTest = GetComponent<MinimalTest>();
        if (minimalTest != null)
        {
            minimalTest.ResetAll();
        }
        
        // Re-enable all scripts
        var allComponents = FindObjectsOfType<MonoBehaviour>();
        foreach (var component in allComponents)
        {
            component.enabled = true;
        }
        
        Debug.Log("[DebugSceneManager] Scene reset to original state (v2.5)");
    }
} 