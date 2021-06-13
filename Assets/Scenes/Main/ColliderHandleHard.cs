using UnityEngine;

public class ColliderHandleHard : MonoBehaviour
{
    // the pattern that this object represent
    public Global.GameObjectPattern representPatternSet;

    private void registerSelectedObject() {
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
            default:
                return;
        }
    }

    private void easyEyeRegister()
    {
        EyeOnlyEasyRunner runnerEasyInstance = GameObject
            .Find("GameRunner").
            GetComponent<EyeOnlyEasyRunner>();
        
        if (runnerEasyInstance != null && 
            !runnerEasyInstance.trialDone && 
            Global.currentState != TrialState.Head)
        {
            runnerEasyInstance.selectedPatternSet = representPatternSet;
        }
    }

    private void hardEyeRegister()
    {
        EyeOnlyHardRunner runnerInstance = GameObject
            .Find("GameRunner")
            .GetComponent<EyeOnlyHardRunner>();
        
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

    private void deRegisterSelectedObject() {

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

        HeadHandler trackerInstance = GameObject
            .Find("headCursor")
            .GetComponent<HeadHandler>();

        if (Global.currentState == TrialState.HeadEye && 
            trackerInstance.isObserving)
        {
            trackerInstance.isObserving = false;
        }
        
        // observing handle in HeadEye case
        if (Global.currentState == TrialState.HeadEye) 
        { 
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

    private void hardEyeDeRegister()
    {
        EyeOnlyHardRunner runnerInstance = GameObject
            .Find("GameRunner")
            .GetComponent<EyeOnlyHardRunner>();
        
        if (runnerInstance != null && 
            !runnerInstance.trialDone && 
            Global.currentState != TrialState.Head)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite 
                =  runnerInstance.white;
            
            runnerInstance.selectedPatternSet = null;
        }
    }

    private void easyEyeDeRegister()
    {
        EyeOnlyEasyRunner runnerEasyInstance = GameObject
            .Find("GameRunner").
            GetComponent<EyeOnlyEasyRunner>();
        
        if (runnerEasyInstance != null && 
            !runnerEasyInstance.trialDone && 
            Global.currentState != TrialState.Head)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite 
                =  runnerEasyInstance.white;
            
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
        EyeOnlyEasyRunner runnerEasyInstance = GameObject
            .Find("GameRunner").
            GetComponent<EyeOnlyEasyRunner>();

        if (runnerEasyInstance != null)
        {
            switch (Global.currentState)
            {
                case TrialState.Head:
                    runnerEasyInstance.headSelectedPatternSet = representPatternSet;
                    break;
                case TrialState.Order:
                    if (runnerEasyInstance.selectedPatternSet == representPatternSet)
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
        EyeOnlyHardRunner runnerInstance = GameObject
            .Find("GameRunner")
            .GetComponent<EyeOnlyHardRunner>();

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
        EyeOnlyEasyRunner runnerEasyInstance = GameObject
            .Find("GameRunner").
            GetComponent<EyeOnlyEasyRunner>();
        
        if (runnerEasyInstance != null)
        {
            switch(Global.currentState)
            {
                case TrialState.Head:
                    if (!runnerEasyInstance.trialDone)
                    {
                        this
                            .gameObject
                            .GetComponent<SpriteRenderer>()
                            .sprite = runnerEasyInstance.white;
                        runnerEasyInstance.headSelectedPatternSet = null;
                    }
                    break;
                case TrialState.Order:
                    runnerEasyInstance.headSelectedPatternSet = null;
                    break;
                default:
                    break;
            }
        }
    }

    private void hardHeadDeRegister()
    {
        EyeOnlyHardRunner runnerInstance = GameObject
            .Find("GameRunner")
            .GetComponent<EyeOnlyHardRunner>();

        if (runnerInstance != null)
        {
            switch(Global.currentState)
            {
                case TrialState.Head:
                    if (!runnerInstance.trialDone)
                    {
                        this
                            .gameObject
                            .GetComponent<SpriteRenderer>()
                            .sprite = runnerInstance.white;
                        runnerInstance.headSelectedPatternSet = null;
                    }
                    break;
                case TrialState.Order:
                    runnerInstance.headSelectedPatternSet = null;
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
                    runnerTrialInstance.didHeadSelect = false;
                    break;
                case TrialState.Order:
                    runnerTrialInstance.didHeadSelect = false;
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Sent each frame where another object is within a trigger collider
    /// attached to this object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.name.Equals("eyeCursor")) 
        {
            registerSelectedObject();
        }
        if (other.gameObject.name.Equals("headCursor")) 
        {
            registerHeadSelectedObject();
        }
    }

    /// <summary>
    /// Sent when another object leaves a trigger collider attached to
    /// this object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name.Equals("eyeCursor")) 
        {
            deRegisterSelectedObject();
        }
        if (other.gameObject.name.Equals("headCursor")) 
        {
            deRegisterHeadSelectedObject();
        }
    }
}
