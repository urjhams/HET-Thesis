using System.Collections;
using Tobii.Research;
using Tobii.Research.Unity;
using UnityEngine;
using UnityEngine.UI;

public class CalibrationRunner : MonoBehaviour
{
    private IEyeTracker eyeTracker;

    [SerializeField]
    private Image calibrationRepresentPoint;

    [SerializeField]
    private Canvas calibrationCanvas;

    [SerializeField]
    private Image panel;

    private CalibrationThread calibrationThread;

    private Tobii.Research.Unity.CalibrationPoint pointScript;

    private bool isCalibrating
    {
        set
        {
            calibrationCanvas.gameObject.SetActive(value);
            pointScript.gameObject.SetActive(value);
            panel.color = value ? Color.black : new Color(0, 0, 0, 0);
        }
    }

    void Awake()
    {
        Utility.getFirstEyeTracker(
            handle: (eyetracker) => this.eyeTracker = eyetracker
        );
    }

    void Start()
    {
        pointScript = calibrationRepresentPoint
            .GetComponent<Tobii.Research.Unity.CalibrationPoint>();
        isCalibrating = false;
    }

    void Update()
    {

    }

    public void startCalibration()
    {
        if (eyeTracker != null)
        {
            StartCoroutine(Execute(eyeTracker));
        }
    }

    void OnDisable()
    {
        if (calibrationThread != null)
        {
            calibrationThread.StopThread();
            calibrationThread = null;
        }
    }

    // ------------------------------- calibration
    IEnumerator Execute(IEyeTracker eyeTracker)
    {
        if (eyeTracker != null)
        {
            yield return Calibrate(eyeTracker);
        }
        yield break;
    }

    private IEnumerator Calibrate(IEyeTracker eyeTracker)
    {
        isCalibrating = true;

        // check if there is a thread current running
        if (calibrationThread != null)
        {
            calibrationThread.StopThread();
            calibrationThread = null;
        }

        calibrationThread = new CalibrationThread(eyeTracker, true);

        // Only continue if the calibration thread is running.
        for (int i = 0; i < 10; i++)
        {
            if (calibrationThread.Running)
            {
                break;
            }

            yield return new WaitForSeconds(0.1f);
        }

        if (!calibrationThread.Running)
        {
            Debug.LogError("Failed to start calibration thread");
            calibrationThread.StopThread();
            calibrationThread = null;
            isCalibrating = false;
            yield break;
        }

        var result = calibrationThread.EnterCalibrationMode();

        yield return StartCoroutine(waitForResult(result));

        // Define the points on screen we should calibrate at.
        // The coordinates are normalized, i.e. (0.0f, 0.0f) 
        // is the upper left corner and (1.0f, 1.0f) is the lower right corner.
        var pointsToCalibrate = new NormalizedPoint2D[] {
                new NormalizedPoint2D(0.5f, 0.5f),
                new NormalizedPoint2D(0.1f, 0.1f),
                new NormalizedPoint2D(0.1f, 0.9f),
                new NormalizedPoint2D(0.9f, 0.1f),
                new NormalizedPoint2D(0.9f, 0.9f),
            };
        // Collect data.
        foreach (var point in pointsToCalibrate)
        {
            // Show an image on screen where you want to calibrate.
            var vector = Utility.ToVector2(point);
            calibrationRepresentPoint.rectTransform.anchoredPosition = 
                new Vector2(Screen.width * vector.x, Screen.height * (1 - vector.y));
            
            // start animation
            pointScript.StartAnim();

            // Wait for animation.
            yield return new WaitForSeconds(1f);

             // As of this writing, adding a point takes about 175 ms. 
             // A failing add can take up to 3000 ms.
            var resultCollection = calibrationThread
                .CollectData(new CalibrationThread.Point(vector));

            // Wait for the call to finish
            yield return StartCoroutine(waitForResult(resultCollection));

            // React to the result of adding a point.
            if (resultCollection.Status == CalibrationStatus.Failure)
            {
                Debug.Log(
                    "There was an error gathering data for this calibration point: " + vector
                    );
            }
        }

        // Compute and apply the calibration.
        var computeResult = calibrationThread.ComputeAndApply();

        // Wait for the call to finish
        yield return StartCoroutine(waitForResult(computeResult));

        // Leave calibration mode.
        var leaveResult = calibrationThread.LeaveCalibrationMode();

         // Wait for the call to finish
        yield return StartCoroutine(waitForResult(leaveResult));

        // Stop the thread.
        calibrationThread.StopThread();
        calibrationThread = null;

        // finishing
        isCalibrating = false;
    }

    private IEnumerator waitForResult(CalibrationThread.MethodResult result)
    {
        // Wait for the thread to finish the blocking call.
        while (!result.Ready)
        {
            yield return new WaitForSeconds(0.02f);
        }

        Debug.Log(result);
    }
}
