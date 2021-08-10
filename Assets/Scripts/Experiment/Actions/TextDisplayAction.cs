using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextDisplayAction : ExperimentAction
{
    [SerializeField] private Text uiText;
    [SerializeField, TextArea] private string[] texts;

    private bool waitingForInput = false;

    public override IEnumerator Run()
    {
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i] == null) continue;
            
            uiText.text = texts[i];
            yield return StartCoroutine(WaitForInput());
        }

        uiText.text = "";
    }

    private IEnumerator WaitForInput()
    {
        waitingForInput = true;
        while (waitingForInput)
        {
            yield return new WaitForFixedUpdate();
        }
    }

    public override void SubmitInput(int input)
    {
        if (input == 100)
            waitingForInput = false;
    }
}