using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour {

	public InputField input_username;
	public Text text_username;
    public GameObject panel_warning;
    public GameObject board_login;

    public void SignIn(){
		string username;

		username = input_username.text;
        Debug.Log(username);

        if (username == "")
        {
            Debug.Log("username empty broh");
            panel_warning.SetActive(true);
        }
        else
        {
            PlayerPrefs.SetString("username", input_username.text);
            text_username.text = "Welcome back, " + username;
            panel_warning.SetActive(false);
            board_login.SetActive(false);
        }


    }
}
