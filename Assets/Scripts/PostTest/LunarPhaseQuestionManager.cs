using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class LunarPhaseQuestionManager : MonoBehaviour
{
    public List<LunarPhase> phases = new List<LunarPhase>();
    public int totalQuestions = 5;
    private int currentQuestion = 0;
    private float totalAngularError = 0f;
    private List<LunarPhase> questionOrder = new List<LunarPhase>();

    public MoonDragger moonDragger;
    public FeedbackManager feedbackManager;
    public PerformanceGrader performanceGrader;
    public UnityEngine.UI.Image phaseImage;
    public TMPro.TextMeshProUGUI questionText;
    public UnityEngine.UI.Button submitButton;
    public TMPro.TextMeshProUGUI angleText;
    // Remove mainUIPanel and startButton fields

    private float currentUserAngle = 0f;
    private bool canSubmit = false;
    private LunarPhase currentPhase;

    public Material galaxySkybox; // Assign in Inspector
    public Canvas mainCanvas; // Assign in Inspector
    public Transform earthTransform; // Assign in Inspector
    public float canvasDistance = 2.0f; // Distance from camera
    public Vector3 canvasOffset = new Vector3(0, 1.5f, -1.5f); // Offset from earth

    // Add fields for text file loading
    [Header("Post Test Text File")]
    [SerializeField] private string postTestFileName = "MondphasenText.txt";
    private string[] postTestLines;
    [SerializeField] private int postTestStartLine = 37; // First post test question
    [SerializeField] private int feedbackStartLine = 46; // First feedback line
    [SerializeField] private int gradeLine = 54; // Final grade line

    // Add fields for sprite assignment
    [SerializeField] private string spriteFolder = "LunarPhases"; // e.g., Resources/LunarPhases

    private void LoadPostTestText()
    {
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, postTestFileName);
        if(System.IO.File.Exists(filePath))
        {
            postTestLines = System.IO.File.ReadAllLines(filePath);
            Debug.Log($"[LunarPhaseQuestionManager] Loaded {postTestLines.Length} lines from {postTestFileName}");
        }
        else
        {
            Debug.LogError("[LunarPhaseQuestionManager] Post test text file not found! " + filePath);
            postTestLines = new string[] { "Post test text file not found!" };
        }
        
        if (feedbackManager != null)
        {
            feedbackManager.SetPostTestLines(postTestLines);
        }
        else
        {
            Debug.LogWarning("[LunarPhaseQuestionManager] feedbackManager is null - feedback may not work properly");
        }
    }

    private void DisplayPostTestText(int lineIndex)
    {
        if (questionText == null)
        {
            Debug.LogError("[LunarPhaseQuestionManager] questionText is null! Cannot display text.");
            return;
        }
        
        if (postTestLines != null && lineIndex < postTestLines.Length)
        {
            questionText.text = postTestLines[lineIndex];
        }
        else
        {
            questionText.text = "Text index out of range!";
            Debug.LogWarning($"[LunarPhaseQuestionManager] Text line index {lineIndex} out of range. postTestLines length: {(postTestLines != null ? postTestLines.Length : 0)}");
        }
    }

    private void AutoAssignPhaseSprites()
    {
        Debug.Log($"[LunarPhaseQuestionManager] AutoAssignPhaseSprites: Loading sprites from folder '{spriteFolder}'");
        
        // Load all sprites from the specified Resources folder
        Sprite[] sprites = Resources.LoadAll<Sprite>(spriteFolder);
        Debug.Log($"[LunarPhaseQuestionManager] Found {sprites.Length} sprites in Resources/{spriteFolder}");
        
        foreach (var phase in phases)
        {
            // Try to match by phaseType (recommended, as it's an enum and less likely to have typos)
            string phaseName = phase.phaseType.ToString().Replace(" ", "").ToLower();
            // Find a sprite whose name matches the phaseType (case-insensitive, spaces removed)
            Sprite found = System.Array.Find(sprites, s => s.name.Replace(" ", "").ToLower() == phaseName);
            if (found != null)
            {
                phase.phaseSprite = found;
                Debug.Log($"[LunarPhaseQuestionManager] Assigned sprite '{found.name}' to phase {phase.phaseType}");
            }
            else
            {
                Debug.LogWarning($"[LunarPhaseQuestionManager] No sprite found for phase {phase.phaseType} (expected sprite name: {phaseName})");
            }
        }
    }

    private void AutoAssignPhaseAngles()
    {
        foreach (var phase in phases)
        {
            switch (phase.phaseType)
            {
                case LunarPhaseType.NewMoon:
                    phase.correctAngle = 0f;
                    break;
                case LunarPhaseType.WaxingCrescent:
                    phase.correctAngle = 45f;
                    break;
                case LunarPhaseType.FirstQuarter:
                    phase.correctAngle = 90f;
                    break;
                case LunarPhaseType.WaxingGibbous:
                    phase.correctAngle = 135f;
                    break;
                case LunarPhaseType.FullMoon:
                    phase.correctAngle = 180f;
                    break;
                case LunarPhaseType.WaningGibbous:
                    phase.correctAngle = 225f;
                    break;
                case LunarPhaseType.LastQuarter:
                    phase.correctAngle = 270f;
                    break;
                case LunarPhaseType.WaningCrescent:
                    phase.correctAngle = 315f;
                    break;
            }
        }
    }

    private void PopulatePhasesList()
    {
        // Only populate if the list is empty
        if (phases.Count == 0)
        {
            Debug.Log("[LunarPhaseQuestionManager] Populating phases list...");
            
            // Create all 8 lunar phases
            phases.Add(new LunarPhase { phaseType = LunarPhaseType.NewMoon, displayName = "New Moon" });
            phases.Add(new LunarPhase { phaseType = LunarPhaseType.WaxingCrescent, displayName = "Waxing Crescent" });
            phases.Add(new LunarPhase { phaseType = LunarPhaseType.FirstQuarter, displayName = "First Quarter" });
            phases.Add(new LunarPhase { phaseType = LunarPhaseType.WaxingGibbous, displayName = "Waxing Gibbous" });
            phases.Add(new LunarPhase { phaseType = LunarPhaseType.FullMoon, displayName = "Full Moon" });
            phases.Add(new LunarPhase { phaseType = LunarPhaseType.WaningGibbous, displayName = "Waning Gibbous" });
            phases.Add(new LunarPhase { phaseType = LunarPhaseType.LastQuarter, displayName = "Last Quarter" });
            phases.Add(new LunarPhase { phaseType = LunarPhaseType.WaningCrescent, displayName = "Waning Crescent" });
            
            Debug.Log($"[LunarPhaseQuestionManager] Created {phases.Count} lunar phases");
        }
        else
        {
            Debug.Log($"[LunarPhaseQuestionManager] Phases list already has {phases.Count} items");
        }
    }

    // Remove Awake and OnStartClicked methods

    void Start()
    {
        LoadPostTestText();
        PopulatePhasesList(); // Populate phases first
        AutoAssignPhaseAngles(); // Then assign angles
        AutoAssignPhaseSprites(); // Then assign sprites
        // Disable any onboarding/tutorial panels at runtime
        string[] onboardingNames = {"TutorialPanel", "OnboardingPanel", "IntroPanel", "GoalManager", "CoachingUIParent", "StepPanel", "WelcomePanel"};
        foreach (string name in onboardingNames)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null && obj.activeSelf)
            {
                obj.SetActive(false);
                Debug.Log($"Disabled onboarding/tutorial panel: {name}");
            }
        }

        // Log a warning if the test panel is not found or not visible
        if (mainCanvas == null)
        {
            Debug.LogWarning("[LunarPhaseQuestionManager] mainCanvas is not assigned!");
        }
        else
        {
            if (!mainCanvas.gameObject.activeInHierarchy)
            {
                Debug.LogWarning("[LunarPhaseQuestionManager] mainCanvas is not active in hierarchy!");
            }
            if (mainCanvas.transform.childCount == 0)
            {
                Debug.LogWarning("[LunarPhaseQuestionManager] mainCanvas has no children (test panel may be missing)!");
            }
            else
            {
                var testPanel = mainCanvas.transform.GetChild(0).gameObject;
                if (!testPanel.activeInHierarchy)
                {
                    Debug.LogWarning("[LunarPhaseQuestionManager] Test panel (first child of mainCanvas) is not active in hierarchy!");
                }
            }
        }

        // Set galaxy skybox
        if (galaxySkybox != null)
        {
            RenderSettings.skybox = galaxySkybox;
            if (Camera.main != null)
                Camera.main.clearFlags = CameraClearFlags.Skybox;
        }
        // Disable AR passthrough if present
        if (Camera.main != null)
        {
            var arBg = Camera.main.GetComponent<ARCameraBackground>();
            if (arBg != null) arBg.enabled = false;
        }
        // Begin the post-test immediately, as before
        questionOrder = new List<LunarPhase>(phases);
        Shuffle(questionOrder);
        if (questionOrder.Count > totalQuestions)
            questionOrder = questionOrder.GetRange(0, totalQuestions);
        currentQuestion = 0;
        totalAngularError = 0f;
        canSubmit = false;
        ShowCurrentQuestion();
        if (submitButton != null)
        {
            submitButton.onClick.RemoveAllListeners();
            submitButton.onClick.AddListener(OnSubmitClicked);
            submitButton.interactable = false;
        }
    }

    void Shuffle(List<LunarPhase> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            LunarPhase temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void ShowCurrentQuestion()
    {
        if (currentQuestion < questionOrder.Count)
        {
            currentPhase = questionOrder[currentQuestion];
            Debug.Log($"[LunarPhaseQuestionManager] Showing question {currentQuestion + 1}: {currentPhase.phaseType} (sprite: {(currentPhase.phaseSprite != null ? currentPhase.phaseSprite.name : "NULL")})");
            
            moonDragger.SetTargetPhase(currentPhase);
            // Use text file for question text
            DisplayPostTestText(postTestStartLine + currentQuestion);
            if (phaseImage != null)
            {
                phaseImage.sprite = currentPhase.phaseSprite;
                Debug.Log($"[LunarPhaseQuestionManager] Set phaseImage.sprite to {(currentPhase.phaseSprite != null ? currentPhase.phaseSprite.name : "NULL")}");
            }
            else
            {
                Debug.LogError("[LunarPhaseQuestionManager] phaseImage is null!");
            }
            if (submitButton != null)
                submitButton.interactable = false;
            canSubmit = false;
            
            // Initialize angle display
            if (moonDragger != null)
            {
                float initialAngle = moonDragger.GetCurrentAngle();
                UpdateUserAngle(initialAngle);
            }
        }
        else
        {
            // Test finished
            string grade = performanceGrader.GetGrade(totalAngularError, questionOrder.Count);
            // Show final grade from text file
            if (feedbackManager != null)
                feedbackManager.ShowFinalGradeLine(gradeLine);
            DisplayPostTestText(postTestStartLine + questionOrder.Count);
            if (phaseImage != null)
                phaseImage.sprite = null;
            if (submitButton != null)
                submitButton.interactable = false;
            if (angleText != null)
                angleText.text = "";
        }
    }
    
    // Add a method to continuously update angle display
    void Update()
    {
        // Continuously update angle display even when not dragging
        if (moonDragger != null && currentPhase != null)
        {
            float currentAngle = moonDragger.GetCurrentAngle();
            if (Mathf.Abs(currentAngle - currentUserAngle) > 0.1f) // Only update if angle changed significantly
            {
                UpdateUserAngle(currentAngle);
            }
        }
    }

    // Called by MoonDragger as the user drags the moon
    public void UpdateUserAngle(float angle)
    {
        currentUserAngle = angle;
        
        // Update angle text with more detailed information
        if (angleText != null)
        {
            if (currentPhase != null)
            {
                float targetAngle = currentPhase.correctAngle;
                float angularError = Mathf.DeltaAngle(currentUserAngle, targetAngle);
                string errorText = angularError >= 0 ? $"+{angularError:F1}°" : $"{angularError:F1}°";
                
                angleText.text = $"Moon Angle: {currentUserAngle:F1}°\nTarget: {targetAngle:F1}°\nError: {errorText}";
                
                // Color code the error (green for good, yellow for okay, red for poor)
                if (Mathf.Abs(angularError) <= 15f)
                {
                    angleText.color = Color.green;
                }
                else if (Mathf.Abs(angularError) <= 30f)
                {
                    angleText.color = Color.yellow;
                }
                else
                {
                    angleText.color = Color.red;
                }
            }
            else
            {
                angleText.text = $"Moon Angle: {currentUserAngle:F1}°";
                angleText.color = Color.white;
            }
        }
        
        // Enable submit button when user starts interacting
        if (submitButton != null && !canSubmit)
            submitButton.interactable = true;
        canSubmit = true;
    }

    // Called when the submit button is clicked
    public void OnSubmitClicked()
    {
        if (!canSubmit) return;
        float correctAngle = currentPhase.correctAngle;
        float angularError = Mathf.DeltaAngle(currentUserAngle, correctAngle);
        RecordAnswer(angularError);
        canSubmit = false;
        if (submitButton != null)
            submitButton.interactable = false;
    }

    public void RecordAnswer(float angularError)
    {
        totalAngularError += Mathf.Abs(angularError);
        // Show feedback from text file
        if (feedbackManager != null)
            feedbackManager.ShowFeedbackLine(feedbackStartLine + currentQuestion);
        currentQuestion++;
        Invoke(nameof(ShowCurrentQuestion), 2f); // Wait 2 seconds before next question
    }

    void LateUpdate()
    {
        // Force the canvas to always be 2 meters in front of the camera and face the user
        if (mainCanvas != null && Camera.main != null)
        {
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camPos = Camera.main.transform.position;
            mainCanvas.transform.position = camPos + camForward * 2.0f;
            mainCanvas.transform.rotation = Quaternion.LookRotation(mainCanvas.transform.position - camPos);
            // Debug log
            var bbPanel = mainCanvas.transform.Find("BlackboardPanel");
            Debug.Log($"[Canvas Debug] Canvas pos: {mainCanvas.transform.position}, scale: {mainCanvas.transform.localScale}, active: {mainCanvas.gameObject.activeInHierarchy}");
            if (bbPanel != null)
            {
                Debug.Log($"[Canvas Debug] BlackboardPanel pos: {bbPanel.position}, scale: {bbPanel.localScale}, active: {bbPanel.gameObject.activeInHierarchy}");
            }
            else
            {
                Debug.LogWarning("[Canvas Debug] BlackboardPanel not found as child of Canvas!");
            }
        }
    }
} 