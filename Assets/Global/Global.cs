using UnityEngine;
using System.Linq;
using System;

public enum TrialState
{
    Eye, Head, HeadEye, Order, None
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
    public static TrialState currentState = TrialState.None;

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
            int random = UnityEngine.Random.Range(index, array.Length);
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
            default:
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

    public static bool checkContained<T>(T[] lhs, T[] rhs)
    {
        if (lhs.Length < rhs.Length)
        {
            return false;
        }

        // get the diference in lenght to get max tries based on the indexes
        int differenceLength = lhs.Length - rhs.Length;

        /*
        the maximum tries is `difirence length + 1`.
        For example lhs = rhs (diffirence length = 0) which meean we jsut need 
        to compare 1 time from index 0 of lhs.
        If lhs has 1 element longer than rhs, then will be 2 compare(at index 0 and 1)
        */
        for (int index = 0; index <= differenceLength; index++)
        {
            // cut the lhs to size of rhs from index
            var segment = new ArraySegment<T>(lhs, index, rhs.Length).ToArray<T>();

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
}
