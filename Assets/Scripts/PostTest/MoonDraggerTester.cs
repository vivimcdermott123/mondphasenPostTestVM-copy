using UnityEngine;
using System.Linq;


/// <summary>
/// Test script to verify moon dragging functionality
/// Attach this to the moon GameObject to test pinching and dragging
/// </summary>
public class MoonDraggerTester : MonoBehaviour
{
    [Header("Test Settings")]
    public bool enableDebugLogging = true;
    public bool testKeyboardControls = true;
    
    private MoonDragger moonDragger;
    private Transform earthTransform;
    private float testAngle = 0f;
    
    void Start()
    {
        Debug.Log("[MoonDraggerTester] Starting initialization...");
        
        // Check if MoonDragger script exists
        var moonDraggerType = System.Type.GetType("MoonDragger");
        if (moonDraggerType == null)
        {
            Debug.LogError("[MoonDraggerTester] MoonDragger script class not found! Check for compilation errors.");
            return;
        }
        else
        {
            Debug.Log("[MoonDraggerTester] MoonDragger script class found");
        }
        
        // First try to get MoonDragger from this GameObject
        moonDragger = GetComponent<MoonDragger>();
        Debug.Log($"[MoonDraggerTester] MoonDragger on this GameObject: {(moonDragger != null ? "Found" : "Not found")}");
        
        // If not found on this GameObject, try to find it in the scene
        if (moonDragger == null)
        {
            Debug.Log("[MoonDraggerTester] Searching for MoonDragger in scene...");
            var allMoonDraggers = FindObjectsOfType<MoonDragger>();
            Debug.Log($"[MoonDraggerTester] Found {allMoonDraggers.Length} MoonDragger component(s) in scene");
            
            if (allMoonDraggers.Length > 0)
            {
                moonDragger = allMoonDraggers[0];
                Debug.Log($"[MoonDraggerTester] Using MoonDragger from: {moonDragger.gameObject.name}");
            }
            else
            {
                Debug.LogError("[MoonDraggerTester] No MoonDragger components found in scene!");
                Debug.Log("[MoonDraggerTester] Make sure MoonDragger is attached to the Moon_Body GameObject");
                Debug.Log("[MoonDraggerTester] Check that the MoonDragger script compiles without errors");
                return;
            }
        }
        else
        {
            Debug.Log("[MoonDraggerTester] Found MoonDragger component on this GameObject");
        }
        
        // Try to find earth transform
        Debug.Log("[MoonDraggerTester] Searching for Earth transform...");
        
        // Try multiple possible names for the Earth object
        string[] earthNames = { "Earth_Body", "Earth", "earth", "Planet", "planet" };
        earthTransform = null;
        
        foreach (var name in earthNames)
        {
            var found = GameObject.Find(name);
            if (found != null)
            {
                earthTransform = found.transform;
                Debug.Log($"[MoonDraggerTester] Found Earth transform: {found.name}");
                break;
            }
        }
        
        // If still not found, try to find by looking for objects with "earth" in the name
        if (earthTransform == null)
        {
            var allObjects = FindObjectsOfType<GameObject>();
            foreach (var obj in allObjects)
            {
                if (obj.name.ToLower().Contains("earth"))
                {
                    earthTransform = obj.transform;
                    Debug.Log($"[MoonDraggerTester] Found Earth transform by name search: {obj.name}");
                    break;
                }
            }
        }
        
        // If still not found, try to get it from the MoonDragger component
        if (earthTransform == null && moonDragger != null)
        {
            earthTransform = moonDragger.earthTransform;
            if (earthTransform != null)
            {
                Debug.Log($"[MoonDraggerTester] Found Earth transform from MoonDragger: {earthTransform.name}");
            }
        }
        
        if (earthTransform == null)
        {
            Debug.LogWarning("[MoonDraggerTester] Earth transform not found - some tests may not work");
            Debug.Log("[MoonDraggerTester] Searched for: Earth_Body, Earth, earth, Planet, planet");
        }
        else
        {
            Debug.Log($"[MoonDraggerTester] Successfully found Earth transform: {earthTransform.name}");
        }
        
        Debug.Log("[MoonDraggerTester] Moon dragging test initialized successfully");
    }
    
    void Update()
    {
        if (!testKeyboardControls) return;
        
        // Test keyboard controls for debugging
        float angleChange = 0f;
        
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            angleChange = -30f * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            angleChange = 30f * Time.deltaTime;
        }
        
