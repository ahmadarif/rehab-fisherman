using UnityEngine;
using JsonFx.Json;
using System.Collections;
using UnityEngine.UI;

public class Api : MonoBehaviour {

    public string username, hand, prediction, actual;
    public Hashtable token;

    public InputField input_un;
    public InputField input_hand;

    public string idDB;
    public string usernameDB;
    public string handDB;
    public string predictionDB;
    public string actualDB;

	public static Hashtable[] dataHistories;
	public ArrayList bulkData = new ArrayList();

	public int[] actualData;
	public int[] predictionData;


    Api cobaApi;

	// Use this for initialization
	void Start () {
        cobaApi = this;
        
	}

	// Update is called once per frame
	void Update () {
	    
	}

    public IEnumerator PostHistories(string username, string hand, string prediction, string actual)
    {
        Hashtable data = new Hashtable();
        data.Add("username", username);
        data.Add("hand", hand);
        data.Add("prediction", prediction);
        data.Add("actual", actual);

        UnityHTTP.Request histories = new UnityHTTP.Request("post", "http://localhost:3333/history", data);

        histories.Send();
        Debug.Log("Data Posted!");

        yield return null;
    }

    public void HitHistories(string username, string hand, string prediction, string actual)
    {
        cobaApi.StartCoroutine(PostHistories(username, hand, prediction, actual));
    }

    public void TekanTombol()
    {
        HitHistories(username, hand, prediction, actual);
    }

    public IEnumerator GetHistories(string username, string hand)
    {
		Debug.Log ("Mulai GetHistories");
        UnityHTTP.Request histories = new UnityHTTP.Request("get", "http://localhost:3333/history?hand=" +hand+ "&username="+username);

        histories.Send(
            (request) =>
            {
                token = request.response.Object;
                if (token == null)
                {
                    Debug.Log("Gagal Parsing JSON");
                    return;
                }
                else
                {
                    Debug.Log("Berhasil Parsing JSON");
                    //Debug.Log("Username: "+ username +" Hand: " +hand);
                    //Debug.Log(token);

					bulkData = (ArrayList) token["data"];
					Debug.Log("Jumlah Data di DB: " +bulkData.Count);

					dataHistories = new Hashtable[bulkData.Count];

					for(int i=0 ; i<bulkData.Count ; i++){
						dataHistories[i] = (Hashtable) bulkData[i];
					}

					actualData = new int[bulkData.Count];
					predictionData = new int[bulkData.Count];

					for (int i = 0; i < bulkData.Count; i++) {
						actualData[i] = (int) dataHistories[i]["actual"];
						predictionData[i] = (int) dataHistories[i]["prediction"];
					}

                }
            }
        );


        yield return null;
    }

    public void AmbilData(string shoulder)
    {
		string username = PlayerPrefs.GetString ("username");
        cobaApi.StartCoroutine(GetHistories(username, shoulder));
    }

}
