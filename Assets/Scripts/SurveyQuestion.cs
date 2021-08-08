using UnityEngine;

[CreateAssetMenu(fileName = "Q_", menuName = "Survey/Survey Question", order = 1)]
public class SurveyQuestion : ScriptableObject
{
    [TextArea] public string question;
    public string[] answers;
}