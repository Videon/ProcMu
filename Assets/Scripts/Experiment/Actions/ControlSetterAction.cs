using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlSetterAction : ExperimentAction
{
    [SerializeField] private PlayerInput characterControls;
    [SerializeField] private PlayerInput uiControls;

    [SerializeField] private bool characterControlsActive;
    [SerializeField] private bool uiControlsActive;

    public override IEnumerator Run()
    {
        characterControls.enabled = false;
        uiControls.enabled = false;

        characterControls.enabled = characterControlsActive;
        uiControls.enabled = uiControlsActive;
        yield break;
    }
}