using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Simple post test setup script - attach this to any empty GameObject
/// </summary>
public class SimplePostTestSetup : MonoBehaviour
{
    [Header("Assign Your Post Test Canvas Here")]
    [SerializeField] private Canvas postTestCanvas;
    
    private void Start()
    {
        Debug.Log("[SimplePostTestSetup] Starting setup...");
        
        // Find canvas if not assigned
        if (postTestCanvas == null)
        {
            Canvas[] allCanvases = FindObjectsOfType<Canvas>();
            foreach (var canvas in allCanvases)
            {
                if (canvas.name.ToLower().Contains("post"))
                {
                    postTestCanvas = canvas;
                    Debug.Log($"[SimplePostTestSetup] Found post test canvas: {canvas.name}");
                    break;
                }
            }
        }
        
        if (postTestCanvas != null)
        {
            // Apply blackboard theme
            BlackboardPostTestTheme theme = postTestCanvas.GetComponent<BlackboardPostTestTheme>();
            if (theme == null)
            {
                theme = postTestCanvas.gameObject.AddComponent<BlackboardPostTestTheme>();
            }
            theme.ApplyTheme();
            
            // Find and setup question manager
            LunarPhaseQuestionManager questionManager = FindObjectOfType<LunarPhaseQuestionManager>();
            if (questionManager != null)
            {
                // Auto-assign references
                questionManager.mainCanvas = postTestCanvas;
                
                // Find UI elements
                Button startButton = postTestCanvas.transform.Find("StartButton")?.GetComponent<Button>();
                Button submitButton = postTestCanvas.transform.Find("SubmitButton")?.GetComponent<Button>();
                TextMeshProUGUI questionText = postTestCanvas.transform.Find("QuestionText")?.GetComponent<TextMeshProUGUI>();
                Image phaseImage = postTestCanvas.transform.Find("PhaseImage")?.GetComponent<Image>();
                
                // Assign to question manager
                questionManager.phaseImage = phaseImage;
                questionManager.questionText = questionText;
                questionManager.submitButton = submitButton;
                
                // Set button texts
                if (startButton != null)
                {
                    TextMeshProUGUI startText = startButton.GetComponentInChildren<TextMeshProUGUI>();
                    if (startText != null) 
                    {
                        startText.text = "Start Post Test";
                        // Set up start button functionality
                        startButton.onClick.RemoveAllListeners();
                        startButton.onClick.AddListener(() => {
                            startButton.gameObject.SetActive(false);
                            if (questionManager != null)
                            {
                                questionManager.enabled = true;
                            }
                        });
                    }
                }
                
                if (submitButton != null)
                {
                    TextMeshProUGUI submitText = submitButton.GetComponentInChildren<TextMeshProUGUI>();
                    if (submitText != null) submitText.text = "Submit Answer";
                }
                
                Debug.Log("[SimplePostTestSetup] Successfully set up question manager!");
            }
            else
            {
                Debug.LogError("[SimplePostTestSetup] LunarPhaseQuestionManager not found!");
            }
        }
        else
        {
            Debug.LogError("[SimplePostTestSetup] Post test canvas not found!");
        }
    }
    
    [ContextMenu("Test Post Test")]
    public void TestPostTest()
    {
        if (postTestCanvas != null)
        {
            postTestCanvas.gameObject.SetActive(true);
            Debug.Log("[SimplePostTestSetup] Post test canvas activated!");
        }
    }
} 