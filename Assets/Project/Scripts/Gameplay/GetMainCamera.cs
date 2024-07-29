using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetMainCamera : MonoBehaviour
{
    public Canvas canvas;
    public enum TestState { Equipped, Throwing, Attatched, Recalling}
    public TestState testStateToggle;

    public void Update()
    {
        //Personally for some state machines I like two switches, one for Update() and one that we only run when we need to
        switch (testStateToggle)
        {
            case TestState.Equipped:

                break;
            case TestState.Throwing:

                break;
            case TestState.Attatched:

                break;
            case TestState.Recalling:
                //The Movement Lerp
                //If Our Distance Is Good
                RefreshTestStates(TestState.Equipped);

                //We pass in a TestState into RefreshTestStates literally just so these two lines can be a single one like above
                testStateToggle = TestState.Equipped;
                //RefreshTestStates();
                break;
        }
    }

    //This one only runs when we tell it to, which is mostly for stuff we only need to set once per state change (the knife setting, UI settings, animation settings etc.)
    public void RefreshTestStates(TestState testState)
    {
        testStateToggle = testState;

        switch (testStateToggle)
        {
            case TestState.Equipped:
                
                break;
            case TestState.Throwing:
                break;
            case TestState.Attatched:
                //SetKnifeParent();
                break;
            case TestState.Recalling:
                //RemoveKnifeParent();
                break;
        }
    }

    public void Start()
    {
        canvas.worldCamera = GameManager.Instance.mainCamera;
    }
}
