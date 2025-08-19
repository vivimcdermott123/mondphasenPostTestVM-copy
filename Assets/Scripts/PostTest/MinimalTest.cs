using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Minimal test script to debug the white dots issue
/// This script starts with basic functionality and gradually enables features
/// Updated for visionOS platform version 2.5 compatibility
/// </summary>
public class MinimalTest : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool enableBasicText = true;
    public bool enableBackground = false;
    public bool enable3DObjects = false;
    public bool enableXRComponents = false;
    
    [Header("UI References")]
    public TextMeshProUGUI debugText;
    public Canvas debugCanvas;
    
    private int frameCount = 0;
    private float startTime;
    
    void Start()
    {
        Debug.Log("[MinimalTest] Starting minimal test (v2.5)...");
        startTime = Time.time;
        
        // Step 1: Basic initialization
        if (enableBasicText)
        {
            SetupBasicText();
        }
        
        // Step 2: Background (if enabled)
        if (enableBackground)
        {
            SetupBackground();
        }
        
        // Step 3: 3D Objects (if enabled)
        if (enable3DObjects)
        {
            Setup3DObjects();
        }
        
        // Step 4: XR Components (if enabled)
        if (enableXRComponents)
        {
            SetupXRComponents();
        }
        
        Debug.Log("[MinimalTest] Minimal test initialization completed (v2.5)");
    }
    
    void Update()
    {
        frameCount++;
        
        if (debugText != null && enableBasicText)
        {
            float elapsedTime = Time.time - startTime;
            debugText.text = $"Minimal Test Running (v2.5)\n" +
                           $"Time: {elapsedTime:F1}s\n" +
                           $"Frames: {frameCount}\n" +
                           $"FPS: {1.0f / Time.deltaTime:F1}\n" +
                           $"Basic Text: {enableBasicText}\n" +
                           $"Background: {enableBackground}\n" +
                           $"3D Objects: {enable3DObjects}\n" +
                           $"XR Components: {enableXRComponents}";
        }
    }
    
    void SetupBasicText()
    {
        Debug.Log("[MinimalTest] Setting up basic text (v2.5)...");
        
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
            debugText.text = "Minimal Test Starting... (v2.5)";
            debugText.fontSize = 24;
            debugText.color = Color.white;
            debugText.alignment = TextAlignmentOptions.Center;
            
            // Position the text
            var rectTransform = debugText.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(400, 200);
        }
        
        Debug.Log("[MinimalTest] Basic text setup completed (v2.5)");
    }
    
    void SetupBackground()
    {
        Debug.Log("[MinimalTest] Setting up background (v2.5)...");
        
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
        
        Debug.Log("[MinimalTest] Background setup completed (v2.5)");
    }
    
    void Setup3DObjects()
    {
        Debug.Log("[MinimalTest] Setting up 3D objects (v2.5)...");
        
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
        
        Debug.Log("[MinimalTest] 3D objects setup completed (v2.5)");
    }
    
    void SetupXRComponents()
    {
        Debug.Log("[MinimalTest] Setting up XR components (v2.5)...");
        
        // Check if XR is available
        if (UnityEngine.XR.XRSettings.isDeviceActive)
        {
            Debug.Log("[MinimalTest] XR device is active (v2.5)");
        }
        else
        {
            Debug.LogWarning("[MinimalTest] XR device is not active (v2.5)");
        }
        
        // Add XR interaction components to test objects
        var testObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (var obj in testObjects)
        {
            if (obj.GetComponent<Collider>() == null)
            {
                obj.AddComponent<BoxCollider>();
            }
        }
        
        Debug.Log("[MinimalTest] XR components setup completed (v2.5)");
    }
    
    [ContextMenu("Enable Basic Text")]
    public void EnableBasicText()
    {
        enableBasicText = true;
        SetupBasicText();
        Debug.Log("[MinimalTest] Basic text enabled (v2.5)");
    }
    
    [ContextMenu("Enable Background")]
    public void EnableBackground()
    {
        enableBackground = true;
        SetupBackground();
        Debug.Log("[MinimalTest] Background enabled (v2.5)");
    }
    
    [ContextMenu("Enable 3D Objects")]
    public void Enable3DObjects()
    {
        enable3DObjects = true;
        Setup3DObjects();
        Debug.Log("[MinimalTest] 3D objects enabled (v2.5)");
    }
    
    [ContextMenu("Enable XR Components")]
    public void EnableXRComponents()
    {
        enableXRComponents = true;
        SetupXRComponents();
        Debug.Log("[MinimalTest] XR components enabled (v2.5)");
    }
    
    [ContextMenu("Reset All")]
    public void ResetAll()
    {
        enableBasicText = false;
        enableBackground = false;
        enable3DObjects = false;
        enableXRComponents = false;
        
        // Destroy all created objects
        var objectsToDestroy = GameObject.FindGameObjectsWithTag("Player");
        foreach (var obj in objectsToDestroy)
        {
            if (obj.name.Contains("Test") || obj.name.Contains("Debug"))
            {
                DestroyImmediate(obj);
            }
        }
        
        Debug.Log("[MinimalTest] All components reset (v2.5)");
    }
} 