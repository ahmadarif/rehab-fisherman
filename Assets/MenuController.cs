using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class MenuController : MonoBehaviour {

    [SerializeField]
    private GameObject tutorial,
        selectArm,
        back;

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

}