        if (angleChange != 0f && earthTransform != null)
        {
            testAngle += angleChange;
            testAngle = (testAngle + 360f) % 360f;
            
            // Move moon to test position
            Vector3 newPosition = earthTransform.position + 
                new Vector3(Mathf.Cos(testAngle * Mathf.Deg2Rad), 0, Mathf.Sin(testAngle * Mathf.Deg2Rad)) * 2f;
            transform.position = newPosition;
            
            if (enableDebugLogging)
            {
                Debug.Log($"[MoonDraggerTester] Test angle: {testAngle:F1}°");
            }
        }
    }
    
    [ContextMenu("Test Moon Dragging")]
    public void TestMoonDragging()
    {
        Debug.Log("[MoonDraggerTester] Testing moon dragging functionality...");
        
        // Re-find the MoonDragger component in case it wasn't found during Start()
        if (moonDragger == null)
        {
            Debug.Log("[MoonDraggerTester] MoonDragger is null, searching for it...");
            moonDragger = FindObjectOfType<MoonDragger>();
            
            if (moonDragger == null)
            {
                Debug.LogError("[MoonDraggerTester] MoonDragger component not found!");
                Debug.Log("[MoonDraggerTester] Let me check what's in the scene...");
                
                // List all GameObjects to help debug
                var allObjects = FindObjectsOfType<GameObject>();
                Debug.Log($"[MoonDraggerTester] Total GameObjects in scene: {allObjects.Length}");
                
                // Look specifically for Moon_Body
                var moonBody = GameObject.Find("Moon_Body");
                if (moonBody != null)
                {
                    Debug.Log($"[MoonDraggerTester] Found Moon_Body: {moonBody.name} (active: {moonBody.activeInHierarchy})");
                    var moonDraggerOnBody = moonBody.GetComponent<MoonDragger>();
                    Debug.Log($"[MoonDraggerTester] MoonDragger on Moon_Body: {(moonDraggerOnBody != null ? "Found" : "NOT FOUND")}");
                }
                else
                {
                    Debug.LogError("[MoonDraggerTester] Moon_Body GameObject not found!");
                }
                
                return;
            }
            else
            {
                Debug.Log($"[MoonDraggerTester] Found MoonDragger on: {moonDragger.gameObject.name}");
            }
        }
        
        Debug.Log($"[MoonDraggerTester] MoonDragger found on: {moonDragger.gameObject.name}");
        
        // Test getting current angle
        try
        {
            float currentAngle = moonDragger.GetCurrentAngle();
            Debug.Log($"[MoonDraggerTester] Current moon angle: {currentAngle:F1}°");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[MoonDraggerTester] Error getting current angle: {e.Message}");
        }
        
        // Test XR components on the moon dragger's GameObject
        var grabInteractable = moonDragger.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable != null)
        {
            Debug.Log("[MoonDraggerTester] XRGrabInteractable component found and configured");
        }
        else
        {
            Debug.LogWarning("[MoonDraggerTester] XRGrabInteractable component not found on MoonDragger GameObject!");
        }
        
        // Test collider on the moon dragger's GameObject
        var collider = moonDragger.GetComponent<Collider>();
        if (collider != null)
        {
            Debug.Log($"[MoonDraggerTester] Collider found: {collider.GetType().Name}");
        }
        else
        {
            Debug.LogWarning("[MoonDraggerTester] No collider found on MoonDragger GameObject - moon may not be grabbable!");
        }
        
        // Test if earth transform is available
        if (earthTransform != null)
        {
            Debug.Log($"[MoonDraggerTester] Earth transform available: {earthTransform.name}");
        }
        else
        {
            Debug.LogWarning("[MoonDraggerTester] Earth transform not available - some functionality may not work");
        }
        
        Debug.Log("[MoonDraggerTester] Moon dragging test completed");
    }
    
    [ContextMenu("Reset Moon Position")]
    public void ResetMoonPosition()
    {
        if (earthTransform != null)
        {
            testAngle = 0f;
            Vector3 resetPosition = earthTransform.position + Vector3.right * 2f;
            transform.position = resetPosition;
            Debug.Log("[MoonDraggerTester] Moon position reset to 0°");
        }
    }
    
    [ContextMenu("Debug Scene Objects")]
    public void DebugSceneObjects()
    {
        Debug.Log("=== SCENE OBJECTS DEBUG ===");
        
        // List all GameObjects in scene
        var allObjects = FindObjectsOfType<GameObject>();
        Debug.Log($"Total GameObjects in scene: {allObjects.Length}");
        
        // Look for objects with "Moon" in the name
        var moonObjects = allObjects.Where(obj => obj.name.ToLower().Contains("moon")).ToArray();
        Debug.Log($"Objects with 'Moon' in name: {moonObjects.Length}");
        foreach (var obj in moonObjects)
        {
            Debug.Log($"  - {obj.name} (active: {obj.activeInHierarchy})");
            var moonDragger = obj.GetComponent<MoonDragger>();
            Debug.Log($"    MoonDragger component: {(moonDragger != null ? "Found" : "Not found")}");
        }
        
        // Look for objects with "Earth" in the name
        var earthObjects = allObjects.Where(obj => obj.name.ToLower().Contains("earth")).ToArray();
        Debug.Log($"Objects with 'Earth' in name: {earthObjects.Length}");
        foreach (var obj in earthObjects)
        {
            Debug.Log($"  - {obj.name} (active: {obj.activeInHierarchy})");
        }
        
        // Check for MoonDragger components
        var allMoonDraggers = FindObjectsOfType<MoonDragger>();
        Debug.Log($"Total MoonDragger components: {allMoonDraggers.Length}");
        foreach (var md in allMoonDraggers)
        {
            Debug.Log($"  - {md.gameObject.name} (active: {md.gameObject.activeInHierarchy})");
            Debug.Log($"    Earth Transform: {(md.earthTransform != null ? md.earthTransform.name : "NOT SET")}");
        }
        
        Debug.Log("=== END SCENE OBJECTS DEBUG ===");
    }
    
    [ContextMenu("Fix Earth Transform")]
    public void FixEarthTransform()
    {
        Debug.Log("=== FIXING EARTH TRANSFORM ===");
        
        if (moonDragger == null)
        {
            Debug.LogError("MoonDragger not found - cannot fix Earth transform");
            return;
        }
        
        // Try to find Earth_Body
        var earthBody = GameObject.Find("Earth_Body");
        if (earthBody != null)
        {
            moonDragger.earthTransform = earthBody.transform;
            earthTransform = earthBody.transform;
            Debug.Log($"Set Earth transform to: {earthBody.name}");
        }
        else
        {
            Debug.LogError("Earth_Body not found in scene!");
            Debug.Log("Please make sure the Earth object is named 'Earth_Body'");
        }
        
        Debug.Log("=== EARTH TRANSFORM FIX COMPLETED ===");
    }
} 