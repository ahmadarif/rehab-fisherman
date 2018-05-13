using UnityEngine;
using JsonFx.Json;
using System.Collections;

public class Api : MonoBehaviour {

	// Use this for initialization
	void Start () {

        
        StartCoroutine(Test());
	}

    IEnumerator Test()
    {
        Debug.Log("Proses");
        WWWForm form = new WWWForm();
        form.AddField("username", "eky");
        WWW www = new WWW("http://localhost:3333");

        yield return www;
        var reader = new JsonReader();
        dynamic output = reader.Read(www.text);

        //Debug.Log(output);

        Debug.Log("Finish");      
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
