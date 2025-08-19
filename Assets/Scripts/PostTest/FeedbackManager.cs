using UnityEngine;
using TMPro;

public class FeedbackManager : MonoBehaviour
{
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI finalGradeText;

    private string[] postTestLines;
    public void SetPostTestLines(string[] lines) { postTestLines = lines; }
    public void ShowFeedbackLine(int lineIndex) {
        if (feedbackText != null && postTestLines != null && lineIndex < postTestLines.Length)
            feedbackText.text = postTestLines[lineIndex];
        else if (feedbackText != null)
            feedbackText.text = "Feedback text index out of range!";
    }
    public void ShowFinalGradeLine(int lineIndex) {
        if (finalGradeText != null && postTestLines != null && lineIndex < postTestLines.Length)
            finalGradeText.text = postTestLines[lineIndex];
        else if (finalGradeText != null)
            finalGradeText.text = "Final grade text index out of range!";
    }
} 