using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Debug script to help identify white screen issues
/// Attach this to any GameObject in the scene to monitor the app state and UI components
/// </summary>
public class DebugWhiteScreen : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] private bool enableDebugLogging = true;
    [SerializeField] private bool logEveryFrame = false;
    [SerializeField] private bool showUIStatus = true;
    
    private Moonphases_Manager moonphasesManager;
    private Canvas[] allCanvases;
    private GameObject[] allUIElements;
    
    private void Start()
    {
        if (enableDebugLogging)
        {
            Debug.Log("[DebugWhiteScreen] Debug script started");
            
            // Find the main manager
            moonphasesManager = FindObjectOfType<Moonphases_Manager>();
            if (moonphasesManager != null)
            {
                Debug.Log("[DebugWhiteScreen] Found Moonphases_Manager");
            }
            else
            {
                Debug.LogError("[DebugWhiteScreen] Moonphases_Manager not found!");
            }
            
            // Find all canvases
            allCanvases = FindObjectsOfType<Canvas>();
            Debug.Log($"[DebugWhiteScreen] Found {allCanvases.Length} canvases in scene");
            
            // Find all UI elements
            allUIElements = FindObjectsOfType<GameObject>();
            Debug.Log($"[DebugWhiteScreen] Found {allUIElements.Length} total GameObjects in scene");
            
            // Log all UI elements with "Menu" in the name
            Debug.Log("[DebugWhiteScreen] UI Elements with 'Menu' in name:");
            foreach (var obj in allUIElements)
            {
                if (obj.name.ToLower().Contains("menu"))
                {
                    Debug.Log($"[DebugWhiteScreen] - {obj.name} (active: {obj.activeInHierarchy})");
                }
            }
        }
    }
    
    private void Update()
    {
        if (!enableDebugLogging) return;
        
        if (logEveryFrame && Time.frameCount % 60 == 0) // Log every 60 frames (once per second at 60fps)
        {
            LogCurrentState();
        }
    }
    
    [ContextMenu("Log Current State")]
    public void LogCurrentState()
    {
        if (!enableDebugLogging) return;
        
        Debug.Log("=== DEBUG WHITE SCREEN STATE ===");
        
        // Log app state
        if (moonphasesManager != null)
        {
            // Use reflection to get the private appState field
            var appStateField = typeof(Moonphases_Manager).GetField("appState", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (appStateField != null)
            {
                var appState = appStateField.GetValue(moonphasesManager);
                Debug.Log($"[DebugWhiteScreen] Current AppState: {appState}");
            }
        }
        
        // Log canvas states
        Debug.Log("[DebugWhiteScreen] Canvas states:");
        foreach (var canvas in allCanvases)
        {
            if (canvas != null)
            {
                Debug.Log($"[DebugWhiteScreen] - Canvas '{canvas.name}': active={canvas.gameObject.activeInHierarchy}, enabled={canvas.enabled}, renderMode={canvas.renderMode}");
            }
        }
        
        // Log UI element states
        if (showUIStatus)
        {
            Debug.Log("[DebugWhiteScreen] UI Element states:");
            foreach (var obj in allUIElements)
            {
                if (obj != null && (obj.name.ToLower().Contains("menu") || obj.name.ToLower().Contains("canvas") || obj.name.ToLower().Contains("ui")))
                {
                    Debug.Log($"[DebugWhiteScreen] - {obj.name}: active={obj.activeInHierarchy}, hasCanvas={obj.GetComponent<Canvas>() != null}, hasImage={obj.GetComponent<Image>() != null}");
                }
            }
        }
        
        // Check for any visible UI elements
        bool anyUIVisible = false;
        foreach (var canvas in allCanvases)
        {
            if (canvas != null && canvas.gameObject.activeInHierarchy && canvas.enabled)
            {
                anyUIVisible = true;
                break;
            }
        }
        
        if (!anyUIVisible)
        {
            Debug.LogError("[DebugWhiteScreen] NO VISIBLE UI ELEMENTS FOUND! This is likely the cause of the white screen.");
        }
        else
        {
            Debug.Log("[DebugWhiteScreen] Some UI elements are visible");
        }
        
        Debug.Log("=== END DEBUG STATE ===");
    }
    
    [ContextMenu("Force Show All Menus")]
    public void ForceShowAllMenus()
    {
        Debug.Log("[DebugWhiteScreen] Force showing all menu elements...");
        
        foreach (var obj in allUIElements)
        {
            if (obj != null && obj.name.ToLower().Contains("menu"))
            {
                obj.SetActive(true);
                Debug.Log($"[DebugWhiteScreen] Activated {obj.name}");
            }
        }
    }
    
    [ContextMenu("Test Post Test Transition")]
    public void TestPostTestTransition()
    {
        Debug.Log("[DebugWhiteScreen] Testing post test transition...");
        
        // Find and activate the post test setup
        PostTestSetupFix postTestSetup = FindObjectOfType<PostTestSetupFix>();
        if (postTestSetup != null)
        {
            postTestSetup.OnMainAppComplete();
            Debug.Log("[DebugWhiteScreen] PostTestSetupFix.OnMainAppComplete() called");
        }
        else
        {
            Debug.LogError("[DebugWhiteScreen] PostTestSetupFix not found!");
        }
    }
} 