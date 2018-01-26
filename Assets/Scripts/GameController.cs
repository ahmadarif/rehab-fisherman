using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour {
    public Transform stepObj;
    public int stepDistance = 5;
    public int maxStep = 10;

    public float currentTime = 10;
    public bool timeOn = true;
    public Image bar_time;
    public Text text_time, text_score, text_step;


    // Use this for initialization
    void Start () {
        /*for (int i = 0; i < maxStep; i++)
        {
            Instantiate(stepObj, new Vector3(i * stepDistance, -2, 0), Quaternion.identity);
            Debug.Log(stepObj.transform.position);

        }*/
    }
	
	// Update is called once per frame
	void Update () {
        if (timeOn)
        {
            if (currentTime >= 0)
            {
                currentTime -= Time.deltaTime;
                text_time.text = System.Math.Round(currentTime).ToString();
                bar_time.fillAmount -= Time.deltaTime * 0.1f;

            }
            else
            {
                Debug.Log("Time's Up");
            }
        }
    }
}
