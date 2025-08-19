using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This script fixes the post test setup and ensures proper connection between main app and post test.
/// 
/// INSTRUCTIONS:
/// 1. Create an empty GameObject in your scene (name it "PostTestManager" or similar)
/// 2. Attach this script to that empty GameObject
/// 3. Assign your PostTestCanvas in the Inspector
/// 4. The script will automatically find and connect all other components
/// </summary>
public class PostTestSetupFix : MonoBehaviour
{
    [Header("Main App Connection")]
    [SerializeField] private Moonphases_Manager mainAppManager;
    
    [Header("Post Test Components")]
    [SerializeField] private Canvas postTestCanvas;
    [SerializeField] private LunarPhaseQuestionManager questionManager;
    
    [Header("UI References to Auto-Assign")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button submitButton;
    [SerializeField] private TextMeshProUGUI startButtonText;
    [SerializeField] private TextMeshProUGUI submitButtonText;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI angleText;
    [SerializeField] private Image phaseImage;
    
    [Header("Post Test Logic Components")]
    [SerializeField] private MoonDragger moonDragger;
    [SerializeField] private FeedbackManager feedbackManager;
    [SerializeField] private PerformanceGrader performanceGrader;
    
    [Header("Theme")]
    [SerializeField] private BlackboardPostTestTheme blackboardTheme;
    
    private void Awake()
    {
        // Ensure post test canvas is initially hidden and stays hidden
        if (postTestCanvas != null)
        {
            postTestCanvas.gameObject.SetActive(false);
            Debug.Log("[PostTestSetupFix] Post test canvas hidden on startup");
        }
        
        // Also hide any other post test related objects
        HideAllPostTestElements();
    }
    
    private void HideAllPostTestElements()
    {
        // Hide the main MenuPostTest GameObject if it exists
        var menuPostTest = GameObject.Find("MenuPostTest");
        if (menuPostTest != null)
        {
            menuPostTest.SetActive(false);
            Debug.Log("[PostTestSetupFix] MenuPostTest hidden");
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
                    Debug.Log($"[PostTestSetupFix] Hidden post test object: {obj.name}");
                }
            }
        }
    }
    
    private void Start()
    {
        // Auto-assign references if not set
        AutoAssignReferences();
        
        // Set up button texts
        SetupButtonTexts();
        
        // Apply blackboard theme
        ApplyBlackboardTheme();
        
        // Set up main app connection
        SetupMainAppConnection();
        
        Debug.Log("[PostTestSetupFix] Post test setup completed successfully!");
    }
    
    private void AutoAssignReferences()
    {
        // Find the post test canvas if not assigned
        if (postTestCanvas == null)
        {
            postTestCanvas = FindObjectOfType<Canvas>();
            if (postTestCanvas != null && !postTestCanvas.name.ToLower().Contains("post"))
            {
                // Try to find a canvas with "post" in the name
                Canvas[] allCanvases = FindObjectsOfType<Canvas>();
                foreach (var canvas in allCanvases)
                {
                    if (canvas.name.ToLower().Contains("post"))
                    {
                        postTestCanvas = canvas;
                        break;
                    }
                }
            }
        }
        
        // Find question manager
        if (questionManager == null)
        {
            questionManager = FindObjectOfType<LunarPhaseQuestionManager>();
        }
        
        // Auto-find UI elements in the post test canvas
        if (postTestCanvas != null)
        {
            if (startButton == null)
            {
                startButton = postTestCanvas.transform.Find("StartButton")?.GetComponent<Button>();
            }
            
            if (submitButton == null)
            {
                submitButton = postTestCanvas.transform.Find("SubmitButton")?.GetComponent<Button>();
            }
            
            if (startButtonText == null && startButton != null)
            {
                startButtonText = startButton.GetComponentInChildren<TextMeshProUGUI>();
            }
            
            if (submitButtonText == null && submitButton != null)
            {
                submitButtonText = submitButton.GetComponentInChildren<TextMeshProUGUI>();
            }
            
            if (questionText == null)
            {
                questionText = postTestCanvas.transform.Find("QuestionText")?.GetComponent<TextMeshProUGUI>();
            }
            
            if (angleText == null)
            {
                angleText = postTestCanvas.transform.Find("AnglePanel/AngleText")?.GetComponent<TextMeshProUGUI>();
            }
            
            if (phaseImage == null)
            {
                phaseImage = postTestCanvas.transform.Find("PhaseImage")?.GetComponent<Image>();
            }
        }
        
        // Auto-find logic components
        if (moonDragger == null)
        {
            moonDragger = FindObjectOfType<MoonDragger>();
        }
        
        if (feedbackManager == null)
        {
            feedbackManager = FindObjectOfType<FeedbackManager>();
        }
        
        if (performanceGrader == null)
        {
            performanceGrader = FindObjectOfType<PerformanceGrader>();
        }
        
        // Assign references to question manager
        if (questionManager != null)
        {
            questionManager.moonDragger = moonDragger;
            questionManager.feedbackManager = feedbackManager;
            questionManager.performanceGrader = performanceGrader;
            questionManager.phaseImage = phaseImage;
            questionManager.questionText = questionText;
            questionManager.submitButton = submitButton;
            questionManager.angleText = angleText;
            questionManager.mainCanvas = postTestCanvas;
        }
    }
    
    private void SetupButtonTexts()
    {
        if (startButtonText != null)
        {
            startButtonText.text = "Start Post Test";
        }
        
        if (submitButtonText != null)
        {
            submitButtonText.text = "Submit Answer";
        }
        
        // Set up start button functionality
        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(StartPostTest);
        }
    }
    
    private void ApplyBlackboardTheme()
    {
        if (postTestCanvas != null)
        {
            blackboardTheme = postTestCanvas.GetComponent<BlackboardPostTestTheme>();
            if (blackboardTheme == null)
            {
                blackboardTheme = postTestCanvas.gameObject.AddComponent<BlackboardPostTestTheme>();
            }
            
            // Apply theme immediately
            blackboardTheme.ApplyTheme();
            
            // Also apply theme to any child panels that might need it
            Image[] childImages = postTestCanvas.GetComponentsInChildren<Image>(true);
            foreach (var img in childImages)
            {
                if (img.GetComponent<Button>() == null && 
                    (img.name.ToLower().Contains("panel") || img.name.ToLower().Contains("background")))
                {
                    // Set to dark green/blackboard color
                    img.color = new Color32(30, 50, 30, 255);
                    Debug.Log($"[PostTestSetupFix] Set {img.name} to blackboard color");
                }
            }
        }
    }
    
    private void SetupMainAppConnection()
    {
        if (mainAppManager == null)
        {
            mainAppManager = FindObjectOfType<Moonphases_Manager>();
        }
        
        // Subscribe to main app completion
        if (mainAppManager != null)
        {
            // The main app will automatically transition to MenuPostTest state
            // which should activate this canvas
            Debug.Log("[PostTestSetupFix] Connected to main app manager");
        }
        else
        {
            Debug.LogWarning("[PostTestSetupFix] Main app manager not found! Post test may not launch automatically.");
        }
    }
    
    public void StartPostTest()
    {
        Debug.Log("[PostTestSetupFix] Starting post test...");
        
        // Hide start button
        if (startButton != null)
        {
            startButton.gameObject.SetActive(false);
        }
        
        // Start the question manager
        if (questionManager != null)
        {
            questionManager.enabled = true;
            // Don't call Start() directly as it's private - just enable the component
            // The question manager will handle its own initialization
        }
        else
        {
            Debug.LogError("[PostTestSetupFix] Question manager not found!");
        }
    }
    
    /// <summary>
    /// Called by the main app when transitioning to MenuPostTest state
    /// </summary>
    public void OnMainAppComplete()
    {
        Debug.Log("[PostTestSetupFix] Main app completed, showing post test...");
        
        // Show the post test canvas
        if (postTestCanvas != null)
        {
            postTestCanvas.gameObject.SetActive(true);
            Debug.Log("[PostTestSetupFix] Post test canvas activated");
        }
        
        // Show the main MenuPostTest GameObject
        var menuPostTest = GameObject.Find("MenuPostTest");
        if (menuPostTest != null)
        {
            menuPostTest.SetActive(true);
            Debug.Log("[PostTestSetupFix] MenuPostTest activated");
        }
        
        // Reset UI state
        if (startButton != null)
        {
            startButton.gameObject.SetActive(true);
        }
        
        if (questionText != null)
        {
            questionText.text = "Welcome to the Post Test!\n\nYou will be shown different lunar phases and asked to position the moon correctly.\n\nClick 'Start Post Test' to begin.";
        }
        
        Debug.Log("[PostTestSetupFix] Post test is now ready and visible");
    }
    
    /// <summary>
    /// For testing purposes - manually trigger post test
    /// </summary>
    [ContextMenu("Test Post Test")]
    public void TestPostTest()
    {
        OnMainAppComplete();
    }
} 