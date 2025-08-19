using UnityEngine;

/// <summary>
/// Simple test script to manually trigger the post test for testing purposes.
/// Attach this to any GameObject in the scene and use the Test Post Test button in the Inspector.
/// </summary>
public class PostTestTester : MonoBehaviour
{
    [Header("Test Controls")]
    [SerializeField] private bool autoStartPostTest = false;
    
    private void Start()
    {
        if (autoStartPostTest)
        {
            // Wait a moment then start the post test
            Invoke(nameof(TestPostTest), 1f);
        }
    }
    
    [ContextMenu("Test Post Test")]
    public void TestPostTest()
    {
        Debug.Log("[PostTestTester] Manually triggering post test...");
        
        // Find and activate the post test setup
        PostTestSetupFix postTestSetup = FindObjectOfType<PostTestSetupFix>();
        if (postTestSetup != null)
        {
            postTestSetup.OnMainAppComplete();
            postTestSetup.StartPostTest();
        }
        else
        {
            Debug.LogError("[PostTestTester] PostTestSetupFix not found! Make sure it's attached to your PostTestCanvas.");
        }
    }
    
    [ContextMenu("Show Post Test Canvas Only")]
    public void ShowPostTestCanvasOnly()
    {
        Debug.Log("[PostTestTester] Showing post test canvas only...");
        
        PostTestSetupFix postTestSetup = FindObjectOfType<PostTestSetupFix>();
        if (postTestSetup != null)
        {
            postTestSetup.OnMainAppComplete();
        }
        else
        {
            Debug.LogError("[PostTestTester] PostTestSetupFix not found!");
        }
    }
} 