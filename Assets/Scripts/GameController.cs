using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class GameController : MonoBehaviour {

    [SerializeField]
    public Transform stepObj,
        caughtTarget;

    public Transform[] target;
    public PlayerController player;

    public GameObject fish;

    public float currentTime;
    public float speed;
    public bool timeOn = true, 
        isAngleReached,
        onHook;

    public Image bar_time,
        ui_strike,
        ui_failed;

    public Text text_time, 
        text_score, 
        text_step, 
        text_angle,
        text_fishCaught;

    private int current;
    int angle,
        angleTarget,
        fishCaught,
        score,
        currentLevel;

    //public int stepDistance = 5;
    //public int maxStep = 10;

    // Use this for initialization
    void Start () {

        currentLevel = 1;
        currentTime = 10;
        fishCaught = 0;
        angle = 0;
        
        //menentukan sudut tujuan. sementara ditentukan oleh nilai random
        angleTarget = UnityEngine.Random.RandomRange(1, 180);
        Debug.Log("Target Sekarang = " + angleTarget);

        /*Instantiate Step
        for (int i = 0; i < maxStep; i++)
        {
            Instantiate(stepObj, new Vector3(i * stepDistance, -2, 0), Quaternion.identity);
            Debug.Log(stepObj.transform.position);

        }*/
    }

    // Update is called once per frame
    void Update () {

        //Pengulangan game sebanyak 10 kali
        if(currentLevel <= 10)
        {
            
            checkAngle();

            timeRun();

            updateFishOnHook();
        }

        


        /*fish movement
        if (fish[0].transform.position != target[current].position)
        {
            Vector3 pos = Vector3.MoveTowards(fish[0].transform.position, target[current].position, speed * Time.deltaTime);
            fish[0].GetComponent<Rigidbody2D>().MovePosition(pos);
        }*/

        //time bar update
        
    }

    private void updateFishOnHook()
    {
        if (onHook)
        {
            Instantiate(fish, new Vector2(caughtTarget.transform.position.x, caughtTarget.transform.position.y), Quaternion.identity);
        }
    }

    private void timeRun()
    {
        if (timeOn == true && isAngleReached == false)
        {
            if (currentTime >= 0)
            {
                currentTime -= Time.deltaTime;
                text_time.text = System.Math.Round(currentTime).ToString();
                bar_time.fillAmount -= Time.deltaTime * 0.1f;

            }
            else
            {
                Debug.Log("Time's Up! Game Over");
            }
        }
        else if (timeOn == true && isAngleReached == true)
        {
            //hentikan waktu
            timeOn = false;

            Debug.Log("Sudut Tercapai! Lanjut Level");
            fishCaught++;
            text_fishCaught.text = fishCaught + "/10";
            isAngleReached = false;


        }
    }

    private void checkAngle()
    {
        
        if (angle != angleTarget)
        {
            isAngleReached = false;
            if (Input.GetKey(KeyCode.DownArrow))
            {
                angle--;
                text_angle.text = angle.ToString();
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                angle++;
                text_angle.text = angle.ToString();

            }

        }
        else {

            isAngleReached = true;

            //
            /*
            1. Play animasi narik kail
            2. Ikan Nambah
        
            */

            //player.myAnim.SetBool("pull", true);
            //player.myAnim.SetBool("throw", true);
            Debug.Log("Animasi pull dieksekusi");
            ui_strike.gameObject.SetActive(true);

            //set back hand to 0
            resetAngle();

        }

        
    }

    private void resetAngle()
    {
        player.pullHand.transform.rotation = Quaternion.identity;
        player.anglePointer.transform.rotation = Quaternion.identity;

        angle = 0;
        text_angle.text = "0";
        StartCoroutine(waitFor(1.0f));
        Debug.Log("Nunggu selesai");
        
    }

    IEnumerator waitFor(float duration)
    {

        yield return new WaitForSeconds(duration);

        ui_strike.gameObject.SetActive(false);

    }
}
