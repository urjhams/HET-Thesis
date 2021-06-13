using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public struct TrialData
{
    private double time;
    private Result result;

    public TrialData(Result result, double time)
    {
        this.time = time;
        this.result = result;
    }

    public double getTime()
    {
        return this.time;
    }

    public Result getResult()
    {
        return this.result;
    }
}
public enum Result
{
    Correct, Incorrect, Overtime
}

public class EyeOnlyBaseRunner : MonoBehaviour
{
    public Text debugText;

    private bool ready = false;

    [HideInInspector]
    public Result result = Result.Overtime;

    [HideInInspector]
    public TrialData[] tempTrialData = new TrialData[10];

    // current selected pattern by eye cursor
    [HideInInspector]
    public Global.GameObjectPattern selectedPatternSet;

    // current selected pattern by head cursor
    [HideInInspector]
    public Global.GameObjectPattern headSelectedPatternSet;

    // The main pattern object
    public GameObject[] mainObj;

    [HideInInspector]
    public Global.GameObjectPattern mainObjPattern;

    public GameObject[] subObjList;

    // pattern objects list
    [HideInInspector]
    public Global.GameObjectPatternGroup subObjsGroup;

    public GameObject mainFrame;

    // the frame object list (which co-responding 
    // with pattern objects as container)
    public GameObject[] subFrame;

    // list of sprites for patterns
    public Sprite[] spriteList;

    // the trial time left, will counted down right from start
    [HideInInspector]
    public const double _timeLeft = 25;

    [HideInInspector]
    public double timeLeft = 25;

    // after this amount of time when eye gaze hit the objects, 
    // it will be counted as "lock" (eye only scenario)
    [HideInInspector]
    public double _lockTime = 0;

    [HideInInspector]
    public double lockTime = 0;

    // after this amount of seconds when selecting, 
    // active confirmation result (correct or incorrect)
    [HideInInspector]
    public double _confirmTime = 0;

    [HideInInspector]
    public double confirmTime = 0;

    public Sprite white;
    public Sprite blue;
    public Sprite yellow;
    public Sprite green;
    public Sprite red;
    public Sprite purple;

    [HideInInspector]
    public int trialCount = 1;

    [HideInInspector]
    public const int maxTrialsNumber = 10;

    [HideInInspector]
    public bool trialDone = false;

    // 2s for state delay, 0.5 for baseline screen
    [HideInInspector]
    public double delayTime = 4;

    private double readyTime = 2.5;

    public void Awake()
    {
        QualitySettings.vSyncCount = 0;     // disable vSync
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        // no idea why delay time is turned to 2.5 at initializing step 
        // so need to set it in Start()
        delayTime = 4;

        if (!Global.inDebugMode)
        {
            debugText.enabled = false;
        }

        fillObjectsToPattern();
        fillObjectsSprite();
        prepareComponents();
        Helper.prepareCursors();
    }

    void Update()
    {
        // Debug.Log(((int)(1.0f / Time.smoothDeltaTime)).ToString());
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu & Calibration");
        }

        if (Input.GetKeyDown(KeyCode.Space) && !ready)
        {
            ready = true;
            GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;
        }

        if (readyTime >= 0)
        {
            readyTime -= Time.deltaTime;
            mainFrame.SetActive(false);
            foreach (GameObject frame in subFrame)
            {
                frame.SetActive(false);
            }
            return;
        }
        else
        {
            mainFrame.SetActive(true);
            foreach (GameObject frame in subFrame)
            {
                frame.SetActive(true);
            }
        }

        if (!ready)
        {
            return;
        }

        trialTimeHandle();

