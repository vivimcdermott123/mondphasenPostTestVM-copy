using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System.Security.Cryptography;

public enum AppState : int
{
    Tutorial_Overview,
    Tutorial_MenuSelect,
    Tutorial_Menudown,
    TransitionFromFoam,
    Intro,
    TaskOne_SunDirection,
    TaskOne_Perspective,
    TaskOne_Intro,
    TaskOne_ObjectConstellation,
    TaskOne_ObjectConstellationFoam,
    TaskOne_MentalPerspectiveChange,
    TaskOne_QuestionBirdView,
    TaskOne_QuestionBirdViewWrong,
    TaskOne_QuestionBirdViewRight,
    TaskOne_PerspectiveChange,
    TaskOne_ExplanationIntro,
    TaskOne_StepShading,
    TaskOne_ExplanationShading,
    TaskOne_QuestionShading,
    TaskOne_ShadingWrong,
    TaskOne_ShadingRight,
    TaskOne_StepLine,
    TaskOne_ExplanationLine,
    TaskOne_QuestionLine,
    TaskOne_LineWrong,
    TaskOne_LineRight,
    TaskOne_StepLighting,
    TaskOne_ExplanationLighting,
    TaskOne_QuestionLighting,
    TaskOne_LightingWrong,
    TaskOne_LightingRight,
    TaskOne_StepLightSide,
    TaskOne_ExplanationLightSide,
    TaskOne_QuestionLightSide,
    TaskOne_LightSideWrong,
    TaskOne_LightSideRight,
    TaskOne_ExplanationMoonphase,
    TaskOne_MoonphaseWrong,
    TaskOne_MoonphaseRight,
    TaskOne_ChangeToPaper,
    TaskTwo_Intro,
    TaskTwo_Question,
    TaskTwo_WrongAnswer,
    TaskTwo_CorrectAnswer,
    EndOfApp,
    MenuPostTest,
    Tutorial_Exploremove,
    Tutorial_Explorescroll,
    Experiment_Intro,
    Experiment_EarthView,
    TaskOne_ObjectConstellationEarth,
    TaskOne_ObjectConstellationMoon,
    TaskOne_ExplanationPerspective,
    TaskOne_FreeExploration,
    TaskTwo_Freeplay,
}

public class Moonphases_Manager : MonoBehaviour
{

    #region SerializeFields

    [Header("Menu Core")]
    [SerializeField] private GameObject MenuMain;
    [SerializeField] private GameObject MenuCore;
    [SerializeField] private float ScreenSize;
    [SerializeField] private GameObject buttonNext;
    [SerializeField] private GameObject buttonPrevious;

    [Header("Menu Extended")]
    [SerializeField] private GameObject MenuExtended;

    [Header("Menu Image")]
    [SerializeField] private GameObject MenuImage;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Image ImageComponent;
    [SerializeField] private int currentSprite = 0;

    [Header("Menu Question")]
    [SerializeField] private GameObject MenuQuestion;
    [SerializeField] private GameObject MenuLeftRight;
    [SerializeField] private GameObject MenuPostTest;

    [Header("Visualizations")]
    [SerializeField] private GameObject MoonLineShading;
    [SerializeField] private GameObject MoonLineEarthFacing;
    [SerializeField] private GameObject MoonLineParallels;
    [SerializeField] private GameObject SunDirectionVis;
    [SerializeField] private GameObject Label_ObjectConstellation;
    [SerializeField] private GameObject Label_ObjectConstellationFoam;
    [SerializeField] private GameObject EquatorLine;

    [Header("Textmanagement")]
    [SerializeField] private TextMeshProUGUI textDisplay;
    [SerializeField] private string fileName = "MondphasenText.txt";
    [SerializeField] private string[] allLines;

    [Header("Audiomanagement")]
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private AudioSource audioSource;

    [Header("CameraZoom")]
    [SerializeField] private Camera MainCamera;
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minZoom = 20f;
    [SerializeField] private float maxZoom = 60f;
    [SerializeField] private bool CameraZoomActive = false;

    [Header("CameraPerspective")]
    [SerializeField] private Transform CameraToMove;
    [SerializeField] private Transform BirdViewPosition;
    [SerializeField] private Transform EarthViewPosition;
    [SerializeField] private Transform BirdViewMoonPosition;
    [SerializeField] private float threshold = 0.1f;
    [SerializeField] private bool isUpPressed = false;
    [SerializeField] private bool isDownPressed = false;
    [SerializeField] private bool ChangePerspectiveActive = false;
    [SerializeField] private Transform LookDirectionEarth;
    [SerializeField] private Transform LookDirectionBirdView;
    [SerializeField] private Transform PlanetSystem;
    [SerializeField] private Transform PlanetPostionBirdView;
    [SerializeField] private Transform SceneContent;

    [Header("Moon")]
    [SerializeField] private Transform Moon_Body;
    [SerializeField] private Transform MoonPosition_Intro;
    [SerializeField] private Transform MoonPosition_Mission1;
    [SerializeField] private Transform MoonPosition_Mission2;

    private AppState appState;

    #endregion

    #region Initilization 
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("[Moonphases_Manager] ===== START() BEGIN =====");
        Debug.Log("[Moonphases_Manager] GameObject name: " + gameObject.name);
        Debug.Log("[Moonphases_Manager] GameObject active: " + gameObject.activeInHierarchy);
        Debug.Log("[Moonphases_Manager] Component enabled: " + this.enabled);
        Debug.Log("[Moonphases_Manager] Platform: " + Application.platform);
        
        AutoAssignComponents();
        
        Debug.Log("[Moonphases_Manager] After AutoAssignComponents:");
        Debug.Log("[Moonphases_Manager] - CameraToMove: " + (CameraToMove != null ? CameraToMove.name + " at " + CameraToMove.position : "NULL"));
        Debug.Log("[Moonphases_Manager] - Moon_Body: " + (Moon_Body != null ? Moon_Body.name + " at " + Moon_Body.position : "NULL"));
        Debug.Log("[Moonphases_Manager] - SceneContent: " + (SceneContent != null ? SceneContent.name + " active:" + SceneContent.gameObject.activeInHierarchy : "NULL"));
        Debug.Log("[Moonphases_Manager] - PlanetSystem: " + (PlanetSystem != null ? PlanetSystem.name + " at " + PlanetSystem.position : "NULL"));
        
        InitScreenAndMenu();
        LoadTextFromFile();
        
        // Ensure post test panel is hidden at startup
        HidePostTestPanel();
        
        appState = AppState.Tutorial_Overview;
        
        Debug.Log("[Moonphases_Manager] Before AppStateHandler - appState: " + appState);
        AppStateHandler();
        
        Debug.Log("[Moonphases_Manager] Before camera positioning:");
        Debug.Log("[Moonphases_Manager] - BirdViewPosition: " + (BirdViewPosition != null ? BirdViewPosition.position.ToString() : "NULL"));
        Debug.Log("[Moonphases_Manager] - LookDirectionEarth: " + (LookDirectionEarth != null ? LookDirectionEarth.position.ToString() : "NULL"));
        
