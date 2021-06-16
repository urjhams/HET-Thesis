using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class RecordRunner : MonoBehaviour
{
    enum RecordState
    {
        Off, On
    }

    public Sprite backgroundNormal;
    public Sprite backgroundRecording;
    
    public GameObject background;

    private RecordState currentRecordState = RecordState.Off;

    private List<HeadState> tempPetchValues = new List<HeadState>();

    private float currentStablePitch = 0F;

    private float estimatePitchDifference = 3;

    [HideInInspector]
    public float currentPitchValue = 0F;

    void Start()
    {
        
    }

    void Update()
    {
        Debug.Log(currentPitchValue);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu & Calibration");
        }
        nodRecognition();
    }

    private void nodRecognition()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (Application.isEditor)
            {
                return;
            }
            if (currentRecordState == RecordState.Off)
            {
                currentRecordState = RecordState.On;
                currentStablePitch = currentPitchValue;
                background
                    .GetComponent<SpriteRenderer>()
                    .sprite = backgroundRecording;
            }

            if (currentRecordState == RecordState.On)
            {
                switch (currentPitchValue)
                {
                    case float value when (value <= currentStablePitch - estimatePitchDifference):
                        tempPetchValues.Add(HeadState.Up);
                        break;
                    case float value when (value >= currentStablePitch + estimatePitchDifference):
                        tempPetchValues.Add(HeadState.Down);
                        break;
                    default:
                        tempPetchValues.Add(HeadState.Stable);
                        break;
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Space)) 
        {
            if (Application.isEditor)
            {
                return;
            }
            // turn to Off
            if (currentRecordState == RecordState.On)
            {
                currentRecordState = RecordState.Off;
                background
                    .GetComponent<SpriteRenderer>()
                    .sprite = backgroundNormal;
                // save data
                string textToSave = "";
                foreach (HeadState value in tempPetchValues)
                {
                    var result = "HeadState." + value.ToString();
                    textToSave = textToSave + " " + result + ",";
                }
                string moment = DateTime.Now.ToFileTime().ToString();
                var frames = tempPetchValues.Count.ToString();
                string fileName = 
                    "data-" + moment + "-" + frames + "_frames" + ".txt";
                string directoryPath = Application.dataPath + "/" + "Saved test data";
                string path = directoryPath  + "/" + fileName;

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // This text is added only once to the file.
                if (!File.Exists(path))
                {
                    // Create a file to write to.
                    File.WriteAllText(path, textToSave);
                }

                // clear the temp array
                tempPetchValues = new List<HeadState>();
            }
        }       
    }
}