        switch (Global.currentState)
        {
            case TrialState.Eye:
                updateInEyeOnly();
                break;
            case TrialState.Head:
                updateInHeadOnly();
                break;
            case TrialState.HeadEye:
                updateHeadSupportEye();
                break;
            case TrialState.Order:
                updateEyeHeadOrder();
                break;
            case TrialState.Trial:
                break;
        }
        trialDoneTimeHandle();
    }

    // ------------------------------- trials

    // Condition 1
    private void updateInEyeOnly()
    {
        if (trialDone)
        {
            return;
        }

        if (selectedPatternSet != null
            && selectedPatternSet.objects.Length > 0
            && lockTime > 0)
        {
            selectedPatternSet
                .objects[0]
                .transform
                .parent
                .gameObject
                .GetComponent<SpriteRenderer>()
                .sprite = blue;
        }
        else
        {
            // reset
            resetLockTime();
            return;
        }

        // eye lock time counting down, but will reset 
        // and stop the in next frame if there is no selected object
        lockTime -= Time.deltaTime;

        if (selectedPatternSet != null && lockTime <= 0)
        {
            if (Helper.samePattern(selectedPatternSet, mainObjPattern))
            {
                selectedPatternSet
                    .objects[0]
                    .transform
                    .parent
                    .gameObject
                    .GetComponent<SpriteRenderer>()
                    .sprite = green;

                result = Result.Correct;
                var takenTime = _timeLeft - timeLeft;
                trialDoneHandle(result, takenTime);
            }
            else
            {
                selectedPatternSet
                    .objects[0]
                    .transform
                    .parent
                    .gameObject
                    .GetComponent<SpriteRenderer>()
                    .sprite = red;

                result = Result.Incorrect;
                var takenTime = _timeLeft - timeLeft;
                trialDoneHandle(result, takenTime);
            }
        }
    }

    // Condition 2
    private void updateInHeadOnly()
    {
        if (headSelectedPatternSet == null)
        {
            return;
        }

        if (trialDone)
        {
            return;
        }

        if (headSelectedPatternSet != null
            && headSelectedPatternSet.objects.Length > 0)
        {
            headSelectedPatternSet
            .objects[0]
            .transform
            .parent
            .gameObject
            .GetComponent<SpriteRenderer>()
            .sprite = blue;
        }
        else
        {
            // reset
            resetLockTime();
            return;
        }

        // head lock time counting down, but will reset 
        // and stop in the next frame if there is no selected object
        lockTime -= Time.deltaTime;

        if (lockTime <= 0)
        {
            if (Helper.samePattern(headSelectedPatternSet, mainObjPattern))
            {
                headSelectedPatternSet
                    .objects[0]
                    .transform
                    .parent
                    .gameObject
                    .GetComponent<SpriteRenderer>()
                    .sprite = green;

                result = Result.Correct;
                var takenTime = _timeLeft - timeLeft;
                trialDoneHandle(result, takenTime);
            }
            else
            {
                headSelectedPatternSet
                    .objects[0]
                    .transform
                    .parent
                    .gameObject
                    .GetComponent<SpriteRenderer>()
                    .sprite = red;

                result = Result.Incorrect;
                var takenTime = _timeLeft - timeLeft;
                trialDoneHandle(result, takenTime);
            }
        }
    }

    // Condition 3
    /*
    Hypothesis: 
    We could have some state like: Up - 1, Stable - 2, Down - 3 like Hidden Markov Models
    Use a stack to store the sequence, like { 1, 1, 1, 1, 2, 2, 2, 3, 3, 3, 3, 3 }
    with each element represent for a state in each frame.
    So if we could use some patterns to compare, if it matched, in the next frame we will
    count as a nod detected. The requirement for the observation in this scenario is only 
    when the eye cursor is aiming at an object, or is selecting a pattern.
    */
    private void updateHeadSupportEye()
    {
        HeadHandler trackerInstance = GameObject
            .Find("headCursor")
            .GetComponent<HeadHandler>();

        if (trialDone)
        {
            return;
        }

        if (selectedPatternSet != null
            && selectedPatternSet.objects.Length > 0)
        {
            lockTime -= Time.deltaTime;
            if (lockTime <= 0)
            {
                selectedPatternSet
                    .objects[0]
                    .transform
                    .parent
                    .gameObject
                    .GetComponent<SpriteRenderer>()
                    .sprite = purple;

                if (!trackerInstance.isObserving)
                {
                    // start observe the nod
                    trackerInstance.isObserving = true;
                }

                if (trackerInstance.didNod)
                {
                    if (Helper.samePattern(selectedPatternSet, mainObjPattern))
                    {
                        selectedPatternSet
                            .objects[0]
                            .transform
                            .parent
                            .gameObject
                            .GetComponent<SpriteRenderer>()
                            .sprite = green;

                        result = Result.Correct;
                        var takenTime = _timeLeft - timeLeft;
                        trialDoneHandle(result, takenTime);
                    }
                    else
                    {
                        selectedPatternSet
                            .objects[0]
                            .transform
                            .parent
                            .gameObject
                            .GetComponent<SpriteRenderer>()
                            .sprite = red;

                        result = Result.Incorrect;
                        var takenTime = _timeLeft - timeLeft;
                        trialDoneHandle(result, takenTime);
                    }
                }
            }
            else
            {
                selectedPatternSet
                    .objects[0]
                    .transform
                    .parent
                    .gameObject
                    .GetComponent<SpriteRenderer>()
                    .sprite = blue;
            }
        }
        else
        {
            resetLockTime();
        }
    }

    // Condition  4 - concept 2
    private void updateEyeHeadOrder()
    {
        if (trialDone)
        {
            return;
        }

        if (selectedPatternSet != null
            && selectedPatternSet.objects.Length > 0)
        {
            if (headSelectedPatternSet != null
                && headSelectedPatternSet == selectedPatternSet)
            {
                confirmTime -= Time.deltaTime;
                if (confirmTime <= 0)
                {
                    if (Helper.samePattern(selectedPatternSet, mainObjPattern))
                    {
                        selectedPatternSet
                            .objects[0]
                            .transform
                            .parent
                            .gameObject
                            .GetComponent<SpriteRenderer>()
                            .sprite = green;
                        result = Result.Correct;
                        var takenTime = _timeLeft - timeLeft;
                        trialDoneHandle(result, takenTime);
                    }
                    else
                    {
                        selectedPatternSet
                            .objects[0]
                            .transform
                            .parent
                            .gameObject
                            .GetComponent<SpriteRenderer>()
                            .sprite = red;
                        result = Result.Incorrect;
                        var takenTime = _timeLeft - timeLeft;
                        trialDoneHandle(result, takenTime);
                    }
                }
            }
            else
            {
                selectedPatternSet
                    .objects[0]
                    .transform
                    .parent
                    .gameObject
                    .GetComponent<SpriteRenderer>()
                    .sprite = blue;
                // reset
                resetLockTime();
            }
        }
        else
        {
            // reset
            resetLockTime();
        }
    }


    // ------------------------------- essentials
    private void trialTimeHandle()
    {
        if (!trialDone)
        {
            timeLeft -= Time.deltaTime;
        }
        if (timeLeft <= 0 && !trialDone)
        {
            trialDoneHandle(Result.Overtime, 25.0);
        }
    }

    private void trialDoneHandle(Result result, double time)
    {
        time = Math.Round(time, 2);
        debugText.text = "result: " + result.ToString() +
            " - time: " + time.ToString() + "s - trialCount: " +
            trialCount.ToString() + "/" + maxTrialsNumber.ToString();

        if (trialCount == maxTrialsNumber)
        {
            saveTrialData(new TrialData(result, time));
            saveData();
            trialCount++;
        }
        else if (trialCount < maxTrialsNumber)
        {
            saveTrialData(new TrialData(result, time));
            trialCount++;
        }
        trialDone = true;
    }

    private void trialDoneTimeHandle()
    {
        if (trialDone)
        {
            delayTime -= Time.deltaTime;
            if (delayTime > 0 && delayTime <= 2)
            {
                mainFrame.SetActive(false);
                foreach (GameObject frame in subFrame)
                {
                    frame.SetActive(false);
                }
            }
            if (delayTime <= 0)
            {
                // reset all the subframe's rectangle to white
                foreach (GameObject rect in subFrame)
                {
                    rect.GetComponent<SpriteRenderer>().sprite = white;
                }

                if (trialCount <= maxTrialsNumber)
                {
                    mainFrame.SetActive(true);
                    foreach (GameObject frame in subFrame)
                    {
                        frame.SetActive(true);
                    }
                }
                trialFinish();
            }
        }
    }

    public virtual void trialFinish()
    {
        if (trialCount > maxTrialsNumber)
        {
            SceneManager.LoadScene("Menu & Calibration");
        }
        else
        {
            trialDone = false;
            // reshuffle for new sprites
            fillObjectsSprite();

            // reset
            timeLeft = _timeLeft;
            delayTime = 5;
            resetLockTime();

            // reset selected pattern if needed
            selectedPatternSet = null;
            headSelectedPatternSet = null;

            // reset HeadHandler state in HeadEye mode
            var head = GameObject
                .Find("headCursor")
                .GetComponent<HeadHandler>();

            if (head != null)
            {
                head.didNod = false;
                head.isObserving = false;   // stop the Observe if needed
            }
        }
    }

    public virtual void fillObjectsSprite() { }

    public virtual void fillObjectsToPattern() { }

    private void saveTrialData(TrialData data)
    {
        var index = trialCount - 1;
        tempTrialData[index] = new TrialData(data.getResult(), data.getTime());
    }

    private void saveData()
    {
        var methodName = "";
        switch (Global.currentState)
        {
            case TrialState.Eye:
                methodName = "Eyes-only";
                break;
            case TrialState.Head:
                methodName = "Head-only";
                break;
            case TrialState.HeadEye:
                methodName = " Multimodal 1";
                break;
            case TrialState.Order:
                methodName = " Multimodal 2";
                break;
            case TrialState.Trial:
                break;
        }

        for (int index = 0; index < tempTrialData.Length; index++)
        {
            var resultString = "N/A";
            switch (tempTrialData[index].getResult())
            {
                case Result.Correct:
                    resultString = "Correct";
                    break;
                case Result.Incorrect:
                    resultString = "Incorrect";
                    break;
                case Result.Overtime:
                    resultString = "Overtime";
                    break;
            }
            var time = tempTrialData[index].getTime();
            var data = new string[3]
            {
                (index + 1).ToString(),
                resultString,
                time.ToString()
            };

            var level = "";
            switch (Global.currentLevel)
            {
                case TrialLevel.Easy:
                    level = "Easy";
                    break;
                case TrialLevel.Hard:
                    level = "Hard";
                    break;
                default:
                    break;
            }

            string fileName = Global.participantName + " - " + methodName + "-" + level;
            CSVManager.appendtoFile(fileName, data);
        }
    }

    public void prepareComponents()
    {
        switch (Global.currentState)
        {
            case TrialState.Eye:
                _lockTime = 0.7;
                break;
            case TrialState.Head:
                _lockTime = 0.7;
                break;
            case TrialState.HeadEye:
                _lockTime = 0.7;
                break;
            case TrialState.Order:
                _confirmTime = 0.7;
                break;
            case TrialState.Trial:
                break;
        }
        resetLockTime();
    }

    public void resetLockTime()
    {
        lockTime = _lockTime;
        confirmTime = _confirmTime;
    }

    public void fillObjectsWithSprites(int length, int components = 4)
    {
        // create an array of indexs in pattern spirtes array
        int[] indexs = new int[length];
        for (int index = 0; index < length; index++)
        {
            indexs[index] = index;
        }

        // the array to hole our suffeled sets of patterns
        int[][] finalOrderSets = new int[length][];

        // assign the array above with random values from the array of indexs
        for (int index = 0; index < indexs.Length; index++)
        {
            // first, make a randomly suffled version of indexs array
            int[] suffledIndexs = indexs;
            Utility.reshuffle(suffledIndexs);

            int[] array = new int[components];
            // then assign for first 4 items in to the current position of the final order array
            for (int componentIndex = 0; componentIndex < components; componentIndex++)
            {
                array[componentIndex] = suffledIndexs[componentIndex];
            }
            finalOrderSets[index] = array;
        }

        var random = new System.Random();

        // get the random index, this will be the index of the pattern 
        // that will be used for main pattern
        int randomIndex = random.Next(0, finalOrderSets.Length - 1);

        for (int index = 0; index < components; index++)
        {
            // apply the random set of sprite pattern into main Object
            mainObjPattern
                .objects[index]
                .GetComponent<SpriteRenderer>()
                .sprite = spriteList[finalOrderSets[randomIndex][index]];
        }
        // save the order into main object
        mainObjPattern.order = finalOrderSets[randomIndex];

        for (int index = 0; index < length; index++)
        {
            for (int innerIndex = 0; innerIndex < components; innerIndex++)
            {
                // fill the sprites for objects in pattern object at specific position of sub object
                subObjsGroup
                    .patterns[index]
                    .objects[innerIndex]
                    .GetComponent<SpriteRenderer>()
                    .sprite = spriteList[finalOrderSets[index][innerIndex]];
            }

            // save the current order into sub object
            subObjsGroup.patterns[index].order = finalOrderSets[index];

            // Apply patterns into objects in subFrame array
            try
            {
                subFrame[index]
                    .GetComponent<ColliderHandleHard>()
                    .representPatternSet = subObjsGroup.patterns[index];
            }
            catch
            {
                Debug.Log(
                    "Cannot apply for `ColliderHandleHard` of subframe object at index "
                        + index
                );
            }
        }
    }

    public void fillGameObjectsToPattern(int groups, int components)
    {
        //------------------------- Main object set up
        // this pattern store x game objects component repesented x spirtes
        mainObjPattern = new Global.GameObjectPattern(components);
        mainObjPattern.objects = mainObj;

        //------------------------- Sub objects group set up
        // this pattern store 4 game objects repesented 4 spirtes
        // this group has 8 gameObjectPattern-s
        subObjsGroup = new Global.GameObjectPatternGroup(groups, components);

        var groupIndex = 0;
        var tempArray = new GameObject[components];
        var tempArrayIndex = 0;

        // There are 8 groups, which mean 8*4 = 24 enitities in subObjList
        for (int index = 0; index < subObjList.Length; index++)
        {
            tempArray[tempArrayIndex] = subObjList[index];
            tempArrayIndex++;

            // after each group (4 components), reset tempArrayIndex, increase group index
            if ((index + 1) % components == 0)
            {
                subObjsGroup.patterns[groupIndex].objects = tempArray;

                groupIndex++;
                tempArray = new GameObject[components];
                tempArrayIndex = 0;
            }
        }
    }

}
