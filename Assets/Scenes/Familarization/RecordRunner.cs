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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu & Calibration");
        }
        nodRecognition();
    }

    private void nodRecognition()
    {
        if (Input.GetKey(KeyCode.C))
        {
            if (currentRecordState == RecordState.Off)
            {
                currentRecordState = RecordState.On;
                currentStablePitch = currentPitchValue;
                background.GetComponent<SpriteRenderer>().sprite = backgroundRecording;
            }

            if (currentRecordState == RecordState.On)
            {
                if (currentPitchValue <= currentStablePitch - estimatePitchDifference)
                {
                    tempPetchValues.Add(HeadState.Up);
                }
                else if (currentPitchValue >= currentStablePitch + estimatePitchDifference)
                {
                    tempPetchValues.Add(HeadState.Down);
                }
                else
                {
                    tempPetchValues.Add(HeadState.Stable);
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.C)) 
        {
            // turn to Off
            if (currentRecordState == RecordState.On)
            {
                currentRecordState = RecordState.Off;
                background.GetComponent<SpriteRenderer>().sprite = backgroundNormal;
                // save data
                string textToSave = "";
                foreach (HeadState value in tempPetchValues)
                {
                    var result = "";
                    switch (value)
                    {
                        case HeadState.Up:
                            result = "Up";
                            break;
                        case HeadState.Down:
                            result = "Down";
                            break;
                        case HeadState.Stable:
                            result = "Stable";
                            break;
                    }
                    textToSave = textToSave + "  " + result;
                }
                string moment = DateTime.Now.ToFileTime().ToString();
                string fileName = "data_" + moment + ".txt";
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
