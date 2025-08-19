using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple script to fix the canvas background color
/// Attach this to your PostTestCanvas if the background is still white
/// </summary>
public class CanvasBackgroundFix : MonoBehaviour
{
    [Header("Background Settings")]
    [SerializeField] private Color backgroundColor = new Color32(30, 50, 30, 255); // Dark green/blackboard color
    
    void Start()
    {
        FixBackground();
    }
    
    [ContextMenu("Fix Background")]
    public void FixBackground()
    {
        Debug.Log("[CanvasBackgroundFix] Fixing canvas background...");
        
        // Fix the main canvas background
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            // Set canvas background color
            var canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            
            // Force add an Image component to the canvas if it doesn't have one
            var canvasImage = GetComponent<Image>();
            if (canvasImage == null)
            {
                canvasImage = gameObject.AddComponent<Image>();
                Debug.Log("[CanvasBackgroundFix] Added Image component to canvas");
            }
            canvasImage.color = backgroundColor;
            Debug.Log($"[CanvasBackgroundFix] Set canvas background to: {backgroundColor}");
        }
        
        // Find and fix all background images
        Image[] allImages = GetComponentsInChildren<Image>(true);
        foreach (var img in allImages)
        {
            // Skip buttons
            if (img.GetComponent<Button>() != null)
                continue;
                
            // Check if this looks like a background (has children or is named like a panel)
            bool isBackground = img.transform.childCount > 0 || 
                               img.name.ToLower().Contains("panel") || 
                               img.name.ToLower().Contains("background") ||
                               img.name.ToLower().Contains("blackboard") ||
                               img.name.ToLower().Contains("canvas") ||
                               img.rectTransform.sizeDelta.x > 100 || // Large images are likely backgrounds
                               img.rectTransform.sizeDelta.y > 100;
            
            if (isBackground)
            {
                img.color = backgroundColor;
                Debug.Log($"[CanvasBackgroundFix] Set {img.name} to background color: {backgroundColor}");
            }
        }
        
        Debug.Log("[CanvasBackgroundFix] Background fix completed!");
    }
    
    // Call this method from the inspector or other scripts
    [ContextMenu("Force Background Fix")]
    public void ForceBackgroundFix()
    {
        Debug.Log("[CanvasBackgroundFix] Force fixing background...");
        
        // Force set ALL images to background color (except buttons)
        Image[] allImages = GetComponentsInChildren<Image>(true);
        foreach (var img in allImages)
        {
            if (img.GetComponent<Button>() == null)
            {
                img.color = backgroundColor;
                Debug.Log($"[CanvasBackgroundFix] FORCED {img.name} to background color");
            }
        }
    }
} 