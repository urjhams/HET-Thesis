using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public InputField nameField;
    
    public void saveCurrentName()
    {
        Global.participantName = nameField.text;
    }
    void Awake()
    {
        nameField.text = Global.participantName;
    }
    void OnDestroy()
    {
        saveCurrentName();
    }
}
