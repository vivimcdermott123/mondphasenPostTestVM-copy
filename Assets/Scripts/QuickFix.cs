using UnityEngine;

/// <summary>
/// Quick fix for white screen issues
/// </summary>
public class QuickFix : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== QUICK FIX STARTED ===");
        FixWhiteScreen();
    }
    
    [ContextMenu("Fix White Screen")]
    public void FixWhiteScreen()
    {
        Debug.Log("Applying quick white screen fix...");
        
        // 1. Fix all Canvas render modes
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        foreach (var canvas in allCanvases)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.gameObject.SetActive(true);
            Debug.Log($"Fixed canvas: {canvas.name}");
        }
        
        // 2. Ensure main camera is properly configured
        var mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.stereoTargetEye = StereoTargetEyeMask.Both;
            mainCamera.enabled = true;
            mainCamera.gameObject.SetActive(true);
            Debug.Log("Fixed main camera");
        }
        
        // 3. Activate all UI elements
        var menuMain = GameObject.Find("MenuMain");
        if (menuMain != null) menuMain.SetActive(true);
        
        var menuCore = GameObject.Find("MenuCore");
        if (menuCore != null) menuCore.SetActive(true);
        
        var menuExtended = GameObject.Find("MenuExtended");
        if (menuExtended != null) menuExtended.SetActive(true);
        
        Debug.Log("=== QUICK FIX COMPLETED ===");
    }
} 