        // VisionOS-specific camera setup
        if (CameraToMove != null && BirdViewPosition != null && LookDirectionEarth != null)
        {
            // For visionOS, we need to be more careful with camera positioning
            // The VolumeCamera will handle the main rendering, so we just set up the scene camera
            CameraToMove.position = BirdViewPosition.position;
            CameraToMove.LookAt(LookDirectionEarth.position);
            
            // Ensure camera is properly configured for visionOS
            if (CameraToMove.GetComponent<Camera>() != null)
            {
                var cam = CameraToMove.GetComponent<Camera>();
                cam.stereoTargetEye = StereoTargetEyeMask.Both;
                cam.nearClipPlane = 0.01f;
                cam.farClipPlane = 1000f;
                Debug.Log("[Moonphases_Manager] Camera configured for visionOS stereo rendering");
            }
            
            Debug.Log("[Moonphases_Manager] Camera positioned to: " + CameraToMove.position + " looking at: " + LookDirectionEarth.position);
        }
        else
        {
            Debug.LogError("[Moonphases_Manager] CANNOT POSITION CAMERA - Missing components!");
            Debug.LogError("[Moonphases_Manager] CameraToMove: " + (CameraToMove != null ? "OK" : "NULL"));
            Debug.LogError("[Moonphases_Manager] BirdViewPosition: " + (BirdViewPosition != null ? "OK" : "NULL"));
            Debug.LogError("[Moonphases_Manager] LookDirectionEarth: " + (LookDirectionEarth != null ? "OK" : "NULL"));
        }
        
        audioSource = GetComponent<AudioSource>();
        Debug.Log("[Moonphases_Manager] AudioSource: " + (audioSource != null ? "Found" : "NOT FOUND"));
        
        // VisionOS-specific initialization
        InitializeForVisionOS();
        
