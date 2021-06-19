using Tobii.Gaming;
using UnityEngine;
using System.Collections.Generic;
using System;

public enum HeadState
{
    Up, Stable, Down
}

public enum HeadStateChange
{
    StableToDown, StableToUp, DownBackStable, UpBackStable
}

public class HeadPlloter : MonoBehaviour
{
    public Queue<HeadState> stateSequence = new Queue<HeadState>();

    // the highest frames in the pattern is 119, 
    // so we set the boundery of observation to 120 frames.
    private int stateSequenceLimit = 120;

    [HideInInspector]
    public bool didNod = false;

    [HideInInspector]
    public bool isObserving = false;

    private float currentStablePitch = 0F;

    private float estimatePitchDifference = 3;

    private Camera cam;
    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        var headPose = TobiiAPI.GetHeadPose();
        if (headPose.IsRecent())
        {
            // -------------------------- get yaw, pitch
            // get yaw and pitch
            var pitch = headPose.Rotation.eulerAngles.x;
            if (pitch > 180) pitch -= 360;
            var yaw = headPose.Rotation.eulerAngles.y;
            if (yaw > 180) yaw -= 360;
            
            // --------------------------- adjust head plotter
            // get screen resolution
            var resolution = Screen.currentResolution;
            var width = resolution.width;
            var height = resolution.height;

            /*  tobii 5 spec: 
                https://help.tobii.com/hc/en-us/articles/213473765-Head-tracking-specifications
                Pitch:  +45 -45
                Yaw:    +65 -65
                Roll:   +25 -25
            */
            var screenPosition = cam.ScreenToWorldPoint( new Vector3(0, 0, 10));
            double pixelInMm = 0.2645833333;
            var sensitiveFactor = 1.5;
            var newX = yaw * pixelInMm * sensitiveFactor;
            var newY = pitch * pixelInMm * sensitiveFactor;
            newY = -newY;
            var screenPosition2 = new Vector3((float)newX, (float)newY, screenPosition.z);
            transform.position = screenPosition2;

            // -------------------------- nod observe
            RecordRunner recordInstance = GameObject
                .Find("GameRunner").
                GetComponent<RecordRunner>();

            if (recordInstance != null)
            {
                recordInstance.currentPitchValue = pitch;
            }

            if (Global.currentState == TrialState.HeadEye && !didNod)
            {
                if (!isObserving)
                {
                    // start observing
                    isObserving = true;
                    currentStablePitch = pitch;
                }
                else
                {
                    stateSequenceObseve(pitch);
                }
            }
        }
    }

    private void stateSequenceObseve(float pitch)
    {
        if (stateSequence.Count == stateSequenceLimit)
        {
            stateSequence.Dequeue();    // remove an element in rear side
        }

        switch (pitch)
        {
            case float value when (value <= currentStablePitch - estimatePitchDifference):
                stateSequence.Enqueue(HeadState.Up);
                break;
            case float value when (value >= currentStablePitch + estimatePitchDifference):
                stateSequence.Enqueue(HeadState.Down);
                break;
            default:
                stateSequence.Enqueue(HeadState.Stable);
                break;
        }
        
        var currentSequence = Array
            .ConvertAll(stateSequence.ToArray(), item => (HeadState)item);
        
        if (foundNodIn(currentSequence))
        {
            didNod = true;
            isObserving = false;
        }
    }

    private bool foundNodIn(HeadState[] states)
    {
        var changeStates = getChangeStates(states);
        foreach (HeadStateChange[] pattern in nodPatterns)
        {
            if (Helper.checkContained(changeStates, pattern))
            {
                return true;
            }
        }
        return false;
    }

    private HeadStateChange[] getChangeStates(HeadState[] states)
    {
        var headChanged = new List<HeadStateChange>();

        for (int index = 1; index < states.Length; index++)
        {
            var previous = states[index - 1];

            switch(states[index])
            {
                case HeadState.Up:
                    if (previous == HeadState.Stable)
                    {
                        headChanged.Add(HeadStateChange.StableToUp);
                    }
                    break;
                case HeadState.Down:
                    if (previous == HeadState.Stable)
                    {
                        headChanged.Add(HeadStateChange.StableToDown);
                    }
                    break;
                case HeadState.Stable:
                    switch (previous)
                    {
                        case HeadState.Up:
                            headChanged.Add(HeadStateChange.UpBackStable);
                            break;
                        case HeadState.Down:
                            headChanged.Add(HeadStateChange.DownBackStable);
                            break;
                        default:
                            break;
                    }
                    break;
            }
        }
        return Array.ConvertAll(headChanged.ToArray(), item => (HeadStateChange)item);
    }

    private HeadStateChange[][] nodPatterns = new HeadStateChange[][]
    {
        // stable -> down -> stable -> down
        new HeadStateChange[]
        {
            HeadStateChange.StableToDown, HeadStateChange.DownBackStable,
            HeadStateChange.StableToDown, HeadStateChange.DownBackStable
        },
        // stable -> down -> stable -> up -> stable -> down -> stable
        new HeadStateChange[]
        {
            HeadStateChange.StableToDown, HeadStateChange.DownBackStable,
            HeadStateChange.StableToUp, HeadStateChange.UpBackStable, 
            HeadStateChange.StableToDown, HeadStateChange.DownBackStable
        },
        // stable -> up -> stable -> down -> stable
        new HeadStateChange[]
        {
            HeadStateChange.StableToUp, HeadStateChange.UpBackStable, 
            HeadStateChange.StableToDown, HeadStateChange.DownBackStable
        },
        // stable -> down -> stable
        new HeadStateChange[]
        {
            HeadStateChange.StableToDown, HeadStateChange.DownBackStable
        }
    };
}
