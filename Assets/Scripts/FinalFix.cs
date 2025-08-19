using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Final fix script to resolve the remaining Unity issues
/// </summary>
public class FinalFix : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== FINAL FIX STARTED ===");
        FixRemainingIssues();
    }
    
    [ContextMenu("Fix Remaining Issues")]
    public void FixRemainingIssues()
    {
        Debug.Log("Fixing remaining Unity issues...");
        
        FixMissingScripts();
        FixXRConfiguration();
        FixLunarPhaseQuestionManager();
        
        Debug.Log("=== FINAL FIX COMPLETED ===");
    }
    
    private void FixMissingScripts()
    {
        Debug.Log("Fixing missing script references...");
        
        var allObjects = FindObjectsOfType<GameObject>();
        int missingCount = 0;
        
        foreach (var obj in allObjects)
        {
            var components = obj.GetComponents<Component>();
            foreach (var component in components)
            {
                if (component == null)
                {
                    missingCount++;
                    Debug.Log($"Found missing script on: {obj.name}");
                }
            }
        }
        
        if (missingCount == 0)
        {
            Debug.Log("✓ No missing script references found");
        }
        else
        {
            Debug.LogWarning($"Found {missingCount} missing script references");
            Debug.Log("MANUAL FIX REQUIRED:");
            Debug.Log("1. In the Hierarchy, find GameObjects with red 'Missing Script' components");
            Debug.Log("2. Select each GameObject and in the Inspector, click the gear icon next to 'Missing Script'");
            Debug.Log("3. Select 'Remove Component' to remove the missing script reference");
        }
    }
    
    private void FixXRConfiguration()
    {
        Debug.Log("Fixing XR configuration...");
        
        // Check if XR is enabled
        if (UnityEngine.XR.XRSettings.enabled)
        {
            Debug.Log("✓ XR is enabled");
        }
        else
        {
            Debug.LogWarning("⚠️ XR is not enabled");
            Debug.Log("MANUAL FIX REQUIRED:");
            Debug.Log("1. Go to Edit → Project Settings → XR Plug-in Management");
            Debug.Log("2. Check 'VisionOS' under Plug-in Providers");
            Debug.Log("3. Go to Edit → Project Settings → XR → VisionOS Settings");
            Debug.Log("4. Ensure settings are properly configured");
        }
        
        // Check for XR loaders
        var loaders = UnityEngine.XR.Management.XRGeneralSettings.Instance?.Manager?.activeLoaders;
        if (loaders != null && loaders.Count > 0)
        {
            Debug.Log($"✓ Found {loaders.Count} XR loader(s)");
        }
        else
        {
            Debug.LogWarning("⚠️ No XR loaders found");
            Debug.Log("MANUAL FIX REQUIRED:");
            Debug.Log("1. Go to Edit → Project Settings → XR Plug-in Management");
            Debug.Log("2. Add 'VisionOS' loader under Plug-in Providers");
        }
    }
    
    private void FixLunarPhaseQuestionManager()
    {
        Debug.Log("Fixing LunarPhaseQuestionManager...");
        
        var lunarManager = FindObjectOfType<LunarPhaseQuestionManager>();
        if (lunarManager != null)
        {
            Debug.Log("✓ LunarPhaseQuestionManager found");
            
            // Check and fix questionText
            if (lunarManager.questionText == null)
            {
                Debug.LogWarning("⚠️ questionText is null in LunarPhaseQuestionManager");
                Debug.Log("Attempting to auto-assign questionText...");
                
                // Try to find a TextMeshProUGUI component
                var textComponent = lunarManager.GetComponentInChildren<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    lunarManager.questionText = textComponent;
                    Debug.Log("✓ Auto-assigned questionText");
                }
                else
                {
                    Debug.LogError("❌ No TextMeshProUGUI found - manual assignment required");
                    Debug.Log("MANUAL FIX REQUIRED:");
                    Debug.Log("1. Select the LunarPhaseQuestionManager GameObject");
                    Debug.Log("2. In the Inspector, assign a TextMeshProUGUI component to the 'Question Text' field");
                }
            }
            else
            {
                Debug.Log("✓ questionText is assigned");
            }
            
            // Check and fix phaseImage
            if (lunarManager.phaseImage == null)
            {
                Debug.LogWarning("⚠️ phaseImage is null in LunarPhaseQuestionManager");
                Debug.Log("Attempting to auto-assign phaseImage...");
                
                // Try to find an Image component
                var imageComponent = lunarManager.GetComponentInChildren<Image>();
                if (imageComponent != null)
                {
                    lunarManager.phaseImage = imageComponent;
                    Debug.Log("✓ Auto-assigned phaseImage");
                }
                else
                {
                    Debug.LogError("❌ No Image component found - manual assignment required");
                    Debug.Log("MANUAL FIX REQUIRED:");
                    Debug.Log("1. Select the LunarPhaseQuestionManager GameObject");
                    Debug.Log("2. In the Inspector, assign an Image component to the 'Phase Image' field");
                }
            }
            else
            {
                Debug.Log("✓ phaseImage is assigned");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ LunarPhaseQuestionManager not found in scene");
        }
    }
    
    [ContextMenu("Create UI Components")]
    public void CreateUIComponents()
    {
        Debug.Log("Creating UI components for LunarPhaseQuestionManager...");
        
        var lunarManager = FindObjectOfType<LunarPhaseQuestionManager>();
        if (lunarManager != null)
        {
            // Create a Canvas if it doesn't exist
            var canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                var canvasGo = new GameObject("PostTestCanvas");
                canvas = canvasGo.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace;
                canvasGo.AddComponent<CanvasScaler>();
                canvasGo.AddComponent<GraphicRaycaster>();
                Debug.Log("✓ Created PostTestCanvas");
            }
            
            // Create question text
            if (lunarManager.questionText == null)
            {
                var textGo = new GameObject("QuestionText");
                textGo.transform.SetParent(canvas.transform);
                var textComponent = textGo.AddComponent<TextMeshProUGUI>();
                textComponent.text = "Question will appear here";
                textComponent.fontSize = 24;
                textComponent.color = Color.white;
                textComponent.alignment = TextAlignmentOptions.Center;
                lunarManager.questionText = textComponent;
                Debug.Log("✓ Created QuestionText");
            }
            
            // Create phase image
            if (lunarManager.phaseImage == null)
            {
                var imageGo = new GameObject("PhaseImage");
                imageGo.transform.SetParent(canvas.transform);
                var imageComponent = imageGo.AddComponent<Image>();
                imageComponent.color = Color.white;
                lunarManager.phaseImage = imageComponent;
                Debug.Log("✓ Created PhaseImage");
            }
            
            Debug.Log("UI components created successfully");
        }
        else
        {
            Debug.LogError("LunarPhaseQuestionManager not found - cannot create UI components");
        }
    }
    
    [ContextMenu("Check All Components")]
    public void CheckAllComponents()
    {
        Debug.Log("=== COMPONENT CHECK ===");
        
        var lunarManager = FindObjectOfType<LunarPhaseQuestionManager>();
        if (lunarManager != null)
        {
            Debug.Log($"LunarPhaseQuestionManager: ✓ Found");
            Debug.Log($"questionText: {(lunarManager.questionText != null ? "✓ Assigned" : "❌ NULL")}");
            Debug.Log($"phaseImage: {(lunarManager.phaseImage != null ? "✓ Assigned" : "❌ NULL")}");
            Debug.Log($"moonDragger: {(lunarManager.moonDragger != null ? "✓ Assigned" : "❌ NULL")}");
            Debug.Log($"feedbackManager: {(lunarManager.feedbackManager != null ? "✓ Assigned" : "❌ NULL")}");
        }
        else
        {
            Debug.LogError("LunarPhaseQuestionManager: ❌ Not found");
        }
        
        var postTest = FindObjectOfType<PostTestSetupFix>();
        Debug.Log($"PostTestSetupFix: {(postTest != null ? "✓ Found" : "❌ Not found")}");
        
        var earth = GameObject.Find("Earth") ?? GameObject.Find("PlanetSystem");
        Debug.Log($"Earth: {(earth != null ? "✓ Found" : "❌ Not found")}");
        
        Debug.Log("=== COMPONENT CHECK COMPLETED ===");
    }
} 