        Debug.Log("[Moonphases_Manager] ===== START() COMPLETED =====");
    }

    private void AutoAssignComponents()
    {
        Debug.Log("[Moonphases_Manager] ===== AUTO-ASSIGN COMPONENTS BEGIN =====");
        Debug.Log("[Moonphases_Manager] Scene name: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        Debug.Log("[Moonphases_Manager] All GameObjects in scene: " + FindObjectsOfType<GameObject>().Length);

        // Auto-assign camera
        if (CameraToMove == null)
        {
            Debug.Log("[Moonphases_Manager] Looking for camera...");
            CameraToMove = Camera.main?.transform;
            if (CameraToMove == null)
            {
                Debug.Log("[Moonphases_Manager] Camera.main is null, searching for any camera...");
                CameraToMove = FindObjectOfType<Camera>()?.transform;
            }
            if (CameraToMove == null)
            {
                Debug.LogError("[Moonphases_Manager] NO CAMERA FOUND IN SCENE!");
                Debug.LogError("[Moonphases_Manager] All cameras in scene:");
                Camera[] allCameras = FindObjectsOfType<Camera>();
                foreach (var cam in allCameras)
                {
                    Debug.LogError("[Moonphases_Manager] - Camera: " + cam.name + " enabled:" + cam.enabled + " at " + cam.transform.position);
                }
            }
            else
            {
                Debug.Log("[Moonphases_Manager] Found camera: " + CameraToMove.name + " at " + CameraToMove.position + " enabled:" + CameraToMove.GetComponent<Camera>().enabled);
            }
        }
        else
        {
            Debug.Log("[Moonphases_Manager] Camera already assigned: " + CameraToMove.name);
        }

        // Auto-assign moon
        if (Moon_Body == null)
        {
            Debug.Log("[Moonphases_Manager] Looking for moon...");
            GameObject moon = GameObject.Find("Moon") ?? GameObject.Find("Moon_Body") ?? GameObject.Find("MoonBody") ?? GameObject.Find("Moon_2k");
            if (moon != null) 
            {
                Moon_Body = moon.transform;
                Debug.Log("[Moonphases_Manager] Found moon: " + Moon_Body.name + " at " + Moon_Body.position + " active:" + Moon_Body.gameObject.activeInHierarchy);
            }
            else
            {
                Debug.LogError("[Moonphases_Manager] NO MOON FOUND IN SCENE!");
                Debug.LogError("[Moonphases_Manager] All GameObjects with 'moon' in name:");
                GameObject[] moonObjects = FindObjectsOfType<GameObject>();
                foreach (var obj in moonObjects)
                {
                    if (obj.name.ToLower().Contains("moon"))
                    {
                        Debug.LogError("[Moonphases_Manager] - " + obj.name + " at " + obj.transform.position + " active:" + obj.activeInHierarchy);
                    }
                }
            }
        }
        else
        {
            Debug.Log("[Moonphases_Manager] Moon already assigned: " + Moon_Body.name);
        }

        // Auto-assign scene content
        if (SceneContent == null)
        {
            Debug.Log("[Moonphases_Manager] Looking for scene content...");
            GameObject sceneContent = GameObject.Find("SceneContent") ?? GameObject.Find("Scene") ?? GameObject.Find("MainScene");
            if (sceneContent != null) 
            {
                SceneContent = sceneContent.transform;
                Debug.Log("[Moonphases_Manager] Found scene content: " + SceneContent.name + " at " + SceneContent.position + " active:" + SceneContent.gameObject.activeInHierarchy);
            }
            else
            {
                Debug.LogError("[Moonphases_Manager] NO SCENE CONTENT FOUND!");
                Debug.LogError("[Moonphases_Manager] All GameObjects with 'scene' in name:");
                GameObject[] sceneObjects = FindObjectsOfType<GameObject>();
                foreach (var obj in sceneObjects)
                {
                    if (obj.name.ToLower().Contains("scene"))
                    {
                        Debug.LogError("[Moonphases_Manager] - " + obj.name + " at " + obj.transform.position + " active:" + obj.activeInHierarchy);
                    }
                }
            }
        }
        else
        {
            Debug.Log("[Moonphases_Manager] Scene content already assigned: " + SceneContent.name);
        }

        // Auto-assign planet system
        if (PlanetSystem == null)
        {
            GameObject planetSystem = GameObject.Find("PlanetSystem") ?? GameObject.Find("Planets") ?? GameObject.Find("SolarSystem") ?? GameObject.Find("Earth");
            if (planetSystem != null) PlanetSystem = planetSystem.transform;
            Debug.Log(PlanetSystem != null ? "Found planet system: " + PlanetSystem.name : "No planet system found");
        }

        // Auto-assign UI components
        if (MenuCore == null)
        {
            MenuCore = GameObject.Find("MenuCore") ?? GameObject.Find("MainMenu") ?? GameObject.Find("Menu");
            Debug.Log(MenuCore != null ? "Found MenuCore: " + MenuCore.name : "No MenuCore found");
        }

        if (MenuExtended == null)
        {
            MenuExtended = GameObject.Find("MenuExtended") ?? GameObject.Find("ExtendedMenu") ?? GameObject.Find("MenuText");
            Debug.Log(MenuExtended != null ? "Found MenuExtended: " + MenuExtended.name : "No MenuExtended found");
        }

        if (MenuMain == null)
        {
            MenuMain = GameObject.Find("MenuMain") ?? GameObject.Find("MainMenu");
            Debug.Log(MenuMain != null ? "Found MenuMain: " + MenuMain.name : "No MenuMain found");
        }

        if (MenuImage == null)
        {
            MenuImage = GameObject.Find("MenuImage") ?? GameObject.Find("ImageMenu");
            Debug.Log(MenuImage != null ? "Found MenuImage: " + MenuImage.name : "No MenuImage found");
        }

        if (MenuQuestion == null)
        {
            MenuQuestion = GameObject.Find("MenuQuestion") ?? GameObject.Find("QuestionMenu");
            Debug.Log(MenuQuestion != null ? "Found MenuQuestion: " + MenuQuestion.name : "No MenuQuestion found");
        }

        if (MenuLeftRight == null)
        {
            MenuLeftRight = GameObject.Find("MenuLeftRight") ?? GameObject.Find("LeftRightMenu");
            Debug.Log(MenuLeftRight != null ? "Found MenuLeftRight: " + MenuLeftRight.name : "No MenuLeftRight found");
        }

        if (MenuPostTest == null)
        {
            MenuPostTest = GameObject.Find("MenuPostTest") ?? GameObject.Find("PostTestMenu");
            Debug.Log(MenuPostTest != null ? "Found MenuPostTest: " + MenuPostTest.name : "No MenuPostTest found");
        }

        // Auto-assign buttons
        if (buttonNext == null)
        {
            buttonNext = GameObject.Find("ButtonNext") ?? GameObject.Find("NextButton") ?? GameObject.Find("Text Poke Button Next");
            Debug.Log(buttonNext != null ? "Found buttonNext: " + buttonNext.name : "No buttonNext found");
        }

        if (buttonPrevious == null)
        {
            buttonPrevious = GameObject.Find("ButtonPrevious") ?? GameObject.Find("PreviousButton") ?? GameObject.Find("Text Poke Button Previous");
            Debug.Log(buttonPrevious != null ? "Found buttonPrevious: " + buttonPrevious.name : "No buttonPrevious found");
        }

        // Auto-assign text display
        if (textDisplay == null)
        {
            textDisplay = FindObjectOfType<TextMeshProUGUI>();
            Debug.Log(textDisplay != null ? "Found textDisplay: " + textDisplay.name : "No textDisplay found");
        }

        // Auto-assign image component
        if (ImageComponent == null)
        {
            ImageComponent = FindObjectOfType<Image>();
            Debug.Log(ImageComponent != null ? "Found ImageComponent: " + ImageComponent.name : "No ImageComponent found");
        }

        // Auto-assign visualization components
        if (MoonLineShading == null)
        {
            MoonLineShading = GameObject.Find("MoonLineShading") ?? GameObject.Find("Moon_Line");
            Debug.Log(MoonLineShading != null ? "Found MoonLineShading: " + MoonLineShading.name : "No MoonLineShading found");
        }

        if (MoonLineEarthFacing == null)
        {
            MoonLineEarthFacing = GameObject.Find("MoonLineEarthFacing") ?? GameObject.Find("Moon_Line_Earth");
            Debug.Log(MoonLineEarthFacing != null ? "Found MoonLineEarthFacing: " + MoonLineEarthFacing.name : "No MoonLineEarthFacing found");
        }

        if (MoonLineParallels == null)
        {
            MoonLineParallels = GameObject.Find("MoonLineParallels") ?? GameObject.Find("Moon_Line_Parallels");
            Debug.Log(MoonLineParallels != null ? "Found MoonLineParallels: " + MoonLineParallels.name : "No MoonLineParallels found");
        }

        if (SunDirectionVis == null)
        {
            SunDirectionVis = GameObject.Find("SunDirectionVis") ?? GameObject.Find("Sun_Direction");
            Debug.Log(SunDirectionVis != null ? "Found SunDirectionVis: " + SunDirectionVis.name : "No SunDirectionVis found");
        }

        // Create default positions if missing
        if (BirdViewPosition == null)
        {
            GameObject birdView = new GameObject("BirdViewPosition");
            birdView.transform.position = new Vector3(0, 10, 0);
            BirdViewPosition = birdView.transform;
            Debug.Log("Created default BirdViewPosition");
        }

        if (EarthViewPosition == null)
        {
            GameObject earthView = new GameObject("EarthViewPosition");
            earthView.transform.position = new Vector3(0, 0, 0);
            EarthViewPosition = earthView.transform;
            Debug.Log("Created default EarthViewPosition");
        }

        if (LookDirectionEarth == null)
        {
            GameObject lookEarth = new GameObject("LookDirectionEarth");
            lookEarth.transform.position = new Vector3(0, 0, 0);
            LookDirectionEarth = lookEarth.transform;
            Debug.Log("Created default LookDirectionEarth");
        }

        if (LookDirectionBirdView == null)
        {
            GameObject lookBirdView = new GameObject("LookDirectionBirdView");
            lookBirdView.transform.position = new Vector3(0, 0, 0);
            LookDirectionBirdView = lookBirdView.transform;
            Debug.Log("Created default LookDirectionBirdView");
        }

        if (PlanetPostionBirdView == null)
        {
            GameObject planetBirdView = new GameObject("PlanetPostionBirdView");
            planetBirdView.transform.position = new Vector3(0, 0, 0);
            PlanetPostionBirdView = planetBirdView.transform;
            Debug.Log("Created default PlanetPostionBirdView");
        }

        if (MoonPosition_Intro == null)
        {
            GameObject moonIntro = new GameObject("MoonPosition_Intro");
            moonIntro.transform.position = new Vector3(5, 0, 0);
            MoonPosition_Intro = moonIntro.transform;
            Debug.Log("Created default MoonPosition_Intro");
        }

        if (MoonPosition_Mission1 == null)
        {
            GameObject moonMission1 = new GameObject("MoonPosition_Mission1");
            moonMission1.transform.position = new Vector3(5, 0, 0);
            MoonPosition_Mission1 = moonMission1.transform;
            Debug.Log("Created default MoonPosition_Mission1");
        }

        if (MoonPosition_Mission2 == null)
        {
            GameObject moonMission2 = new GameObject("MoonPosition_Mission2");
            moonMission2.transform.position = new Vector3(5, 0, 0);
            MoonPosition_Mission2 = moonMission2.transform;
            Debug.Log("Created default MoonPosition_Mission2");
        }

        if (BirdViewMoonPosition == null)
        {
            GameObject birdViewMoon = new GameObject("BirdViewMoonPosition");
            birdViewMoon.transform.position = new Vector3(0, 5, 0);
            BirdViewMoonPosition = birdViewMoon.transform;
            Debug.Log("Created default BirdViewMoonPosition");
        }

        Debug.Log("[Moonphases_Manager] ===== AUTO-ASSIGN COMPONENTS COMPLETED =====");
        Debug.Log("[Moonphases_Manager] Final component status:");
        Debug.Log("[Moonphases_Manager] - CameraToMove: " + (CameraToMove != null ? "✓ " + CameraToMove.name : "✗ NULL"));
        Debug.Log("[Moonphases_Manager] - Moon_Body: " + (Moon_Body != null ? "✓ " + Moon_Body.name : "✗ NULL"));
        Debug.Log("[Moonphases_Manager] - SceneContent: " + (SceneContent != null ? "✓ " + SceneContent.name : "✗ NULL"));
        Debug.Log("[Moonphases_Manager] - PlanetSystem: " + (PlanetSystem != null ? "✓ " + PlanetSystem.name : "✗ NULL"));
        Debug.Log("[Moonphases_Manager] - BirdViewPosition: " + (BirdViewPosition != null ? "✓ " + BirdViewPosition.position : "✗ NULL"));
        Debug.Log("[Moonphases_Manager] - LookDirectionEarth: " + (LookDirectionEarth != null ? "✓ " + LookDirectionEarth.position : "✗ NULL"));
        
        // Analyze scene hierarchy
        Debug.Log("[Moonphases_Manager] ===== SCENE HIERARCHY ANALYSIS =====");
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        Debug.Log("[Moonphases_Manager] Total GameObjects in scene: " + allObjects.Length);
        
        // Log all root objects
        Debug.Log("[Moonphases_Manager] Root GameObjects:");
        foreach (var obj in allObjects)
        {
            if (obj.transform.parent == null)
            {
                Debug.Log("[Moonphases_Manager] - Root: " + obj.name + " (active:" + obj.activeInHierarchy + ")");
            }
        }
        
        // Log objects with specific keywords
        string[] keywords = { "camera", "moon", "earth", "sun", "scene", "planet", "menu", "canvas", "ui" };
        foreach (string keyword in keywords)
        {
            Debug.Log("[Moonphases_Manager] Objects containing '" + keyword + "':");
            foreach (var obj in allObjects)
            {
                if (obj.name.ToLower().Contains(keyword))
                {
                    Debug.Log("[Moonphases_Manager] - " + obj.name + " at " + obj.transform.position + " (active:" + obj.activeInHierarchy + ")");
                }
            }
        }
        
        Debug.Log("[Moonphases_Manager] ===== END SCENE ANALYSIS =====");
    }

    /// <summary>
    /// VisionOS-specific initialization to ensure proper setup for Apple Vision Pro
    /// </summary>
    private void InitializeForVisionOS()
    {
        Debug.Log("[Moonphases_Manager] ===== VISIONOS INITIALIZATION =====");
        
        // Check if we're running on visionOS
        bool isVisionOS = Application.platform == RuntimePlatform.VisionOS;
        Debug.Log($"[Moonphases_Manager] Running on visionOS: {isVisionOS}");
        
        // Ensure all UI canvases are in World Space mode for visionOS
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        foreach (var canvas in allCanvases)
        {
            if (canvas.renderMode != RenderMode.WorldSpace)
            {
                Debug.LogWarning($"[Moonphases_Manager] Canvas '{canvas.name}' is not in World Space mode - converting for visionOS compatibility");
                canvas.renderMode = RenderMode.WorldSpace;
            }
        }
        
        // Check for VolumeCamera (required for visionOS)
        var volumeCameras = FindObjectsOfType<Unity.PolySpatial.VolumeCamera>();
        if (volumeCameras.Length == 0)
        {
            Debug.LogError("[Moonphases_Manager] NO VOLUMECAMERA FOUND! This is required for visionOS apps.");
        }
        else
        {
            Debug.Log($"[Moonphases_Manager] Found {volumeCameras.Length} VolumeCamera(s) - visionOS rendering should work");
        }
        
        // Ensure proper layer setup for visionOS
        if (SceneContent != null)
        {
            // Make sure scene content is on the default layer for proper rendering
            SceneContent.gameObject.layer = 0;
        }
        
        // Check for PolySpatial settings
        var polySpatialSettings = Resources.Load<Unity.PolySpatial.PolySpatialSettings>("PolySpatialSettings");
        if (polySpatialSettings != null)
        {
            Debug.Log($"[Moonphases_Manager] PolySpatial settings found - version {polySpatialSettings.PackageVersion}");
        }
        else
        {
            Debug.LogWarning("[Moonphases_Manager] PolySpatialSettings not found - ensure PolySpatial package is properly installed");
        }
        
        Debug.Log("[Moonphases_Manager] ===== VISIONOS INITIALIZATION COMPLETED =====");
    }

    // Update is called once per frame
    void Update()
    {
        // Debug camera and moon positions every 60 frames (once per second at 60fps)
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log("[Moonphases_Manager] Update - Frame " + Time.frameCount + " - AppState: " + appState);
            Debug.Log("[Moonphases_Manager] Camera position: " + (CameraToMove != null ? CameraToMove.position.ToString() : "NULL"));
            Debug.Log("[Moonphases_Manager] Moon position: " + (Moon_Body != null ? Moon_Body.position.ToString() : "NULL"));
            Debug.Log("[Moonphases_Manager] SceneContent active: " + (SceneContent != null ? SceneContent.gameObject.activeInHierarchy.ToString() : "NULL"));
        }
        
        if (appState == AppState.Tutorial_Overview || appState == AppState.Tutorial_MenuSelect || appState == AppState.Tutorial_Menudown
            || appState == AppState.Intro  || appState == AppState.TransitionFromFoam)
        {
            if (Moon_Body != null && MoonPosition_Intro != null)
            {
                Moon_Body.position = MoonPosition_Intro.position;
            }
            else
            {
                Debug.LogError("[Moonphases_Manager] Cannot position moon - Moon_Body: " + (Moon_Body != null ? "OK" : "NULL") + ", MoonPosition_Intro: " + (MoonPosition_Intro != null ? "OK" : "NULL"));
            }
        }

        else if (appState == AppState.TaskTwo_Intro || appState == AppState.TaskTwo_Freeplay || appState == AppState.TaskTwo_Question
            || appState == AppState.TaskTwo_WrongAnswer || appState == AppState.TaskTwo_CorrectAnswer || appState == AppState.EndOfApp)
        {
            Moon_Body.position = MoonPosition_Mission2.position;
        }

        else
        {
            Moon_Body.position = MoonPosition_Mission1.position;
        }



        if (appState == AppState.TaskOne_Perspective || appState == AppState.TaskOne_Intro || appState == AppState.TaskOne_ObjectConstellation 
            || appState == AppState.TaskOne_ObjectConstellationFoam || appState == AppState.TaskOne_MentalPerspectiveChange || appState == AppState.TaskOne_QuestionBirdView
            || appState == AppState.TaskOne_QuestionBirdViewRight || appState == AppState.TaskOne_QuestionBirdViewWrong)
        {
            SceneContent.position = EarthViewPosition.position;
            SceneContent.rotation = Quaternion.Euler(0, -90, 0);
            PlanetSystem.localPosition = new Vector3(0, 0, 0);
            PlanetSystem.localRotation = Quaternion.Euler(0, 0, 0);

        }

        else if (appState == AppState.TaskOne_PerspectiveChange || appState == AppState.TaskOne_ExplanationIntro || appState == AppState.TaskOne_StepShading
            || appState == AppState.TaskOne_StepLine || appState == AppState.TaskOne_StepLightSide || appState == AppState.TaskOne_StepLighting || appState == AppState.TaskOne_ExplanationLighting
            || appState == AppState.TaskOne_ExplanationLightSide || appState == AppState.TaskOne_ExplanationLine || appState == AppState.TaskOne_ExplanationShading || appState == AppState.TaskOne_QuestionLighting
            || appState == AppState.TaskOne_QuestionLightSide || appState == AppState.TaskOne_QuestionLine || appState == AppState.TaskOne_QuestionShading || appState == AppState.TaskOne_ShadingRight
            || appState == AppState.TaskOne_ShadingWrong || appState == AppState.TaskOne_LineRight || appState == AppState.TaskOne_LineWrong || appState == AppState.TaskOne_LightingRight || appState == AppState.TaskOne_LightingWrong
            || appState == AppState.TaskOne_LightSideRight || appState == AppState.TaskOne_LightSideWrong || appState == AppState.TaskOne_ExplanationMoonphase || appState == AppState.TaskOne_MoonphaseRight
            || appState == AppState.TaskOne_MoonphaseWrong || appState == AppState.TaskOne_ChangeToPaper)
        {
            SceneContent.position = new Vector3(0, 0, 0);
            SceneContent.rotation = Quaternion.Euler(0, 0, 0);
            PlanetSystem.localPosition = PlanetPostionBirdView.position;
            PlanetSystem.localRotation = Quaternion.Euler(-90, 0, 0);
        }
        
        else
        {
            SceneContent.position = new Vector3(0, 0, 0);
            SceneContent.rotation = Quaternion.Euler(0, 0, 0);
            PlanetSystem.localPosition = new Vector3(0, 0, 0);
            PlanetSystem.localRotation = Quaternion.Euler(0, 0, 0);
        }

    }

    private void InitScreenAndMenu()
    {
        MenuCore.SetActive(true);
        MenuExtended.SetActive(true);
        MenuImage.SetActive(false);
        MenuQuestion.SetActive(false);
        buttonNext.SetActive(true);
        buttonPrevious.SetActive(true);

    }

    private void LoadTextFromFile()
    {
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);

        if(System.IO.File.Exists(filePath))
        {
            allLines = System.IO.File.ReadAllLines(filePath);
        }
        else
        {
            textDisplay.text = "Datei nicht gefunden! ";
        }
    }

    private void DisplayText(int lineIndex)
    {
        if (lineIndex < allLines.Length)
        {
            textDisplay.text = allLines[lineIndex];
        }
        else
        {
            textDisplay.text = "Zeilenindex au�erhalb des Bereichs! ";
        }
    }

    private void ShowSprite(int index)
    {
        if (ImageComponent == null)
        {
            Debug.LogError("[Moonphases_Manager] ImageComponent is null! Cannot show sprite.");
            return;
        }
        
        if (sprites != null && index < sprites.Length)
        {
            ImageComponent.sprite = sprites[index];
            currentSprite = index;
        }
        else
        {
            Debug.LogWarning($"[Moonphases_Manager] Sprite index {index} out of range. sprites length: {(sprites != null ? sprites.Length : 0)}");
        }
    }

    public void PlayAudio(int audioIndex)
    {
        if (audioSource == null)
        {
            Debug.LogError("[Moonphases_Manager] audioSource is null! Cannot play audio.");
            return;
        }
        
        if (audioClips != null && audioIndex < audioClips.Length)
        {
            audioSource.clip = audioClips[audioIndex];
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning($"[Moonphases_Manager] Audio index {audioIndex} out of range. audioClips length: {(audioClips != null ? audioClips.Length : 0)}");
        }
    }

    #endregion

    #region Handling of AppState and Menu

    private void AppStateHandler()
    {
        if (appState == AppState.Tutorial_Overview)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(1);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }
    //display txt /line

