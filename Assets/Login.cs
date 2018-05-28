using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour {

	public InputField input_username;
	public Text text_username;


	public void SignIn(){
		string username;

		PlayerPrefs.SetString("username", input_username.text);
		username = input_username.text;
		text_username.text = "Welcome back, "+ username;
	}
}
