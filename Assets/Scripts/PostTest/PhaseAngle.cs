using UnityEngine;

public class PhaseAngleExtractor : MonoBehaviour
{
    public Transform center; // Assign your Earth or Sun transform here

    [Header("Phase Anchors")]
    public Transform NewMoon;
    public Transform WaxingCrescent;
    public Transform FirstQuarter;
    public Transform WaxingGibbous;
    public Transform FullMoon;
    public Transform WaningGibbous;
    public Transform LastQuarter;
    public Transform WaningCrescent;

    [ContextMenu("Print Phase Angles")]
    public void PrintPhaseAngles()
    {
        PrintAngle("New Moon", NewMoon);
        PrintAngle("Waxing Crescent", WaxingCrescent);
        PrintAngle("First Quarter", FirstQuarter);
        PrintAngle("Waxing Gibbous", WaxingGibbous);
        PrintAngle("Full Moon", FullMoon);
        PrintAngle("Waning Gibbous", WaningGibbous);
        PrintAngle("Last Quarter", LastQuarter);
        PrintAngle("Waning Crescent", WaningCrescent);
    }

    private void PrintAngle(string name, Transform phase)
    {
        if (center == null || phase == null)
        {
            Debug.LogWarning($"Missing reference for {name}");
            return;
        }
        Vector3 dir = phase.position - center.position;
        float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        angle = (angle + 360f) % 360f;
        Debug.Log($"{name}: {angle:F1}°");
    }
}