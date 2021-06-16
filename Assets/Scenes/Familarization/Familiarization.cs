using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Familiarization : MonoBehaviour
{
    enum Position
    {
        Up, Down, Left, Right
    }
    
    public GameObject background;

    public GameObject sampleObject;
    
    public Sprite backgroundRecording;

    [HideInInspector]
    public bool didEyeSelect = false;

    [HideInInspector]
    public bool didHeadSelect = false;

    private double _confirmTime = 0.7;

    private double confirmTime = 0;

    public Sprite white;
    public Sprite blue;
    public Sprite yellow;
    public Sprite green;
    public Sprite red;
    public Sprite purple;

    public void Awake() {
        QualitySettings.vSyncCount = 0;     // disable vSync
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        // no idea why these value turned to `true` when the screen start 🤷🏻‍♂️
        // so just set them to false at start
        didEyeSelect = false;
        didHeadSelect = false;

        Helper.prepareCursors();
        randomizePosition();
        confirmTime = _confirmTime;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu & Calibration");
        }

        switch (Global.currentState)
        {
            case TrialState.Eye:
                tryEyes();
                break;
            case TrialState.Head:
                tryHead();
                break;
            case TrialState.HeadEye:
                tryHeadSupportEye();
                break;
            case TrialState.Order:
                tryEyeHeadOrder();
                break;
            default:
                break;
        }
    }

    private void randomizePosition()
    {
        var positions = Enum.GetValues(typeof(Position));
        var random = new System.Random();
        var newPosition = (Position)positions
            .GetValue(random.Next(positions.Length));

        switch (newPosition)
        {
            case Position.Up:
                // do nothing, since it is already on top from initial
                sampleObject.transform.position = new Vector2(0, 3);
                break;
            case Position.Down:
                sampleObject.transform.position = new Vector2(0, -3);
                break;
            case Position.Left:
                sampleObject.transform.position = new Vector2(-4, 0);
                break;
            case Position.Right:
                sampleObject.transform.position = new Vector2(4, 0);
                break;
        }
    }

    // ---------------------- conditions

    private void tryEyes()
    {
        if (this.didEyeSelect)
        {
            // eye lock time counting down, but will reset 
            // and stop the in next frame if there is no selected object
            confirmTime -= Time.deltaTime;

            if (confirmTime <= 0)
            {
                sampleObject
                    .GetComponent<SpriteRenderer>()
                    .sprite = green;
            }
            else
            {
                sampleObject
                    .GetComponent<SpriteRenderer>()
                    .sprite = blue;
            }
        }
        else
        {
            confirmTime = _confirmTime;
            sampleObject
                .GetComponent<SpriteRenderer>()
                .sprite = white;
        }
    }

    private void tryHead()
    {
        Debug.Log(didHeadSelect);
        if (this.didHeadSelect)
        {
            // head lock time counting down, but will reset 
            // and stop in the next frame if there is no selected object
            confirmTime -= Time.deltaTime;

            if (confirmTime <= 0)
            {
                sampleObject
                    .GetComponent<SpriteRenderer>()
                    .sprite = green;
            }
            else
            {
                sampleObject
                    .GetComponent<SpriteRenderer>()
                    .sprite = blue;
            }
        }
        else
        {
            confirmTime = _confirmTime;
            sampleObject
                .GetComponent<SpriteRenderer>()
                .sprite = white;
            return;
        }
    }

    private void tryHeadSupportEye()
    {
        HeadPlloter trackerInstance = GameObject
            .Find("headCursor")
            .GetComponent<HeadPlloter>();
        
        if (this.didEyeSelect)
        {
            // eye lock time counting down, but will reset 
            // and stop the in next frame if there is no selected object
            confirmTime -= Time.deltaTime;

            if (confirmTime <= 0)
            {
                sampleObject
                    .GetComponent<SpriteRenderer>()
                    .sprite = purple;
                
                if (trackerInstance.isObserving)
                {
                    // start observe the nod
                    trackerInstance.isObserving = true;
                }

                if (trackerInstance.didNod)
                {
                    sampleObject
                        .GetComponent<SpriteRenderer>()
                        .sprite = green;
                }
            }
            else
            {
                sampleObject
                    .GetComponent<SpriteRenderer>()
                    .sprite = blue;
            }
        }
        else
        {
            confirmTime = _confirmTime;
            trackerInstance.isObserving = false;
            trackerInstance.didNod = false;
            sampleObject
                .GetComponent<SpriteRenderer>()
                .sprite = white;
        }
    }

    private void tryEyeHeadOrder()
    {
        if (this.didEyeSelect)
        {
            if (this.didHeadSelect)
            {
                confirmTime -= Time.deltaTime;
                if (confirmTime <= 0)
                {
                    sampleObject
                        .GetComponent<SpriteRenderer>()
                        .sprite = green;
                }
            }
            else
            {
                confirmTime = _confirmTime;
                sampleObject
                    .GetComponent<SpriteRenderer>()
                    .sprite = blue;
            }
        } 
        else
        {
            confirmTime = _confirmTime;
            sampleObject
                .GetComponent<SpriteRenderer>()
                .sprite = white;
        }
    }
}
