using Tobii.Gaming;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
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

    private HeadStateChange[] headChanged = new HeadStateChange[] {};

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

        var currentSequence = Array.ConvertAll(stateSequence.ToArray(), item => (HeadState)item);
        // we need to reverse ince the queue will be from right to left, not left to right
        Array.Reverse(currentSequence);

        foreach (HeadState[] pattern in templateSequences)
        {
            if (checkContained(currentSequence, pattern))
            {
                didNod = true;
                isObserving = false;
                break;
            }
        }
    }

    private bool checkContained(HeadState[] lhs, HeadState[] rhs)
    {
        if (lhs.Length < rhs.Length)
        {
            return false;
        }

        // get the diference in lenght to get max tries based on the indexes
        int differenceLength = lhs.Length - rhs.Length;

        /*
        the maximum tries is  difirence length + 1.
        For example lhs = rhs (diffirence length = 0) which meean we jsut need to compare 1 time from index 0 of lhs
        If lhs has 1 element longer than rhs, then will be 2 compare(at index 0 and 1)
        */
        for (int index = 0; index <= differenceLength; index++)
        {
            // cut the lhs to size of rhs from index
            var segment = new ArraySegment<HeadState>(lhs, index, rhs.Length).ToArray<HeadState>();

            // check if this cut is equal with rhs, stop the loop and return true
            // otherwise continue the loop
            if (segment.SequenceEqual(rhs))
            {
                return true;
            }
        }

        // return false if there is no equal is found
        return false;
    }

    // *** these patterns's elements will be reversed at scene awake to use with the Queue.
    public HeadState[][] templateSequences = new HeadState[][]
    {
        new HeadState[]
        {
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable
        },
        new HeadState[] {
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Stable, HeadState.Stable
        },
        new HeadState[] {
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down
        },
        new HeadState[] {
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Up, HeadState.Up, HeadState.Up, 
            HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, 
            HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, 
            HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down
        },
        new HeadState[] {
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Up, HeadState.Up, HeadState.Up, 
            HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, 
            HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, 
            HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, 
            HeadState.Stable, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, 
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Stable, HeadState.Stable, HeadState.Stable, 
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable
        }
    };
}
