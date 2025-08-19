using UnityEngine;

/// <summary>
/// Helper script to set up MoonDragger component
/// </summary>
public class MoonDraggerSetup : MonoBehaviour
{
    [ContextMenu("Setup MoonDragger")]
    public void SetupMoonDragger()
    {
        Debug.Log("=== SETTING UP MOON DRAGGER ===");
        
        // Find Moon_Body
        var moonBody = GameObject.Find("Moon_Body");
        if (moonBody == null)
        {
            Debug.LogError("Moon_Body not found! Please make sure it exists in the scene.");
            return;
        }
        
        Debug.Log($"Found Moon_Body: {moonBody.name}");
        
        // Check if MoonDragger already exists
        var existingMoonDragger = moonBody.GetComponent<MoonDragger>();
        if (existingMoonDragger != null)
        {
            Debug.Log("MoonDragger component already exists on Moon_Body");
            return;
        }
        
        // Add MoonDragger component
        var moonDragger = moonBody.AddComponent<MoonDragger>();
        Debug.Log("Added MoonDragger component to Moon_Body");
        
        // Find Earth_Body for the earthTransform reference
        var earthBody = GameObject.Find("Earth_Body");
        if (earthBody != null)
        {
            moonDragger.earthTransform = earthBody.transform;
            Debug.Log("Set earthTransform to Earth_Body");
        }
        else
        {
            Debug.LogWarning("Earth_Body not found - you'll need to set earthTransform manually");
        }
        
        // Find PostTestManager for the questionManager reference
        var postTestManager = FindObjectOfType<LunarPhaseQuestionManager>();
        if (postTestManager != null)
        {
            moonDragger.questionManager = postTestManager;
            Debug.Log("Set questionManager to PostTestManager");
        }
        else
        {
            Debug.LogWarning("LunarPhaseQuestionManager not found - you'll need to set questionManager manually");
        }
        
        Debug.Log("=== MOON DRAGGER SETUP COMPLETED ===");
    }
    
    [ContextMenu("Check MoonDragger Status")]
    public void CheckMoonDraggerStatus()
    {
        Debug.Log("=== CHECKING MOON DRAGGER STATUS ===");
        
        var moonBody = GameObject.Find("Moon_Body");
        if (moonBody == null)
        {
            Debug.LogError("Moon_Body not found!");
            return;
        }
        
        var moonDragger = moonBody.GetComponent<MoonDragger>();
        if (moonDragger == null)
        {
            Debug.LogError("MoonDragger component not found on Moon_Body!");
            return;
        }
        
        Debug.Log("✓ MoonDragger found on Moon_Body");
        Debug.Log($"  Earth Transform: {(moonDragger.earthTransform != null ? moonDragger.earthTransform.name : "NOT SET")}");
        Debug.Log($"  Question Manager: {(moonDragger.questionManager != null ? moonDragger.questionManager.name : "NOT SET")}");
        Debug.Log($"  Orbit Radius: {moonDragger.orbitRadius}");
        Debug.Log($"  Enable Pinch To Grab: {moonDragger.enablePinchToGrab}");
        Debug.Log($"  Constrain To Orbit: {moonDragger.constrainToOrbit}");
        
        Debug.Log("=== MOON DRAGGER STATUS CHECK COMPLETED ===");
    }
} 