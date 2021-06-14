using UnityEngine;

public enum TrialState
{
    Eye, Head, HeadEye, Order, Trial
}

public enum TrialLevel
{
    Easy, Hard, Familization
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
}

public class Helper {
    public static void prepareCursors()
    {
        switch (Global.currentState)
        {
            case TrialState.Eye:
                GameObject.Find("headCursor").SetActive(false);
                break;
            case TrialState.Head:
                GameObject.Find("GazePlot").SetActive(false);
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
