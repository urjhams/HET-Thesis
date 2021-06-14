using Tobii.Gaming;
using UnityEngine;

public class HeadPlloter : MonoBehaviour
{
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
            var position = new Vector3(expectedX, expectedY, cam.nearClipPlane);
            var screenPosition = cam.ScreenToWorldPoint(position);

            // move the cursor
            transform.position = screenPosition;
        }
    }
}
