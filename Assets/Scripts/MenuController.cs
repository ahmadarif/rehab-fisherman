using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    public GameObject tutorial;
    public GameObject selectArm;
    public GameObject back;

    public Toggle isTimeOnToggle;

    void Start () {
        PlayerPrefs.SetString("TimeOn", "timeOnTrue");
    }

    public void LoadScene(string sceneName)
    {
        SoundManager.PlaySound("button_click");
        Application.LoadLevel(sceneName);
    }

    public void LoadTutorial()
    {
        SoundManager.PlaySound("button_click");
        tutorial.SetActive(true);
        back.SetActive(true);
    }

    public void Home()
    {
        SoundManager.PlaySound("button_click");
        tutorial.SetActive(false);
        selectArm.SetActive(false);
        back.SetActive(false);
    }

    public void SelectArm()
    {
        SoundManager.PlaySound("button_click");
        selectArm.SetActive(true);
        back.SetActive(true);
    }

    public void ToggleTime()
    {
        if (isTimeOnToggle.isOn)
        {
            PlayerPrefs.SetString("TimeOn", "timeOnTrue");
        }
        else
        {
            PlayerPrefs.SetString("TimeOn", "timeOnFalse");
        }
    }
}
