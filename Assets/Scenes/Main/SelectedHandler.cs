using UnityEngine;
using Tobii.Gaming;

[RequireComponent(typeof(GazeAware))]
public class SelectedHandler : MonoBehaviour
{
    public Global.GameObjectPattern representPatternSet;

    private GazeAware _gazeAwareComponent;

    void Start()
    {
        // put the z position to fit with head cursor
        Camera cam = Camera.main;
        var currentPosition = cam.WorldToScreenPoint(transform.position);
        var newPosition = new Vector3(currentPosition.x, currentPosition.y,cam.nearClipPlane);
        transform.position = cam.ScreenToWorldPoint(newPosition);

        _gazeAwareComponent = GetComponent<GazeAware>();
    }

    void Update()
    {
        if (_gazeAwareComponent.HasGazeFocus)
        {
            switch (Global.currentLevel)
            {
                case TrialLevel.Hard:
                    hardEyeRegister();
                    break;
                case TrialLevel.Easy:
                    easyEyeRegister();
                    break;
                case TrialLevel.Familization:
                    familizationEyeRegister();
                    break;
            }
        }
        else
        {
            switch (Global.currentLevel)
            {
                case TrialLevel.Hard:
                    hardEyeDeRegister();
                    break;
                case TrialLevel.Easy:
                    easyEyeDeRegister();
                    break;
                case TrialLevel.Familization:
                    familizationEyeDeRegister();
                    break;
                default:
                    return;
            }

            // observing handle in HeadEye case
            if (Global.currentState == TrialState.HeadEye)
            {
                HeadHandler trackerInstance = GameObject
                    .Find("headCursor")
                    .GetComponent<HeadHandler>();

                if (trackerInstance.isObserving)
                {
                    trackerInstance.isObserving = false;
                }
                GameObject
                    .Find("headCursor")
                    .GetComponent<HeadHandler>()
                    .isObserving = false;

                GameObject
                    .Find("headCursor")
                    .GetComponent<HeadHandler>()
                    .stateSequence
                    .Clear();
            }
        }
    }

    // ----------------------------- EYE REGISTER--------------------------------
    private void easyEyeRegister()
    {
        EasyRunner runnerEasyInstance = GameObject
            .Find("GameRunner").
            GetComponent<EasyRunner>();

        if (runnerEasyInstance != null &&
            !runnerEasyInstance.trialDone &&
            Global.currentState != TrialState.Head)
        {
            runnerEasyInstance.selectedPatternSet = representPatternSet;
        }
    }

    private void hardEyeRegister()
    {
        HardRunner runnerInstance = GameObject
            .Find("GameRunner")
            .GetComponent<HardRunner>();

        if (runnerInstance != null &&
            !runnerInstance.trialDone &&
            Global.currentState != TrialState.Head)
        {
            runnerInstance.selectedPatternSet = representPatternSet;
        }
    }

    private void familizationEyeRegister()
    {
        Familiarization runnerTrialInstance = GameObject
            .Find("GameRunner").
            GetComponent<Familiarization>();

        if (runnerTrialInstance != null)
        {
            runnerTrialInstance.didEyeSelect = true;
        }
    }

    // ----------------------------- EYE DE-REGISTER--------------------------------
    private void hardEyeDeRegister()
    {
        HardRunner runnerInstance = GameObject
            .Find("GameRunner")
            .GetComponent<HardRunner>();

        if (runnerInstance != null &&
            !runnerInstance.trialDone &&
            runnerInstance.selectedPatternSet == representPatternSet &&
            Global.currentState != TrialState.Head)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite
                = runnerInstance.white;

            runnerInstance.selectedPatternSet = null;
        }
    }

    private void easyEyeDeRegister()
    {
        EasyRunner runnerEasyInstance = GameObject
            .Find("GameRunner").
            GetComponent<EasyRunner>();

        if (runnerEasyInstance != null &&
            !runnerEasyInstance.trialDone &&
            runnerEasyInstance.selectedPatternSet == representPatternSet &&
            Global.currentState != TrialState.Head)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite
                = runnerEasyInstance.white;

            runnerEasyInstance.selectedPatternSet = null;
        }
    }

    private void familizationEyeDeRegister()
    {
        Familiarization runnerTrialInstance = GameObject
            .Find("GameRunner").
            GetComponent<Familiarization>();

        if (runnerTrialInstance != null)
        {
            runnerTrialInstance.didEyeSelect = false;
            if (Global.currentState != TrialState.Head)
            {
                this.gameObject.GetComponent<SpriteRenderer>().sprite
                    = runnerTrialInstance.white;
            }
        }
    }
}
