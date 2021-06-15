using Tobii.Gaming;
using UnityEngine;
using System.Collections.Generic;

public enum HeadState
{
    Up, Stable, Down
}

public class HeadPlloter : MonoBehaviour
{
    public Queue<HeadState> stateSequence = new Queue<HeadState>();

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
            // get yaw and pitch
            var pitch = headPose.Rotation.eulerAngles.x;
            if (pitch > 180) pitch -= 360;
            var yaw = headPose.Rotation.eulerAngles.y;
            if (yaw > 180) yaw -= 360;

            // get screen resolution
            var resolution = Screen.currentResolution;
            var width = resolution.width;
            var height = resolution.height;

            /*  tobii 5 spec: 
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
        }
    }
}
