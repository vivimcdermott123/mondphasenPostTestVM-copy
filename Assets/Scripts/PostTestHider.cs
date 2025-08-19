using UnityEngine;

/// <summary>
/// Ensures the post test panel is hidden until the main application finishes
/// </summary>
public class PostTestHider : MonoBehaviour
{
    void Start()
    {
        Debug.Log("[PostTestHider] Ensuring post test panel is hidden...");
        HidePostTestPanel();
    }
    
    [ContextMenu("Hide Post Test Panel")]
    public void HidePostTestPanel()
    {
        Debug.Log("[PostTestHider] Hiding all post test elements...");
        
        // Hide the main MenuPostTest GameObject
        var menuPostTest = GameObject.Find("MenuPostTest");
        if (menuPostTest != null)
        {
            menuPostTest.SetActive(false);
            Debug.Log("[PostTestHider] MenuPostTest hidden");
        }
        
        // Hide any post test canvases
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        foreach (var canvas in allCanvases)
        {
            if (canvas.name.ToLower().Contains("post") || 
                canvas.name.ToLower().Contains("test"))
            {
                canvas.gameObject.SetActive(false);
                Debug.Log($"[PostTestHider] Hidden post test canvas: {canvas.name}");
            }
        }
        
        // Hide any other post test related objects
        var postTestObjects = FindObjectsOfType<GameObject>();
        foreach (var obj in postTestObjects)
        {
            if (obj.name.ToLower().Contains("posttest") || 
                obj.name.ToLower().Contains("post test") ||
                obj.name.ToLower().Contains("post-test"))
            {
                if (obj != this.gameObject) // Don't hide this script's GameObject
                {
                    obj.SetActive(false);
                    Debug.Log($"[PostTestHider] Hidden post test object: {obj.name}");
                }
            }
        }
        
        Debug.Log("[PostTestHider] Post test panel hiding completed");
    }
    
    [ContextMenu("Show Post Test Panel")]
    public void ShowPostTestPanel()
    {
        Debug.Log("[PostTestHider] Showing post test panel...");
        
        // Show the main MenuPostTest GameObject
        var menuPostTest = GameObject.Find("MenuPostTest");
        if (menuPostTest != null)
        {
            menuPostTest.SetActive(true);
            Debug.Log("[PostTestHider] MenuPostTest shown");
        }
        
        // Find and activate PostTestSetupFix
        var postTestSetup = FindObjectOfType<PostTestSetupFix>();
        if (postTestSetup != null)
        {
            postTestSetup.OnMainAppComplete();
            Debug.Log("[PostTestHider] PostTestSetupFix activated");
        }
        
        Debug.Log("[PostTestHider] Post test panel showing completed");
    }
} 