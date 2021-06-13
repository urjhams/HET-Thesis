using UnityEngine;
using UnityEngine.SceneManagement;

public class Runner : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    public void quit()
    {
        if (!Application.isEditor)
        {
            Application.Quit();
        }
    }

    public void setState(string state)
    {
        switch (state)
        {
            case "Eye":
                Global.currentState = TrialState.Eye;
                break;
            case "Head":
                Global.currentState = TrialState.Head;
                break;
            case "HeadEye":
                Global.currentState = TrialState.HeadEye;
                break;
            case "Order":
                Global.currentState = TrialState.Order;
                break;
            default:
                return;
        }
    }

    public void setLevel(string level)
    {
        switch (level)
        {
            case "Easy":
                Global.currentLevel = TrialLevel.Easy;
                break;
            case "Hard":
                Global.currentLevel = TrialLevel.Hard;
                break;
            case "Familization":
                Global.currentLevel = TrialLevel.Familization;
                break;
            default:
                Global.currentLevel = TrialLevel.Calibration;
                break;
        }
    }
    
    public void changeScene()
    {
        switch (Global.currentLevel)
        {
            case TrialLevel.Easy:
                SceneManager.LoadScene("EyeOnluSceneEasy");
                break;
            case TrialLevel.Hard:
                SceneManager.LoadScene("EyeOnlySceneHard");
                break;
            case TrialLevel.Familization:
                SceneManager.LoadScene("Familiarization");
                break;
            case TrialLevel.Calibration:
                GameObject
                    .Find("Calibration")
                    .GetComponent<CalibrationRunner>()
                    .startCalibration();
                break;
            default:
                return;
        }
        
    }
}
