using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Api : MonoBehaviour
{
	Api cobaApi;
	
	void Start()
	{
		cobaApi = this;
	}

	public IEnumerator HttpGetHistories(string username, string shoulder)
	{
		UnityWebRequest www = UnityWebRequest.Get("http://localhost:3333/history?username=" + username + "&hand=" + shoulder);
		yield return www.SendWebRequest();

		if (www.isNetworkError || www.isHttpError)
		{
			Debug.Log("Get histories failed! => " + www.error);
			yield return null;
		}
		else
		{
			Debug.Log("Get histories complete!");
			yield return www.downloadHandler.text;
		}
	}

	public IEnumerator HttpPostHistory(string username, string shoulder, double prediction, double actual)
	{
		WWWForm form = new WWWForm();
		form.AddField("username", username);
		form.AddField("hand", shoulder);
		form.AddField("prediction", prediction.ToString());
		form.AddField("actual", actual.ToString());

		UnityWebRequest www = UnityWebRequest.Post("http://localhost:3333/history", form);
		yield return www.SendWebRequest();

		if (www.isNetworkError || www.isHttpError)
		{
			Debug.Log("Get histories failed! => " + www.error);
			yield return null;
		}
		else
		{
			Debug.Log("Get histories complete!");
			yield return www.downloadHandler.text;
		}
	}
}
