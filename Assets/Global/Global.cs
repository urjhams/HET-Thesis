using UnityEngine;
using Tobii.Research;
using UnityEngine.SceneManagement;

public enum TrialState
{
    Eye, Head, HeadEye, Order, Trial
}

public enum TrialLevel
{
    Easy, Hard, Familization, Calibration
}

public enum AttemptState
{
    Correct, Incorrect, Unknown, Timeout
}
public static class Global
{
    public static bool inDebugMode = false;
    public static string participantName = "";
    public static TrialState currentState = TrialState.Trial;

    public static TrialLevel currentLevel = TrialLevel.Familization;

    public class GameObjectPattern
    {
        public int[] order;
        public GameObject[] objects;

        public Sprite[] convertToSprites()
        {
            int length = this.objects.Length;
            if (length <= 0)
            {
                return null;
            }
            Sprite[] result = new Sprite[length];
            for (int index = 0; index < length; index++)
            {
                result[index] =
                this
                    .objects[index]
                    .GetComponent<SpriteRenderer>()
                    .sprite;
            }
            return result;
        }

        public GameObjectPattern(int components)
        {
            this.objects = new GameObject[components];
        }
    }

    public class GameObjectPatternGroup
    {
        public GameObjectPattern[] patterns;

        public GameObjectPatternGroup(int groups, int components)
        {
            this.patterns = new GameObjectPattern[groups];
            for (int index = 0; index < groups; index++)
            {
                this.patterns[index] = new GameObjectPattern(components);
            }
        }
    }
}

class Utility
{
    /// <summary>
    /// shuffle the elements in an array
    /// </summary>
    /// <param name = "array">The array that need 
    /// to reorder the elements inside</param>
    public static void reshuffle<T>(T[] array)
    {
        for (int index = 0; index < array.Length; index++)
        {
            T temp = array[index];
            int random = Random.Range(index, array.Length);
            array[index] = array[random];
            array[random] = temp;
        }
    }

    /// <summary>
    /// turn a NormalizedPoint2D to a 2D vector
    /// </summary>
    public static Vector2 ToVector2(NormalizedPoint2D value)
    {
        return new Vector2(value.X, value.Y);
    }

    /// <summary>
    /// turn a 3D point to a 3D vector
    /// </summary>
    public static Vector3 ToVector3(Point3D point)
    {
        return new Vector3(point.X, point.Y, point.Z);
    }

    /// <summary>
    /// get the first found eye tracker
    /// </summary>
    public static void getFirstEyeTracker(System.Action<IEyeTracker> handle)
    {
        EyeTrackerCollection trackers = EyeTrackingOperations.FindAllEyeTrackers();
        foreach (IEyeTracker eyeTracker in trackers)
        {
            Debug.Log(
                string.Format
                (
                    "Adress: {0}, Name: {1}, Mode: {2}, Serial number: {3}, Firmware version: {4}",
                    eyeTracker.Address,
                    eyeTracker.DeviceName,
                    eyeTracker.Model,
                    eyeTracker.SerialNumber,
                    eyeTracker.FirmwareVersion
                )
            );
        }
        if (trackers.Count > 0)
        {
            // --- connect 1st eye tracker
            var eyeTracker = trackers[0];
            Debug.Log("did get the eye tracker");
            handle(eyeTracker);
        }
    }
}

public class Helper {
    public static void changeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public static void prepareCursors()
    {
        switch (Global.currentState)
        {
            case TrialState.Eye:
                GameObject.Find("headCursor").SetActive(false);
                break;
            case TrialState.Head:
                GameObject.Find("eyeCursor").SetActive(false);
                break;
            case TrialState.HeadEye:
                // hide the render of head cursor
                // but still need it
                GameObject
                    .Find("headCursor")
                    .GetComponent<Renderer>()
                    .enabled = false;
                break;
            case TrialState.Order:
                break;
            case TrialState.Trial:
                GameObject
                    .Find("headCursor")
                    .GetComponent<Renderer>()
                    .enabled = false;
                break;
        }
    }

    public static bool samePattern(
        Global.GameObjectPattern patternA,
        Global.GameObjectPattern patternB
        )
    {
        bool result = true;
        for (int index = 0; index < patternA.objects.Length; index++)
        {
            var spriteA = patternA
                .objects[index]
                .GetComponent<SpriteRenderer>()
                .sprite
                .name;

            var spriteB = patternB
                .objects[index]
                .GetComponent<SpriteRenderer>()
                .sprite
                .name;

            if (!(spriteA.Trim().Equals(spriteB)))
            {
                result = false;
                break;
            }
        }
        return result;
    }
}
