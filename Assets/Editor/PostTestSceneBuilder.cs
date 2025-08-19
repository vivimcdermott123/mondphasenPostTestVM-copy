using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PostTestSceneBuilder
{
    [MenuItem("Tools/Build Post-Test Scene")]
    public static void BuildPostTestScene()
    {
        // Create new scene
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);
        GameObject camera = GameObject.Find("Main Camera");
        if (camera != null)
        {
            camera.tag = "MainCamera";
            camera.transform.position = new Vector3(0, 1, -6);
            camera.transform.rotation = Quaternion.identity;
            var camComp = camera.GetComponent<Camera>();
            if (camComp != null)
            {
                camComp.clearFlags = CameraClearFlags.SolidColor;
                camComp.backgroundColor = Color.black;
            }
        }

        // 1. Earth
        GameObject earth = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        earth.name = "Earth";
        earth.transform.position = Vector3.zero;
        earth.transform.localScale = Vector3.one * 0.5f;
        var earthMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/EarthRendering Free/Earth.mat");
        if (earthMat != null) earth.GetComponent<Renderer>().material = earthMat;

        // 2. Moon
        GameObject moon = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        moon.name = "Moon";
        moon.transform.position = new Vector3(2f, 0, 0);
        moon.transform.localScale = Vector3.one * 0.15f;
        var moonMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Moon/Materials/Moon_2k.mat");
        if (moonMat != null) moon.GetComponent<Renderer>().material = moonMat;
        var moonGrab = moon.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        Rigidbody moonRb = moon.GetComponent<Rigidbody>();
        if (moonRb == null)
            moonRb = moon.AddComponent<Rigidbody>();
        moonRb.useGravity = false;
        moon.GetComponent<Collider>().isTrigger = false;

        // 3. Sun (yellow sphere) and sunlight
        GameObject sun = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sun.name = "Sun";
        sun.transform.position = new Vector3(-4f, 2f, 0);
        sun.transform.localScale = Vector3.one * 0.7f;
        sun.GetComponent<Renderer>().material.color = Color.yellow;
        var sunLight = new GameObject("SunLight");
        var dirLight = sunLight.AddComponent<Light>();
        dirLight.type = LightType.Directional;
        dirLight.color = Color.white;
        dirLight.intensity = 1.2f;
        sunLight.transform.rotation = Quaternion.Euler(50, -30, 0);

        // 4. Set skybox to black (space effect)
        RenderSettings.skybox = null;
        if (Camera.main != null)
        {
            Camera.main.backgroundColor = Color.black;
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
        }

        // 5. PostTestManager
        GameObject managerGO = new GameObject("PostTestManager");
        var questionManager = managerGO.AddComponent<LunarPhaseQuestionManager>();
        var feedbackManager = managerGO.AddComponent<FeedbackManager>();
        var grader = managerGO.AddComponent<PerformanceGrader>();
        var moonDragger = moon.AddComponent<MoonDragger>();
        moonDragger.earthTransform = earth.transform;
        moonDragger.orbitRadius = 2f;
        moonDragger.questionManager = questionManager;
        questionManager.moonDragger = moonDragger;
        questionManager.feedbackManager = feedbackManager;
        questionManager.performanceGrader = grader;

        // 6. UI Canvas
        GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        // Position canvas above and behind the Earth
        canvasGO.transform.position = earth.transform.position + new Vector3(0, 1.5f, -1.5f);
        canvasGO.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f); // Good size for XR
        canvasGO.transform.LookAt(camera.transform); // Face the camera
        canvas.sortingOrder = 100;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // Blackboard Panel (center, always active)
        GameObject blackboardPanel = new GameObject("BlackboardPanel", typeof(Image));
        blackboardPanel.transform.SetParent(canvasGO.transform, false);
        blackboardPanel.SetActive(true);
        var blackboardImg = blackboardPanel.GetComponent<Image>();
        blackboardImg.color = new Color(0.1f, 0.1f, 0.1f, 0.85f);
        var bbRect = blackboardPanel.GetComponent<RectTransform>();
        bbRect.sizeDelta = new Vector2(600, 400);
        bbRect.anchoredPosition = Vector2.zero;
        bbRect.anchorMin = new Vector2(0.5f, 0.5f);
        bbRect.anchorMax = new Vector2(0.5f, 0.5f);
        bbRect.pivot = new Vector2(0.5f, 0.5f);

        // Blackboard Phase Image
        GameObject phaseImageGO = new GameObject("PhaseImage", typeof(Image));
        phaseImageGO.transform.SetParent(blackboardPanel.transform, false);
        var phaseImage = phaseImageGO.GetComponent<Image>();
        var piRect = phaseImageGO.GetComponent<RectTransform>();
        piRect.sizeDelta = new Vector2(200, 200);
        piRect.anchoredPosition = new Vector2(0, 80);
        piRect.anchorMin = new Vector2(0.5f, 0.5f);
        piRect.anchorMax = new Vector2(0.5f, 0.5f);
        piRect.pivot = new Vector2(0.5f, 0.5f);
        phaseImage.color = Color.gray;

        // Blackboard Question Text
        GameObject questionTextGO = new GameObject("QuestionText", typeof(TextMeshProUGUI));
        questionTextGO.transform.SetParent(blackboardPanel.transform, false);
        var questionText = questionTextGO.GetComponent<TextMeshProUGUI>();
        questionText.fontSize = 36;
        questionText.alignment = TextAlignmentOptions.Center;
        questionText.text = "Which orbital position matches this lunar phase?";
        var qtRect = questionTextGO.GetComponent<RectTransform>();
        qtRect.sizeDelta = new Vector2(550, 100);
        qtRect.anchoredPosition = new Vector2(0, -80);
        qtRect.anchorMin = new Vector2(0.5f, 0.5f);
        qtRect.anchorMax = new Vector2(0.5f, 0.5f);
        qtRect.pivot = new Vector2(0.5f, 0.5f);

        // Submit Button (below blackboard)
        GameObject submitBtnGO = new GameObject("SubmitButton", typeof(Button), typeof(Image));
        submitBtnGO.transform.SetParent(canvasGO.transform, false);
        var submitBtn = submitBtnGO.GetComponent<Button>();
        var submitImg = submitBtnGO.GetComponent<Image>();
        submitImg.color = new Color(0.2f, 0.6f, 0.2f, 1f);
        var sbRect = submitBtnGO.GetComponent<RectTransform>();
        sbRect.sizeDelta = new Vector2(200, 60);
        sbRect.anchoredPosition = new Vector2(0, -250);
        sbRect.anchorMin = new Vector2(0.5f, 0.5f);
        sbRect.anchorMax = new Vector2(0.5f, 0.5f);
        sbRect.pivot = new Vector2(0.5f, 0.5f);
        // Button Text
        GameObject btnTextGO = new GameObject("ButtonText", typeof(TextMeshProUGUI));
        btnTextGO.transform.SetParent(submitBtnGO.transform, false);
        var btnText = btnTextGO.GetComponent<TextMeshProUGUI>();
        btnText.text = "Submit";
        btnText.fontSize = 32;
        btnText.alignment = TextAlignmentOptions.Center;
        var btRect = btnTextGO.GetComponent<RectTransform>();
        btRect.sizeDelta = new Vector2(200, 60);
        btRect.anchoredPosition = Vector2.zero;
        btRect.anchorMin = new Vector2(0.5f, 0.5f);
        btRect.anchorMax = new Vector2(0.5f, 0.5f);
        btRect.pivot = new Vector2(0.5f, 0.5f);

        // Angle Indicator (top-right)
        GameObject anglePanel = new GameObject("AnglePanel", typeof(Image));
        anglePanel.transform.SetParent(canvasGO.transform, false);
        var angleImg = anglePanel.GetComponent<Image>();
        angleImg.color = new Color(0, 0, 0, 0.7f);
        var apRect = anglePanel.GetComponent<RectTransform>();
        apRect.sizeDelta = new Vector2(250, 60);
        apRect.anchoredPosition = new Vector2(-140, -40);
        apRect.anchorMin = new Vector2(1, 1);
        apRect.anchorMax = new Vector2(1, 1);
        apRect.pivot = new Vector2(1, 1);
        // Angle Text
        GameObject angleTextGO = new GameObject("AngleText", typeof(TextMeshProUGUI));
        angleTextGO.transform.SetParent(anglePanel.transform, false);
        var angleText = angleTextGO.GetComponent<TextMeshProUGUI>();
        angleText.fontSize = 28;
        angleText.alignment = TextAlignmentOptions.MidlineLeft;
        angleText.text = "Angle: 0°";
        var atRect = angleTextGO.GetComponent<RectTransform>();
        atRect.sizeDelta = new Vector2(230, 60);
        atRect.anchoredPosition = new Vector2(-10, -10);
        atRect.anchorMin = new Vector2(1, 1);
        atRect.anchorMax = new Vector2(1, 1);
        atRect.pivot = new Vector2(1, 1);

        // Feedback Text (bottom center)
        GameObject feedbackTextGO = new GameObject("FeedbackText", typeof(TextMeshProUGUI));
        feedbackTextGO.transform.SetParent(canvasGO.transform, false);
        var feedbackText = feedbackTextGO.GetComponent<TextMeshProUGUI>();
        feedbackText.fontSize = 36;
        feedbackText.alignment = TextAlignmentOptions.Center;
        feedbackText.rectTransform.sizeDelta = new Vector2(800, 100);
        feedbackText.rectTransform.anchoredPosition = new Vector2(0, -350);
        feedbackText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        feedbackText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        feedbackText.rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Final Grade Text (bottom center, below feedback)
        GameObject finalGradeTextGO = new GameObject("FinalGradeText", typeof(TextMeshProUGUI));
        finalGradeTextGO.transform.SetParent(canvasGO.transform, false);
        var finalGradeText = finalGradeTextGO.GetComponent<TextMeshProUGUI>();
        finalGradeText.fontSize = 48;
        finalGradeText.alignment = TextAlignmentOptions.Center;
        finalGradeText.rectTransform.sizeDelta = new Vector2(800, 100);
        finalGradeText.rectTransform.anchoredPosition = new Vector2(0, -420);
        finalGradeText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        finalGradeText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        finalGradeText.rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Assign UI to FeedbackManager and QuestionManager
        feedbackManager.feedbackText = feedbackText;
        feedbackManager.finalGradeText = finalGradeText;
        questionManager.phaseImage = phaseImage;
        questionManager.questionText = questionText;
        questionManager.submitButton = submitBtn;
        questionManager.angleText = angleText;
        // Assign canvas and earth transform for runtime control
        questionManager.mainCanvas = canvas;
        questionManager.earthTransform = earth.transform;

        // 7. Populate Lunar Phases (with placeholder sprites)
        questionManager.phases = new System.Collections.Generic.List<LunarPhase>
        {
            new LunarPhase { phaseType = LunarPhaseType.NewMoon, displayName = "New Moon", correctAngle = 0f, phaseSprite = null },
            new LunarPhase { phaseType = LunarPhaseType.WaxingCrescent, displayName = "Waxing Crescent", correctAngle = 45f, phaseSprite = null },
            new LunarPhase { phaseType = LunarPhaseType.FirstQuarter, displayName = "First Quarter", correctAngle = 90f, phaseSprite = null },
            new LunarPhase { phaseType = LunarPhaseType.WaxingGibbous, displayName = "Waxing Gibbous", correctAngle = 135f, phaseSprite = null },
            new LunarPhase { phaseType = LunarPhaseType.FullMoon, displayName = "Full Moon", correctAngle = 180f, phaseSprite = null },
            new LunarPhase { phaseType = LunarPhaseType.WaningGibbous, displayName = "Waning Gibbous", correctAngle = 225f, phaseSprite = null },
            new LunarPhase { phaseType = LunarPhaseType.LastQuarter, displayName = "Last Quarter", correctAngle = 270f, phaseSprite = null },
            new LunarPhase { phaseType = LunarPhaseType.WaningCrescent, displayName = "Waning Crescent", correctAngle = 315f, phaseSprite = null }
        };

        // 8. Save the scene
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene(),
            "Assets/Scenes/PostTestScene.unity"
        );

        Debug.Log("Post-Test Scene created! Open 'Assets/Scenes/PostTestScene.unity' to review.");
    }
} 