using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlackboardPostTestTheme : MonoBehaviour
{
    [Header("Optional: Assign a chalkboard Sprite (UI Image)")]
    public Sprite chalkboardTexture;
    [Header("Optional: Assign a chalk-style TMP font asset")]
    public TMP_FontAsset chalkFont;

    // Blackboard and button colors
    private Color32 blackboardColor = new Color32(30, 50, 30, 255); // #1E321E
    private Color32 buttonGreen = new Color32(34, 139, 34, 255);    // #228B22
    private Color32 textWhite = new Color32(240, 240, 220, 255);    // Chalky white

    void Start()
    {
        ApplyTheme();
    }
    
    public void ApplyTheme()
    {
        Debug.Log("[BlackboardPostTestTheme] Applying blackboard theme...");
        
        // Force the main canvas background first
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            // Try to set canvas background through CanvasGroup
            var canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            
            // Also try to set the canvas background color directly if possible
            var canvasImage = GetComponent<Image>();
            if (canvasImage == null)
            {
                canvasImage = gameObject.AddComponent<Image>();
            }
            canvasImage.color = blackboardColor;
            Debug.Log($"[BlackboardPostTestTheme] Set main canvas background to blackboard color: {blackboardColor}");
        }
        
        // Set background panel(s) to chalkboard texture or color - MORE AGGRESSIVE
        Image[] allImages = GetComponentsInChildren<Image>(true);
        foreach (var img in allImages)
        {
            // More comprehensive background detection
            bool isBackground = img.GetComponent<Button>() == null && 
                               (img.transform.childCount > 0 || 
                                img.name.ToLower().Contains("panel") || 
                                img.name.ToLower().Contains("background") ||
                                img.name.ToLower().Contains("canvas") ||
                                img.name.ToLower().Contains("blackboard") ||
                                img.rectTransform.sizeDelta.x > 100 || // Large images are likely backgrounds
                                img.rectTransform.sizeDelta.y > 100);
            
            if (isBackground)
            {
                Debug.Log($"[BlackboardPostTestTheme] Setting background for: {img.name} (size: {img.rectTransform.sizeDelta})");
                if (chalkboardTexture != null)
                {
                    img.sprite = chalkboardTexture;
                    img.type = Image.Type.Sliced;
                    img.color = Color.white; // Show texture as-is
                }
                else
                {
                    img.color = blackboardColor;
                    Debug.Log($"[BlackboardPostTestTheme] Set {img.name} to blackboard color: {blackboardColor}");
                }
            }
        }

        // Set all TextMeshProUGUI to white and chalk font, with proper text fitting
        TextMeshProUGUI[] allTexts = GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var tmp in allTexts)
        {
            // Set color
            tmp.color = textWhite;
            
            // Apply chalk font if available
            if (chalkFont != null)
                tmp.font = chalkFont;
            
            // Configure text fitting
            tmp.enableAutoSizing = true;
            tmp.fontSizeMin = 8f;
            tmp.fontSizeMax = 72f;
            tmp.overflowMode = TextOverflowModes.Ellipsis;
            tmp.enableWordWrapping = true;
            
            // Set proper text alignment
            if (tmp.name.ToLower().Contains("button"))
            {
                tmp.alignment = TextAlignmentOptions.Center;
            }
            else
            {
                tmp.alignment = TextAlignmentOptions.Left;
            }
            
            // Add outline for chalk effect
            var outline = tmp.GetComponent<Outline>();
            if (outline == null)
                outline = tmp.gameObject.AddComponent<Outline>();
            outline.effectColor = Color.white;
            outline.effectDistance = new Vector2(1.5f, -1.5f);
            
            // Set proper text for buttons if they have placeholder text
            if (tmp.text == "Test" || tmp.text == "Button")
            {
                if (tmp.transform.parent != null && tmp.transform.parent.GetComponent<Button>() != null)
                {
                    // This is a button text - set appropriate text based on button name
                    string buttonName = tmp.transform.parent.name.ToLower();
                    if (buttonName.Contains("start"))
                    {
                        tmp.text = "Start Post Test";
                    }
                    else if (buttonName.Contains("submit"))
                    {
                        tmp.text = "Submit Answer";
                    }
                    else
                    {
                        tmp.text = "Continue";
                    }
                }
            }
            
            Debug.Log($"[BlackboardPostTestTheme] Configured text: {tmp.name} with auto-sizing and wrapping");
        }

        // Set all Buttons to green with white text
        Button[] allButtons = GetComponentsInChildren<Button>(true);
        foreach (var btn in allButtons)
        {
            var img = btn.GetComponent<Image>();
            if (img != null)
            {
                img.color = buttonGreen;
                Debug.Log($"[BlackboardPostTestTheme] Set button {btn.name} to green color: {buttonGreen}");
            }

            var btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                btnText.color = textWhite;
                if (chalkFont != null)
                    btnText.font = chalkFont;
                
                // Configure button text fitting
                btnText.enableAutoSizing = true;
                btnText.fontSizeMin = 8f;
                btnText.fontSizeMax = 48f;
                btnText.overflowMode = TextOverflowModes.Ellipsis;
                btnText.enableWordWrapping = true;
                btnText.alignment = TextAlignmentOptions.Center;
                
                // Optional: Add outline for chalk effect
                var outline = btnText.GetComponent<Outline>();
                if (outline == null)
                    outline = btnText.gameObject.AddComponent<Outline>();
                outline.effectColor = Color.white;
                outline.effectDistance = new Vector2(1.5f, -1.5f);
            }
        }
        
        Debug.Log("[BlackboardPostTestTheme] Theme application completed!");
    }
}