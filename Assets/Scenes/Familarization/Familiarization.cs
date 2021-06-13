using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Familiarization : MonoBehaviour
{
    enum Position
    {
        Up, Down, Left, Right
    }

    enum RecordState
    {
        Off, On
    }
    
    public GameObject background;

    public GameObject sampleObject;

    public Sprite backgroundNormal;
    
    public Sprite backgroundRecording;

    [HideInInspector]
    public bool didEyeSelect = false;

    [HideInInspector]
    public bool didHeadSelect = false;

    private RecordState currentRecordState = RecordState.Off;

    [HideInInspector]
    public float currentPitchValue = 0F;

    private List<HeadState> tempPetchValues = new List<HeadState>();

    private float currentStablePitch = 0F;

    private float estimatePitchDifference = 3;

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
            case TrialState.Trial:
                nodRecognition();
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

    private void nodRecognition()
    {
        HeadHandler handler = GameObject
            .Find("headCursor")
            .GetComponent<HeadHandler>();
        
        if (Input.GetKey(KeyCode.C))
        {
            if (!handler.isObserving)
            {
                handler.isObserving = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.C)) 
        {
            if (handler.isObserving)
            {
                handler.isObserving = false;
            }
        }

        if (handler.didNod && !handler.isObserving)
        {
            background
                .GetComponent<SpriteRenderer>()
                .sprite = backgroundRecording;
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
        HeadHandler trackerInstance = GameObject
            .Find("headCursor")
            .GetComponent<HeadHandler>();
        
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
