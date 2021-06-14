using UnityEngine;

public class ColliderHandle : MonoBehaviour
{
    // the pattern that this object represent
    public Global.GameObjectPattern representPatternSet;

    private void registerHeadSelectedObject() {
        // with condition 3, the collider of Head system is not required 
        // but still need a Head tracker object
        switch (Global.currentState)
        {
            case TrialState.Eye:
                return;
            case TrialState.Head:
                break;
            case TrialState.HeadEye:
                return;
            case TrialState.Order:
                break;
        }

        switch (Global.currentLevel)
        {
            case TrialLevel.Hard:
                hardHeadRegister();
                break;
            case TrialLevel.Easy:
                easyHeadRegister();
                break;
            case TrialLevel.Familization:
                familizationHeadRegister();
                break;
            default:
                return;
        }
    }

    private void easyHeadRegister()
    {
        EasyRunner runnerEasyInstance = GameObject
            .Find("GameRunner").
            GetComponent<EasyRunner>();

        if (runnerEasyInstance != null)
        {
            switch (Global.currentState)
            {
                case TrialState.Head:
                    if (!runnerEasyInstance.trialDone)
                    {
                        runnerEasyInstance.headSelectedPatternSet = representPatternSet;
                    }
                    break;
                case TrialState.Order:
                    if (runnerEasyInstance.selectedPatternSet == representPatternSet 
                    && !runnerEasyInstance.trialDone)
                    {
                        runnerEasyInstance.headSelectedPatternSet = representPatternSet;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private void hardHeadRegister()
    {
        HardRunner runnerInstance = GameObject
            .Find("GameRunner")
            .GetComponent<HardRunner>();

        if (runnerInstance != null)
        {
            switch (Global.currentState)
            {
                case TrialState.Head:
                    runnerInstance.headSelectedPatternSet = representPatternSet;
                    break;
                case TrialState.Order:
                    if (runnerInstance.selectedPatternSet == representPatternSet)
                    {
                        runnerInstance.headSelectedPatternSet = representPatternSet;
                    }
                    break;
                default:
                    break;
            }
        };
    }

    private void familizationHeadRegister()
    {
        Familiarization runnerTrialInstance = GameObject
            .Find("GameRunner").
            GetComponent<Familiarization>();
        

        if (runnerTrialInstance != null)
        {
            runnerTrialInstance.didHeadSelect = true;
        }
    }

    private void deRegisterHeadSelectedObject() {
        // with condition 3, the collider of Head system is not required 
        // but still need a Head tracker object
        switch (Global.currentState)
        {
            case TrialState.Eye:
                return;
            case TrialState.Head:
                break;
            case TrialState.HeadEye:
                return;
            case TrialState.Order:
                break;
        }

        switch (Global.currentLevel)
        {
            case TrialLevel.Hard:
                hardHeadDeRegister();
                break;
            case TrialLevel.Easy:
                easyHeadDeRegister();
                break;
            case TrialLevel.Familization:
                familizationHeadDeRegister();
                break;
            default:
                return;
        }
    }

    private void easyHeadDeRegister()
    {
        EasyRunner runnerEasyInstance = GameObject
            .Find("GameRunner").
            GetComponent<EasyRunner>();
        
        if (runnerEasyInstance != null)
        {
            switch(Global.currentState)
            {
                case TrialState.Head:
                    if (!runnerEasyInstance.trialDone && 
                    runnerEasyInstance.headSelectedPatternSet == representPatternSet)
                    {
                        this
                            .gameObject
                            .GetComponent<SpriteRenderer>()
                            .sprite = runnerEasyInstance.white;
                        
                        runnerEasyInstance.headSelectedPatternSet = null;
                    }
                    break;
                case TrialState.Order:
                    if (runnerEasyInstance.headSelectedPatternSet == representPatternSet)
                    {
                        runnerEasyInstance.headSelectedPatternSet = null;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private void hardHeadDeRegister()
    {
        HardRunner runnerInstance = GameObject
            .Find("GameRunner")
            .GetComponent<HardRunner>();

        if (runnerInstance != null)
        {
            switch(Global.currentState)
            {
                case TrialState.Head:
                    if (!runnerInstance.trialDone && 
                    runnerInstance.headSelectedPatternSet == representPatternSet)
                    {
                        this
                            .gameObject
                            .GetComponent<SpriteRenderer>()
                            .sprite = runnerInstance.white;
                        runnerInstance.headSelectedPatternSet = null;
                    }
                    break;
                case TrialState.Order:
                    if (runnerInstance.headSelectedPatternSet == representPatternSet)
                    {
                        runnerInstance.headSelectedPatternSet = null;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private void familizationHeadDeRegister()
    {
        Familiarization runnerTrialInstance = GameObject
            .Find("GameRunner").
            GetComponent<Familiarization>();

        if (runnerTrialInstance != null)
        {
            runnerTrialInstance.didHeadSelect = false;
            switch(Global.currentState)
            {
                case TrialState.Head:
                    this
                        .gameObject
                        .GetComponent<SpriteRenderer>()
                        .sprite = runnerTrialInstance.white;
                    break;
                default:
                    break;
            }
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.name.Equals("headCursor")) 
        {
            registerHeadSelectedObject();
        }
    }

    private void OnCollisionExit(Collision other) {
        if (other.gameObject.name.Equals("headCursor")) 
        {
            deRegisterHeadSelectedObject();
        }
    }
}
