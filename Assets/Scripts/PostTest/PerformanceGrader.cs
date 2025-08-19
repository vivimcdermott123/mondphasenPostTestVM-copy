using UnityEngine;

public class PerformanceGrader : MonoBehaviour
{
    public string GetGrade(float totalError, int numQuestions)
    {
        float avgError = totalError / numQuestions;
        if (avgError < 10f) return "A";
        if (avgError < 20f) return "B";
        if (avgError < 30f) return "C";
        if (avgError < 45f) return "D";
        return "F";
    }
} 
//accuracy avg (%)