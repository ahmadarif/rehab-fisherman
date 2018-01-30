using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour {
    public Transform stepObj;
    public Transform[] fish;
    public Transform[] target;
    //public int stepDistance = 5;
    //public int maxStep = 10;

    public float currentTime = 10;
    public float speed;
    public bool timeOn = true;
    public Image bar_time;
    public Text text_time, text_score, text_step;

    private int current;

    // Use this for initialization
    void Start () {

        /*Instantiate Step
        for (int i = 0; i < maxStep; i++)
        {
            Instantiate(stepObj, new Vector3(i * stepDistance, -2, 0), Quaternion.identity);
            Debug.Log(stepObj.transform.position);

        }*/
    }
	
	// Update is called once per frame
	void Update () {

        //fish movement
        if (fish[0].transform.position != target[current].position)
        {
            Vector3 pos = Vector3.MoveTowards(fish[0].transform.position, target[current].position, speed * Time.deltaTime);
            fish[0].GetComponent<Rigidbody2D>().MovePosition(pos);
        }

        //time bar update
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
