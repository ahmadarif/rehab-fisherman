using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject tutorial;
    public GameObject selectArm;
	public GameObject back;
	public GameObject signInForm;
	public GameObject loading;
    public Text text_username;
    public Toggle isTimeOnToggle;
	public Api api;

    void Start ()
	{
		string username = PlayerPrefs.GetString("username");

        PlayerPrefs.SetInt("toggleShoulder", 0);

		if (string.IsNullOrEmpty(username)) {
			signInForm.SetActive(true);
		} else {
			signInForm.SetActive(false);
            text_username.text = "Welcome back, " + username;
		}

		//StartCoroutine(Cek());
	}

	IEnumerator Cek()
	{
		CoroutineWithData cd = new CoroutineWithData(this, api.HttpGetHistories("arif", "right"));
		yield return cd.coroutine;

		Debug.Log("result is " + cd.result);
		Debug.Log("message is " + Global.Instance().message);
		

		//HistoryRes myObject = new HistoryRes();
		//JsonUtility.FromJsonOverwrite((string)cd.result, myObject);
		//Debug.Log(myObject.data[0]);
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

	public void SelectRightShoulder()
	{
		PlayerPrefs.SetString("shoulder","right");
	}

	public void SelectLeftShoulder()
	{
		PlayerPrefs.SetString("shoulder", "left");
	}

	public void Logout()
	{
		PlayerPrefs.SetString ("username", null);
		signInForm.SetActive(true);
	}
}
