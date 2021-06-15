using Tobii.Gaming;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public enum HeadState
{
    Up, Stable, Down
}

public class HeadPlloter : MonoBehaviour
{
    public Queue<HeadState> stateSequence = new Queue<HeadState>();

    // currently we lock at 60 fps, the logest pattern array is 41 elements
    // which we actually should just use 41 but for sure we use the limit of
    // 1 seconds, which mean 60 frames in total.
    private int stateSequenceLimit = 60;

    [HideInInspector]
    public bool didNod = false;

    [HideInInspector]
    public bool isObserving = false;

    private float currentStablePitch = 0F;

    private float estimatePitchDifference = 2.5f;

    private Camera cam;
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
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

            float pitchAnglePixel = height / (45 + 1 + 45);   // include 0 degree
            float yawAnglePixel = width / (65 + 1 + 65);      // include 0 degree

            var expectedX = (yaw + 65) * yawAnglePixel;
            if (expectedX < 0) expectedX = 0F;
            if (expectedX > width) expectedX = width;

            var expectedY = (pitch + 45) * pitchAnglePixel;
            // need to get the oposite y cuz pitch > 0 mean look down
            expectedY = height - expectedY;
            if (expectedY < 0) expectedY = 0F;
            if (expectedY > height) expectedY = height;

            // concert to world space position from screen coordinate
            // current pattern objects's z is 10,
            // so put this z to 10 to make sure they gonna collise
            var position = new Vector3(expectedX, expectedY, 10);
            var screenPosition = cam.ScreenToWorldPoint(position);
            //Debug.Log(screenPosition.z);
            // move the cursor
            transform.position = screenPosition;

            // -------------------------- nod observe
            RecordRunner runnerTrialInstance = GameObject
                .Find("GameRunner").
                GetComponent<RecordRunner>();

            if (runnerTrialInstance != null)
            {
                runnerTrialInstance.currentPitchValue = pitch;
            }

            if (Global.currentState == TrialState.HeadEye && !didNod)
            {
                if (!isObserving)
                {
                    // start observing
                    isObserving = true;
                    currentStablePitch = pitch * 100;
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
        switch (pitch * 100)
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
            HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Stable,
            HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up, HeadState.Up,
            HeadState.Up, HeadState.Up, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down,
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down
        },
        new HeadState[] {
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable,
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down,
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down,
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down,
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down
        },
        new HeadState[] {
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable,
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Down, HeadState.Down,
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down,
            HeadState.Down, HeadState.Down,  HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down,
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down,
            HeadState.Down, HeadState.Down
        },
        new HeadState[] {
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Down,
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down,
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down,
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down
        },
        new HeadState[] {
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable,HeadState.Down, HeadState.Down,
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down
        },
        new HeadState[] {
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Stable,
            HeadState.Stable, HeadState.Stable, HeadState.Stable, HeadState.Down, HeadState.Down, HeadState.Down,
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down,
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down,
            HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down, HeadState.Down
        }
    };
}
