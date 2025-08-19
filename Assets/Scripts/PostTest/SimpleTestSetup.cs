using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Simple test setup script that doesn't depend on other scripts
/// Updated for visionOS platform version 2.5 compatibility
/// </summary>
public class SimpleTestSetup : MonoBehaviour
{
    [Header("Test Settings")]
    public bool createHelloWorldOnStart = false;
    public bool enableDebugText = true;
    public bool enableBackground = false;
    public bool enable3DObjects = false;
    
    private TextMeshProUGUI debugText;
    private Canvas debugCanvas;
    private int frameCount = 0;
    private float startTime;
    
    void Start()
    {
        startTime = Time.time;
        
        if (createHelloWorldOnStart)
        {
            CreateHelloWorldScene();
        }
        
        if (enableDebugText)
        {
            SetupDebugText();
        }
    }
    
    void Update()
    {
        frameCount++;
        
        if (debugText != null && enableDebugText)
        {
            float elapsedTime = Time.time - startTime;
            debugText.text = $"Simple Test Running (v2.5)\n" +
                           $"Time: {elapsedTime:F1}s\n" +
                           $"Frames: {frameCount}\n" +
                           $"FPS: {1.0f / Time.deltaTime:F1}\n" +
                           $"Debug Text: {enableDebugText}\n" +
                           $"Background: {enableBackground}\n" +
                           $"3D Objects: {enable3DObjects}";
        }
    }
    
    [ContextMenu("Create Hello World Scene")]
    public void CreateHelloWorldScene()
    {
        Debug.Log("[SimpleTestSetup] Creating Hello World scene (v2.5)...");
        
        // Clear the scene (except this GameObject)
        var allObjects = FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            if (obj != this.gameObject)
            {
                DestroyImmediate(obj);
            }
        }
        
        // Create camera
        var cameraObj = new GameObject("Main Camera");
        var camera = cameraObj.AddComponent<Camera>();
        cameraObj.tag = "MainCamera";
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.black;
        camera.transform.position = new Vector3(0, 0, -10);
        
        // Create light
        var lightObj = new GameObject("Directional Light");
        var light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1f;
        lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);
        
        // Setup debug text
        SetupDebugText();
        
        Debug.Log("[SimpleTestSetup] Hello World scene created (v2.5)");
        Debug.Log("[SimpleTestSetup] This should show basic text on Vision Pro");
    }
    
    void SetupDebugText()
    {
        Debug.Log("[SimpleTestSetup] Setting up debug text (v2.5)...");
        
        // Create canvas if it doesn't exist
        if (debugCanvas == null)
        {
            var canvasObj = new GameObject("DebugCanvas");
            debugCanvas = canvasObj.AddComponent<Canvas>();
            debugCanvas.renderMode = RenderMode.WorldSpace;
            debugCanvas.worldCamera = Camera.main;
            
            // Add CanvasScaler
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            // Add GraphicRaycaster
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // Create text if it doesn't exist
        if (debugText == null)
        {
            var textObj = new GameObject("DebugText");
            textObj.transform.SetParent(debugCanvas.transform, false);
            
            debugText = textObj.AddComponent<TextMeshProUGUI>();
            debugText.text = "Simple Test Starting... (v2.5)";
            debugText.fontSize = 24;
            debugText.color = Color.white;
            debugText.alignment = TextAlignmentOptions.Center;
            
            // Position the text
            var rectTransform = debugText.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(400, 200);
        }
        
        Debug.Log("[SimpleTestSetup] Debug text setup completed (v2.5)");
    }
    
    [ContextMenu("Enable Background")]
    public void EnableBackground()
    {
        if (enableBackground) return;
        
        enableBackground = true;
        Debug.Log("[SimpleTestSetup] Creating background (v2.5)...");
        
        // Create a simple background
        var backgroundObj = new GameObject("Background");
        var backgroundRenderer = backgroundObj.AddComponent<MeshRenderer>();
        var backgroundMesh = backgroundObj.AddComponent<MeshFilter>();
        
        // Create a simple quad mesh
        var mesh = new Mesh();
        mesh.vertices = new Vector3[] {
            new Vector3(-5, -5, 0),
            new Vector3(5, -5, 0),
            new Vector3(5, 5, 0),
            new Vector3(-5, 5, 0)
        };
        mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        mesh.uv = new Vector2[] {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
        };
        mesh.RecalculateNormals();
        
        backgroundMesh.mesh = mesh;
        
        // Create a simple material
        var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.color = new Color(0.1f, 0.1f, 0.3f, 1.0f);
        backgroundRenderer.material = material;
        
        Debug.Log("[SimpleTestSetup] Background enabled (v2.5)");
    }
    
    [ContextMenu("Enable 3D Objects")]
    public void Enable3DObjects()
    {
        if (enable3DObjects) return;
        
        enable3DObjects = true;
        Debug.Log("[SimpleTestSetup] Creating 3D objects (v2.5)...");
        
        // Create a simple cube
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "TestCube";
        cube.transform.position = new Vector3(0, 0, 2);
        cube.transform.localScale = Vector3.one * 0.5f;
        
        // Create a simple sphere
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = "TestSphere";
        sphere.transform.position = new Vector3(2, 0, 2);
        sphere.transform.localScale = Vector3.one * 0.3f;
        
        Debug.Log("[SimpleTestSetup] 3D objects enabled (v2.5)");
    }
    
    [ContextMenu("Run Diagnostics")]
    public void RunDiagnostics()
    {
        Debug.Log("=== SIMPLE DIAGNOSTICS (v2.5) ===");
        
        // Check XR
        bool xrActive = UnityEngine.XR.XRSettings.isDeviceActive;
        Debug.Log($"XR Device Active: {xrActive}");
        
        if (xrActive)
        {
            Debug.Log($"XR Device: {UnityEngine.XR.XRSettings.loadedDeviceName}");
        }
        
        // Check camera
        var camera = Camera.main;
        if (camera != null)
        {
            Debug.Log($"Camera: {camera.name}");
            Debug.Log($"Camera Clear Flags: {camera.clearFlags}");
            Debug.Log($"Camera Background: {camera.backgroundColor}");
        }
        else
        {
            Debug.LogError("No main camera found!");
        }
        
        // Check materials
        var renderers = FindObjectsOfType<Renderer>();
        Debug.Log($"Total Renderers: {renderers.Length}");
        
        foreach (var renderer in renderers)
        {
            foreach (var material in renderer.materials)
            {
                if (material != null && material.shader != null)
                {
                    Debug.Log($"Material on {renderer.name}: {material.shader.name}");
                }
            }
        }
        
        // Check canvas
        var canvases = FindObjectsOfType<Canvas>();
        Debug.Log($"Total Canvases: {canvases.Length}");
        
        foreach (var canvas in canvases)
        {
            Debug.Log($"Canvas: {canvas.name}, Render Mode: {canvas.renderMode}");
        }
        
        Debug.Log("=== DIAGNOSTICS COMPLETED (v2.5) ===");
    }
    
    [ContextMenu("Reset Scene")]
    public void ResetScene()
    {
        Debug.Log("[SimpleTestSetup] Resetting scene (v2.5)...");
        
        // Re-enable all scripts
        var allComponents = FindObjectsOfType<MonoBehaviour>();
        foreach (var component in allComponents)
        {
            component.enabled = true;
        }
        
        Debug.Log("[SimpleTestSetup] Scene reset completed (v2.5)");
    }
} 