//deactivate/reactivate per menu/appstates
        if (appState == AppState.Tutorial_MenuSelect)
        {
            SetActiveMenues(true, true, true, true, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(2);
            ShowSprite(0);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
         
        }

        if (appState == AppState.Tutorial_Menudown)
        {
            SetActiveMenues(true, true, true, true, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(3);
            ShowSprite(1);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.Tutorial_Exploremove)
        {
            SetActiveMenues(true, true, true, true, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(4);
            ShowSprite(2);
            CameraZoomActive = false;
            ChangePerspectiveActive = true;
        }

        if (appState == AppState.Tutorial_Explorescroll)
        {
            SetActiveMenues(true, true, true, true, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(5);
            ShowSprite(3);
            CameraZoomActive = true;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TransitionFromFoam)
        {
            SetActiveMenues(true, true, false, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.Intro)
        {
            SetActiveMenues(true, true, false, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(6);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.Experiment_Intro)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(0);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.Experiment_EarthView)
        {
            SetActiveMenues(true, true, false, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(0);
            CameraZoomActive = true;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_SunDirection)
        {
            SetActiveMenues(true, true, false, false, false, false, false);
            SetActiveVisuals(false, false, false, true, false, false, false);
            DisplayText(7);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_Perspective)
        {
            SetActiveMenues(true, true, false, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(8);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_Intro)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(9);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_ObjectConstellation)
        {
            SetActiveMenues(true, true, false, false, false, false, false);
            SetActiveVisuals(false, false, false, false, true, false, false);
            DisplayText(10);
            CameraZoomActive = true;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_ObjectConstellationEarth)
        {
            SetActiveMenues(true, true, false, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(0);
            CameraZoomActive = true;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_ObjectConstellationMoon)
        {
            SetActiveMenues(true, true, false, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(0);
            CameraZoomActive = true;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_ObjectConstellationFoam)
        {
            SetActiveMenues(true, true, false, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, true, false);
            DisplayText(11);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_MentalPerspectiveChange)
        {
            SetActiveMenues(true, true, false, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, true, false);
            DisplayText(12);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_QuestionBirdView)
        {
            SetActiveMenues(true, false, true, true, true, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(13);
            ShowSprite(4);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_QuestionBirdViewWrong)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(14);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_QuestionBirdViewRight)
        {
            SetActiveMenues(true, true, true, true, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(15);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_PerspectiveChange)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(16);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_ExplanationIntro)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(17);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_ExplanationPerspective)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(0);
            CameraZoomActive = true;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_StepShading)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(18);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_ExplanationShading)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(true, false, false, true, false, false, false);
            DisplayText(19);
            CameraZoomActive = true;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_QuestionShading)
        {
            SetActiveMenues(true, false, true, true, true, false, false);
            SetActiveVisuals(false, false, false, false, false, false,false);
            DisplayText(18);
            ShowSprite(9);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_ShadingWrong)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(14);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_ShadingRight)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(15);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_StepLine)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, true, true, true, false, false, false);
            DisplayText(20);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_ExplanationLine)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, true, true, true, false, false, false);
            DisplayText(21);
            CameraZoomActive = true;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_QuestionLine)
        {
            SetActiveMenues(true, false, true, true, true, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(20);
            ShowSprite(10);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_LineWrong)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(14);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_LineRight)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false,false);
            DisplayText(15);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_StepLighting)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, true, false, false, false, false, false);
            DisplayText(22);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_ExplanationLighting)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, true, false, false, false, false, false);
            DisplayText(23);
            CameraZoomActive = true;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_QuestionLighting)
        {
            SetActiveMenues(true, false, true, true, true, false, false);
            SetActiveVisuals(false, false, false, false, false, false,false);
            DisplayText(22);
            ShowSprite(11);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_LightingWrong)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, false, false, false , false, false, false);
            DisplayText(14);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_LightingRight)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(15);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_StepLightSide)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, true);
            DisplayText(24);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_ExplanationLightSide)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, true, false, false, false, false, true);
            DisplayText(25);
            CameraZoomActive = true;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_QuestionLightSide)
        {
            SetActiveMenues(true, false, true, true, false, true, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(24);
            ShowSprite(12);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_LightSideWrong)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(14);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_LightSideRight)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(15);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_ExplanationMoonphase)
        {
            SetActiveMenues(true, false, true, true, true, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(26);
            ShowSprite(5);
            CameraZoomActive = true;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_MoonphaseWrong)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(27);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_MoonphaseRight)
        {
            SetActiveMenues(true, true, true, true, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(28);
            ShowSprite(7);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_ChangeToPaper)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(29);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskOne_FreeExploration)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(0);
            CameraZoomActive = true;
            ChangePerspectiveActive = true;
        }

        if (appState == AppState.TaskTwo_Intro)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(30);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskTwo_Freeplay)
        {
            SetActiveMenues(true, true, false, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(0);
            CameraZoomActive = true;
            ChangePerspectiveActive = false;
        }

        if (appState  == AppState.TaskTwo_Question)
        {
            SetActiveMenues(true, false, true, true, true, false, false);
            SetActiveVisuals(false, false, false, true, false, false, false);
            DisplayText(31);
            ShowSprite(5);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskTwo_WrongAnswer)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(32);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.TaskTwo_CorrectAnswer)
        {
            SetActiveMenues(true,true,true,true,false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(33);
            ShowSprite(8);
            CameraZoomActive = false;
            ChangePerspectiveActive = false;
        }

        if (appState == AppState.EndOfApp)
        {
            SetActiveMenues(true, true, true, false, false, false, false);
            SetActiveVisuals(false, false, false, false, false, false, false);
            DisplayText(34);
            CameraZoomActive = true;
            ChangePerspectiveActive = true;
            // Automatically transition to MenuPostTest after a short delay
            StartCoroutine(TransitionToMenuPostTest());
        }

        if (appState == AppState.MenuPostTest)
        {
            // Check if MenuPostTest exists and is properly configured
            if (MenuPostTest != null && MenuPostTest.activeInHierarchy)
            {
                SetActiveMenues(false, false, false, false, false, false, true); // Only MenuPostTest active
                SetActiveVisuals(false, false, false, false, false, false, false);
                Debug.Log("[Moonphases_Manager] MenuPostTest state activated successfully");
            }
            else
            {
                // Fallback: keep some UI visible to prevent white screen
                Debug.LogWarning("[Moonphases_Manager] MenuPostTest is null or inactive! Falling back to main menu.");
                SetActiveMenues(true, true, true, false, false, false, false);
                SetActiveVisuals(false, false, false, false, false, false, false);
                DisplayText(34); // Show end of app text
            }
            return;
        }

    }

    private void MenuButtonHandler()  
    {
        switch (appState)
        {
            case AppState.Tutorial_Overview:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(false);
                break;

            case AppState.Tutorial_MenuSelect:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(true);
                break;

            case AppState.Tutorial_Menudown:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(true);
                break;

            case AppState.Tutorial_Exploremove:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(true);
                break;

            case AppState.Tutorial_Explorescroll:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TransitionFromFoam:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(true);
                break;

            case AppState.Intro:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(false);
                break;

            case AppState.Experiment_Intro:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(false);
                break;

            case AppState.Experiment_EarthView:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskOne_Intro:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(false);
                break;

            case AppState.TaskOne_SunDirection:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskOne_Perspective:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskOne_ObjectConstellation:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskOne_ObjectConstellationFoam:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskOne_ObjectConstellationEarth:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskOne_ObjectConstellationMoon:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskOne_MentalPerspectiveChange:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskOne_QuestionBirdView:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(false);
                break;

            case AppState.TaskOne_QuestionBirdViewWrong:
                buttonNext.SetActive(false);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskOne_QuestionBirdViewRight:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(false);
                break;

            case AppState.TaskOne_PerspectiveChange:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskOne_ChangeToPaper:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskOne_ExplanationIntro:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(false);
                break;

            case AppState.TaskOne_ExplanationPerspective:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskOne_StepShading:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(true);
                break;            

            case AppState.TaskOne_ExplanationShading:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskOne_QuestionShading:
                buttonNext.SetActive(false);
                buttonPrevious.SetActive(false);
                break;

            case AppState.TaskOne_ShadingWrong:
                buttonNext.SetActive(false);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskOne_ShadingRight:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(false);
                break;

            case AppState.TaskOne_StepLine:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskOne_ExplanationLine:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskOne_QuestionLine:
                buttonNext.SetActive(false);
                buttonPrevious.SetActive(false);
                break;

            case AppState.TaskOne_LineWrong:
                buttonNext.SetActive(false);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskOne_LineRight:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(false);
                break;

            case AppState.TaskOne_StepLighting:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskOne_ExplanationLighting:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskOne_QuestionLighting:
                buttonNext.SetActive(false);
                buttonPrevious.SetActive(false);
                break;

            case AppState.TaskOne_LightingWrong:
                buttonNext.SetActive(false);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskOne_LightingRight:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(false);
                break;

            case AppState.TaskOne_StepLightSide:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskOne_ExplanationLightSide:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskOne_QuestionLightSide:
                buttonNext.SetActive(false);
                buttonPrevious.SetActive(false);
                break;

            case AppState.TaskOne_LightSideWrong:
                buttonNext.SetActive(false);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskOne_LightSideRight:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(false);
                break;

            case AppState.TaskOne_ExplanationMoonphase:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskOne_MoonphaseWrong:
                buttonNext.SetActive(false);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskOne_MoonphaseRight:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(false);
                break;

            case AppState.TaskOne_FreeExploration:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(false);
                break;

            case AppState.TaskTwo_Intro:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(false);
                break;

            case AppState.TaskTwo_Freeplay:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskTwo_Question:
                buttonNext.SetActive(false);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskTwo_WrongAnswer:
                buttonNext.SetActive(false);
                buttonPrevious.SetActive(true);
                break;

            case AppState.TaskTwo_CorrectAnswer:
                buttonNext.SetActive(true);
                buttonPrevious.SetActive(false);
                break;

            case AppState.EndOfApp:
                buttonNext.SetActive(false);
                buttonPrevious.SetActive(false);
                break; 
                
            case AppState.MenuPostTest:
                buttonNext.SetActive(false);
                buttonPrevious.SetActive(false);
                break;

        }
    }

    private void SetActiveMenues(bool main, bool core, bool extended, bool image, bool question, bool leftright, bool posttest)
    {
        // Add null checks to prevent errors
        if (MenuMain != null) MenuMain.SetActive(main);
        if (MenuCore != null) MenuCore.SetActive(core);
        if (MenuExtended != null) MenuExtended.SetActive(extended);
        if (MenuImage != null) MenuImage.SetActive(image);
        if (MenuQuestion != null) MenuQuestion.SetActive(question);
        if (MenuLeftRight != null) MenuLeftRight.SetActive(leftright);
        if (MenuPostTest != null) MenuPostTest.SetActive(posttest);
        
        // Log the state for debugging
        Debug.Log($"[Moonphases_Manager] SetActiveMenues - Main:{main}, Core:{core}, Extended:{extended}, Image:{image}, Question:{question}, LeftRight:{leftright}, PostTest:{posttest}");
    }

    private void SetActiveVisuals(bool moonlineshade, bool moonlineearth, bool moonlineparallel, bool sundirection, bool labelconstellation, bool labelconstellationfoam, bool equator)
    {
        // Add null checks to prevent errors
        if (MoonLineShading != null) MoonLineShading.SetActive(moonlineshade);
        if (MoonLineEarthFacing != null) MoonLineEarthFacing.SetActive(moonlineearth);
        if (MoonLineParallels != null) MoonLineParallels.SetActive(moonlineparallel);
        if (SunDirectionVis != null) SunDirectionVis.SetActive(sundirection);
        if (Label_ObjectConstellation != null) Label_ObjectConstellation.SetActive(labelconstellation);
        if (Label_ObjectConstellationFoam != null) Label_ObjectConstellationFoam.SetActive(labelconstellationfoam);
        if (EquatorLine != null) EquatorLine.SetActive(equator);
    }

    public void ToggleMenu()
    {
        if (!MenuMain.activeSelf && !MenuCore.activeSelf && !MenuExtended.activeSelf && !MenuImage.activeSelf && !MenuQuestion.activeSelf && !MenuLeftRight.activeSelf)
        {
            AppStateHandler();
        }

        else
        {
            SetActiveMenues(false, false, false, false, false, false, false);
        }
    }

    #endregion

    #region Events

    public void OnButtonBack()
    {
        switch (appState)
        {
            case AppState.Tutorial_MenuSelect:
                appState = AppState.Tutorial_Overview;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.Tutorial_Menudown:
                appState = AppState.Tutorial_MenuSelect;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TransitionFromFoam:
                appState = AppState.Tutorial_Menudown;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.Intro:
                appState = AppState.TransitionFromFoam;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(1);
                break;

            case AppState.TaskOne_SunDirection:
                appState = AppState.Intro;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(2);
                break;

            case AppState.TaskOne_Perspective:
                appState = AppState.TaskOne_SunDirection;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(3);
                break;

            case AppState.TaskOne_Intro:
                appState = AppState.TaskOne_Perspective;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(4);
                break;

            case AppState.TaskOne_ObjectConstellation:
                appState = AppState.TaskOne_Intro;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(5);
                break;

            case AppState.TaskOne_ObjectConstellationFoam:
                appState = AppState.TaskOne_ObjectConstellation;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(6);
                break;

            case AppState.TaskOne_MentalPerspectiveChange:
                appState = AppState.TaskOne_ObjectConstellationFoam;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_QuestionBirdView:
                appState = AppState.TaskOne_MentalPerspectiveChange;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(7);
                break;

            case AppState.TaskOne_QuestionBirdViewWrong:
                appState = AppState.TaskOne_QuestionBirdView;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_QuestionBirdViewRight:
                appState = AppState.TaskOne_QuestionBirdView;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_PerspectiveChange:
                appState = AppState.TaskOne_QuestionBirdView;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_ExplanationIntro:
                appState = AppState.TaskOne_PerspectiveChange;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(10);
                break;

            case AppState.TaskOne_StepShading:
                appState = AppState.TaskOne_ExplanationIntro;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(11);
                break;

            case AppState.TaskOne_ExplanationShading:
                appState = AppState.TaskOne_StepShading;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(12);
                break;

            case AppState.TaskOne_QuestionShading:
                appState = AppState.TaskOne_ExplanationShading;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(13);
                break;

            case AppState.TaskOne_ShadingWrong:
                appState = AppState.TaskOne_QuestionShading;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_ShadingRight:
                appState = AppState.TaskOne_QuestionShading;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_StepLine:
                appState = AppState.TaskOne_ShadingRight;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_ExplanationLine:
                appState = AppState.TaskOne_StepLine;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(14);
                break;

            case AppState.TaskOne_QuestionLine:
                appState = AppState.TaskOne_ExplanationLine;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(15);
                break;

            case AppState.TaskOne_LineWrong:
                appState = AppState.TaskOne_QuestionLine;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_LineRight:
                appState = AppState.TaskOne_QuestionLine;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_StepLighting:
                appState = AppState.TaskOne_LineRight;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_ExplanationLighting:
                appState = AppState.TaskOne_StepLighting;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(16);
                break;

            case AppState.TaskOne_QuestionLighting:
                appState = AppState.TaskOne_ExplanationLighting;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(17);
                break;

            case AppState.TaskOne_LightingWrong:
                appState = AppState.TaskOne_QuestionLighting;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_LightingRight:
                appState = AppState.TaskOne_QuestionLighting;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_StepLightSide:
                appState = AppState.TaskOne_LightingRight;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_ExplanationLightSide:
                appState = AppState.TaskOne_StepLightSide;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(18);
                break;

            case AppState.TaskOne_QuestionLightSide:
                appState = AppState.TaskOne_ExplanationLightSide;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(19);
                break;

            case AppState.TaskOne_LightSideWrong:
                appState = AppState.TaskOne_QuestionLightSide;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_LightSideRight:
                appState = AppState.TaskOne_QuestionLightSide;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_ExplanationMoonphase:
                appState = AppState.TaskOne_LightSideRight;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_MoonphaseWrong:
                appState = AppState.TaskOne_ExplanationMoonphase;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_MoonphaseRight:
                appState = AppState.TaskOne_ChangeToPaper;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(20);
                break;

            case AppState.TaskOne_ChangeToPaper:
                appState = AppState.TaskTwo_Intro;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(21);
                break;

            case AppState.TaskTwo_Intro:
                appState = AppState.TaskTwo_Question;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(22);
                break;

            case AppState.TaskTwo_Question:
                appState = AppState.TaskTwo_WrongAnswer;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskTwo_WrongAnswer:
                appState = AppState.TaskTwo_Question;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskTwo_CorrectAnswer:
                appState = AppState.TaskTwo_Question;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.EndOfApp:
                appState = AppState.TaskTwo_Question;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(24);
                break;       

        }
    }

    public void OnButtonNext()
    {
        switch (appState)
        {
            case AppState.Tutorial_Overview:
                appState = AppState.Tutorial_MenuSelect;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.Tutorial_MenuSelect:
                appState = AppState.Tutorial_Menudown;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.Tutorial_Menudown:
                appState = AppState.TransitionFromFoam;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(1);
                break;

            case AppState.TransitionFromFoam:
                appState = AppState.Intro;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(2);
                break;

            case AppState.Intro:
                appState = AppState.TaskOne_SunDirection;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(3);
                break;

            case AppState.TaskOne_SunDirection:
                appState = AppState.TaskOne_Perspective;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(4);
                break;

            case AppState.TaskOne_Perspective:
                appState = AppState.TaskOne_Intro;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(5);
                break;

            case AppState.TaskOne_Intro:
                appState = AppState.TaskOne_ObjectConstellation;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(6);
                break;

            case AppState.TaskOne_ObjectConstellation:
                appState = AppState.TaskOne_ObjectConstellationFoam;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_ObjectConstellationFoam:
                appState = AppState.TaskOne_MentalPerspectiveChange;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(7);
                break;

            case AppState.TaskOne_MentalPerspectiveChange:
                appState = AppState.TaskOne_QuestionBirdView;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_QuestionBirdView:
                appState = AppState.TaskOne_QuestionBirdViewWrong;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_QuestionBirdViewWrong:
                appState = AppState.TaskOne_QuestionBirdViewRight;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_QuestionBirdViewRight:
                appState = AppState.TaskOne_PerspectiveChange;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(10);
                break;

            case AppState.TaskOne_PerspectiveChange:
                appState = AppState.TaskOne_ExplanationIntro;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(11);
                break;

            case AppState.TaskOne_ExplanationIntro:
                appState = AppState.TaskOne_StepShading;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(12);
               // CameraToMove.LookAt(LookDirectionBirdView.position);
                break;

            case AppState.TaskOne_StepShading:
                appState = AppState.TaskOne_ExplanationShading;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(13);
                break;

            case AppState.TaskOne_ExplanationShading:
                appState = AppState.TaskOne_QuestionShading;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_QuestionShading:
                appState = AppState.TaskOne_ShadingWrong;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_ShadingWrong:
                appState = AppState.TaskOne_ShadingRight;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_ShadingRight:
                appState = AppState.TaskOne_StepLine;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(14);
                break;

            case AppState.TaskOne_StepLine:
                appState = AppState.TaskOne_ExplanationLine;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(15);
                break;

            case AppState.TaskOne_ExplanationLine:
                appState = AppState.TaskOne_QuestionLine;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_QuestionLine:
                appState = AppState.TaskOne_LineWrong;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_LineWrong:
                appState = AppState.TaskOne_LineRight;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_LineRight:
                appState = AppState.TaskOne_StepLighting;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(16);
                break;

            case AppState.TaskOne_StepLighting:
                appState = AppState.TaskOne_ExplanationLighting;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(17);
                break;

            case AppState.TaskOne_ExplanationLighting:
                appState = AppState.TaskOne_QuestionLighting;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_QuestionLighting:
                appState = AppState.TaskOne_LightingWrong;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_LightingWrong:
                appState = AppState.TaskOne_LightingRight;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_LightingRight:
                appState = AppState.TaskOne_StepLightSide;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(18);
                break;

            case AppState.TaskOne_StepLightSide:
                appState = AppState.TaskOne_ExplanationLightSide;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(19);
                break;


            case AppState.TaskOne_ExplanationLightSide:
                appState = AppState.TaskOne_QuestionLightSide;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_QuestionLightSide:
                appState = AppState.TaskOne_LightSideWrong;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_LightSideWrong:
                appState = AppState.TaskOne_LightSideRight;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_LightSideRight:
                appState = AppState.TaskOne_ExplanationMoonphase;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_ExplanationMoonphase:
                appState = AppState.TaskOne_MoonphaseWrong;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_MoonphaseRight:
                appState = AppState.TaskOne_ChangeToPaper;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(20);
                break;

            case AppState.TaskOne_ChangeToPaper:
                appState = AppState.TaskTwo_Intro;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(21);
                break;

            case AppState.TaskTwo_Intro:
                appState = AppState.TaskTwo_Question;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(24);
                break;

            case AppState.TaskTwo_CorrectAnswer:
                appState = AppState.EndOfApp;
                AppStateHandler();
                MenuButtonHandler();
                break;

        }
    }

    public void CameraZoom()
    {
        if (CameraZoomActive)
        {
            float zoomInput = Input.GetAxis("Zoom");
            float newZoom = MainCamera.fieldOfView - zoomInput * zoomSpeed * Time.deltaTime;
            newZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);
            MainCamera.fieldOfView = newZoom;
        }
        
    }

    public void ChangePerspective()
    {
        if (ChangePerspectiveActive)
        {
            float verticalInput = Input.GetAxis("XRI_Left_Primary2DAxis_Vertical");

            if (verticalInput > threshold)
            {
                if (!isUpPressed)
                {
                    isUpPressed = true;
                    isDownPressed = false;
                    CameraToMove.position = EarthViewPosition.position;
                    CameraToMove.LookAt(LookDirectionBirdView.position);
                }
            }

            if (verticalInput < -threshold)
            {
                if (!isDownPressed)
                {
                    isDownPressed = true;
                    isUpPressed = false;
                    CameraToMove.position = BirdViewPosition.position;
                    CameraToMove.LookAt(LookDirectionEarth.position);
                }
            }

            else
            {
                CameraToMove.position = CameraToMove.position;
            }
        } 
    }

    public void AnswerPressedA()
    {
        switch (appState)
        {
            case AppState.TaskOne_QuestionBirdView:
                appState = AppState.TaskOne_QuestionBirdViewWrong;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(8);
                break;

            case AppState.TaskOne_QuestionShading:
                appState = AppState.TaskOne_ShadingWrong;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_QuestionLine:
                appState = AppState.TaskOne_LineRight;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_QuestionLighting:
                appState = AppState.TaskOne_LightingWrong;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_ExplanationMoonphase:
                appState = AppState.TaskOne_MoonphaseWrong;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskTwo_Question:
                appState = AppState.TaskTwo_WrongAnswer;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(26);
                break;

        }
    }

    public void AnswerPressedB()
    {
        switch (appState)
        {
            case AppState.TaskOne_QuestionBirdView:
                appState = AppState.TaskOne_QuestionBirdViewWrong;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(8);
                break;

            case AppState.TaskOne_QuestionShading:
                appState = AppState.TaskOne_ShadingWrong;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_QuestionLine:
                appState = AppState.TaskOne_LineWrong;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_QuestionLighting:
                appState = AppState.TaskOne_LightingRight;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_ExplanationMoonphase:
                appState = AppState.TaskOne_MoonphaseWrong;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskTwo_Question:
                appState = AppState.TaskTwo_WrongAnswer;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(26);
                break;

        }
    }

    public void AnswerPressedC()
    {
        switch (appState)
        {
            case AppState.TaskOne_QuestionBirdView:
                appState = AppState.TaskOne_QuestionBirdViewRight;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(9);
                break;

            case AppState.TaskOne_QuestionShading:
                appState = AppState.TaskOne_ShadingWrong;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_QuestionLine:
                appState = AppState.TaskOne_LineWrong;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_QuestionLighting:
                appState = AppState.TaskOne_LightingWrong;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_ExplanationMoonphase:
                appState = AppState.TaskOne_MoonphaseWrong;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskTwo_Question:
                appState = AppState.TaskTwo_WrongAnswer;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(26);
                break;

        }
    }

    public void AnswerPressedD()
    {
        switch (appState)
        {
            case AppState.TaskOne_QuestionBirdView:
                appState = AppState.TaskOne_QuestionBirdViewWrong;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(8);
                break;

            case AppState.TaskOne_QuestionShading:
                appState = AppState.TaskOne_ShadingRight;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_QuestionLine:
                appState = AppState.TaskOne_LineWrong;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_QuestionLighting:
                appState = AppState.TaskOne_LightingWrong;
                AppStateHandler(); 
                MenuButtonHandler();
                break;

            case AppState.TaskOne_ExplanationMoonphase:
                appState = AppState.TaskOne_MoonphaseRight;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskTwo_Question:
                appState = AppState.TaskTwo_WrongAnswer;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(26);
                break;

        }
    }

    public void AnswerPressedE()
    {
        switch (appState)
        {
            case AppState.TaskOne_QuestionBirdView:
                appState = AppState.TaskOne_QuestionBirdViewWrong;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(8);
                break;

            case AppState.TaskOne_QuestionShading:
                appState = AppState.TaskOne_ShadingWrong;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_QuestionLine:
                appState = AppState.TaskOne_LineWrong;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_QuestionLighting:
                appState = AppState.TaskOne_LightingWrong;
                AppStateHandler();
                MenuButtonHandler();
                break;

            case AppState.TaskOne_ExplanationMoonphase:
                appState = AppState.TaskOne_MoonphaseWrong;
                AppStateHandler();
                MenuButtonHandler();
                break;


            case AppState.TaskTwo_Question:
                appState = AppState.TaskTwo_CorrectAnswer;
                AppStateHandler();
                MenuButtonHandler();
                PlayAudio(27);
                break;

        }
    }

    public void AnswerPressedLeft()
    {
        switch (appState)
        {
            case AppState.TaskOne_QuestionLightSide:
                appState = AppState.TaskOne_LightSideRight;
                AppStateHandler();
                MenuButtonHandler();
                break;
        }
    }

    public void AnswerPressedRight()
    {
        switch (appState)
        {
            case AppState.TaskOne_QuestionLightSide:
                appState = AppState.TaskOne_LightSideWrong;
                AppStateHandler();
                MenuButtonHandler();
                break;
        }
    }
    #endregion

    /// <summary>
    /// Emergency recovery method to fix white screen issues
    /// Call this from the inspector or via console if you get a white screen
    /// </summary>
    [ContextMenu("Emergency Recovery - Fix White Screen")]
    public void EmergencyRecovery()
    {
        Debug.Log("[Moonphases_Manager] EMERGENCY RECOVERY - Fixing white screen...");
        
        // Force activate all menu components
        if (MenuMain != null) MenuMain.SetActive(true);
        if (MenuCore != null) MenuCore.SetActive(true);
        if (MenuExtended != null) MenuExtended.SetActive(true);
        if (MenuImage != null) MenuImage.SetActive(false);
        if (MenuQuestion != null) MenuQuestion.SetActive(false);
        if (MenuLeftRight != null) MenuLeftRight.SetActive(false);
        if (MenuPostTest != null) MenuPostTest.SetActive(false);
        
        // Reset to a safe app state
        appState = AppState.Tutorial_Overview;
        
        // Call the state handler to properly set up the UI
        AppStateHandler();
        MenuButtonHandler();
        
        // Display some text to confirm it's working
        if (textDisplay != null)
        {
            textDisplay.text = "Emergency Recovery - App restored to Tutorial Overview state";
        }
        
        Debug.Log("[Moonphases_Manager] Emergency recovery completed. App should now be visible.");
    }

    /// <summary>
    /// Ensures the post test panel is completely hidden during the main application
    /// </summary>
    [ContextMenu("Hide Post Test Panel")]
    public void HidePostTestPanel()
    {
        Debug.Log("[Moonphases_Manager] Hiding post test panel...");
        
        // Hide the main MenuPostTest GameObject
        if (MenuPostTest != null)
        {
            MenuPostTest.SetActive(false);
            Debug.Log("[Moonphases_Manager] MenuPostTest hidden");
        }
        
        // Hide any post test canvases
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        foreach (var canvas in allCanvases)
        {
            if (canvas.name.ToLower().Contains("post") || 
                canvas.name.ToLower().Contains("test"))
            {
                canvas.gameObject.SetActive(false);
                Debug.Log($"[Moonphases_Manager] Hidden post test canvas: {canvas.name}");
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
                    Debug.Log($"[Moonphases_Manager] Hidden post test object: {obj.name}");
                }
            }
        }
        
        Debug.Log("[Moonphases_Manager] Post test panel hiding completed");
    }

    private IEnumerator TransitionToMenuPostTest()
    {
        Debug.Log("[Moonphases_Manager] Starting transition to MenuPostTest...");
        yield return new WaitForSeconds(2f); // Wait 2 seconds (adjust as needed)
        
        // Find and activate the post test setup BEFORE changing app state
        PostTestSetupFix postTestSetup = FindObjectOfType<PostTestSetupFix>();
        if (postTestSetup != null)
        {
            Debug.Log("[Moonphases_Manager] Found PostTestSetupFix, activating post test...");
            postTestSetup.OnMainAppComplete();
            
            // Wait a moment for the post test to initialize
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            Debug.LogWarning("[Moonphases_Manager] PostTestSetupFix not found! Post test may not launch properly.");
        }
        
        // Now change the app state
        appState = AppState.MenuPostTest;
        AppStateHandler();
        MenuButtonHandler();
        
        Debug.Log("[Moonphases_Manager] Transition to MenuPostTest completed. AppState: " + appState);
    }

}
