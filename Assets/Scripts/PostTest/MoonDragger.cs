using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class MoonDragger : MonoBehaviour
{
    [Header("Moon Dragging Settings")]
    public Transform earthTransform;
    public float orbitRadius = 2.0f;
    public LunarPhaseQuestionManager questionManager;
    
    [Header("XR Interaction Settings")]
    public bool enablePinchToGrab = true;
    public bool constrainToOrbit = true;
    public float dragSensitivity = 1.0f;
    
    private LunarPhase currentPhase;
    private bool isDragging = false;
    private XRGrabInteractable grabInteractable;
    private Vector3 lastDragPosition;
    private float currentAngle = 0f;

    void Awake()
    {
        // Get or add the XR Grab Interactable component
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable == null)
        {
            grabInteractable = gameObject.AddComponent<XRGrabInteractable>();
            Debug.Log("[MoonDragger] Added XRGrabInteractable component");
        }
        
        // Set up grab interactable for pinch support
        grabInteractable.selectMode = InteractableSelectMode.Multiple;
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
        grabInteractable.selectEntered.AddListener(OnDragStart);
        grabInteractable.selectExited.AddListener(OnDragEnd);
        
        // Configure for pinch interaction
        if (enablePinchToGrab)
        {
            grabInteractable.interactionLayers = 1; // Default interaction layer
            grabInteractable.throwOnDetach = false;
            grabInteractable.attachTransform = transform;
        }
        
        Debug.Log("[MoonDragger] Moon dragger initialized with XR interaction support");
    }

    public void SetTargetPhase(LunarPhase phase)
    {
        currentPhase = phase;
        Debug.Log($"[MoonDragger] Set target phase: {phase.phaseType} (correct angle: {phase.correctAngle}°)");
        
        // Optionally, move moon to a starting position
        if (earthTransform != null)
        {
            // Start at a random angle
            float startAngle = Random.Range(0f, 360f);
            Vector3 startPos = earthTransform.position + 
                new Vector3(Mathf.Cos(startAngle * Mathf.Deg2Rad), 0, Mathf.Sin(startAngle * Mathf.Deg2Rad)) * orbitRadius;
            transform.position = startPos;
            currentAngle = startAngle;
            
            // Update the angle display immediately
            if (questionManager != null)
                questionManager.UpdateUserAngle(currentAngle);
        }
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        isDragging = true;
        lastDragPosition = args.interactorObject.transform.position;
        Debug.Log("[MoonDragger] Moon grabbed - dragging started");
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isDragging = false;
        Debug.Log("[MoonDragger] Moon released - dragging ended");
        
        // Calculate final angle
        CalculateAndUpdateAngle();
    }
    
    void OnDragStart(SelectEnterEventArgs args)
    {
        // Additional drag start logic if needed
    }
    
    void OnDragEnd(SelectExitEventArgs args)
    {
        // Additional drag end logic if needed
    }

    void Update()
    {
        if (isDragging)
        {
            // Get the current drag position from the interactor
            var interactors = grabInteractable.interactorsSelecting;
            if (interactors.Count > 0)
            {
                Vector3 currentDragPosition = interactors[0].transform.position;
                Vector3 dragDelta = currentDragPosition - lastDragPosition;
                
                // Calculate the angle change based on drag direction
                if (earthTransform != null)
                {
                    Vector3 moonToEarth = (earthTransform.position - transform.position).normalized;
                    Vector3 dragDirection = dragDelta.normalized;
                    
                    // Calculate angle change in the orbital plane
                    float angleChange = Vector3.Dot(dragDirection, Vector3.Cross(moonToEarth, Vector3.up)) * dragSensitivity;
                    
                    // Update current angle
                    currentAngle += angleChange * 10f; // Scale factor for sensitivity
                    currentAngle = (currentAngle + 360f) % 360f;
                    
                    // Constrain to orbit if enabled
                    if (constrainToOrbit)
                    {
                        Vector3 newPosition = earthTransform.position + 
                            new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad), 0, Mathf.Sin(currentAngle * Mathf.Deg2Rad)) * orbitRadius;
                        transform.position = newPosition;
                    }
                    
                    // Update angle display continuously
                    if (questionManager != null)
                        questionManager.UpdateUserAngle(currentAngle);
                }
                
                lastDragPosition = currentDragPosition;
            }
        }
        
        // Always calculate and display current angle, even when not dragging
        if (!isDragging && earthTransform != null)
        {
            CalculateAndUpdateAngle();
        }
    }
    
    private void CalculateAndUpdateAngle()
    {
        if (earthTransform == null) return;
        
        Vector3 moonDir = (transform.position - earthTransform.position).normalized;
        currentAngle = Mathf.Atan2(moonDir.z, moonDir.x) * Mathf.Rad2Deg;
        currentAngle = (currentAngle + 360f) % 360f;
        
        // Update the angle display
        if (questionManager != null)
            questionManager.UpdateUserAngle(currentAngle);
    }
    
    // Public method to get current angle
    public float GetCurrentAngle()
    {
        return currentAngle;
    }
    
    // Public method to get angular error from target
    public float GetAngularError()
    {
        if (currentPhase == null) return 0f;
        return Mathf.DeltaAngle(currentAngle, currentPhase.correctAngle);
    }
} 