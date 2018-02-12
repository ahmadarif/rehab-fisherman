using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class GameController : MonoBehaviour {

    [SerializeField]
    public Transform stepObj,
        caughtTarget,
        angleTarget_ui;

    public Transform[] target;
    public PlayerController player;
    public MoveFish moveFish;

    public GameObject fish,
        gameover_ui;

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
        text_finalScore, 
        text_step, 
        text_angle,
        text_fishCaught;

    private int current;
    int angle,
        angleTarget,
        fishCaught,
        score,
        currentLevel;

    int[] levelPoint;

    //public int stepDistance = 5;
    //public int maxStep = 10;

    // Use this for initialization
    void Start () {

        currentLevel = 1;
        currentTime = 10;
        fishCaught = 0;
        angle = 0;
        score = 0;
        Debug.Log("Cobain VS Code");

        //menentukan sudut tujuan. sementara ditentukan oleh nilai random
        setAngleTarget();
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
        else
        {
            text_finalScore.text = "Score: " + text_score.text;
            gameover_ui.gameObject.SetActive(true);

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
            StartCoroutine(waitForFish(3.0f));
            
            //Instantiate(fish, new Vector2(caughtTarget.transform.position.x, caughtTarget.transform.position.y), Quaternion.identity);
            //Debug.Log("Ikan dibuat");
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
                ui_failed.gameObject.SetActive(true);
                SoundManager.PlaySound("failed");
                moveFish.moveToTarget = false;
                StartCoroutine(waitFor(1f));

                reset();
            }
        }
        else if (timeOn == true && isAngleReached == true)
        {
          //hentikan waktu
            timeOn = false;

            //ikan revert ke tempat awayTarget
            moveFish.transform.position = moveFish.awayTarget.position;

            Debug.Log("Sudut Tercapai! Lanjut Level. Target Sekarang: " + angleTarget);
            fishCaught++;
            text_fishCaught.text = fishCaught + "/10";
            isAngleReached = false;

            player.myAnim.SetBool("pull", true);
            StartCoroutine(waitFor(1f));

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
            ui_strike.gameObject.SetActive(true);

            //tambah score
            addScore();

            //suara dapet
            SoundManager.PlaySound("strike");

            //set back everything to 0
            reset();
            setAngleTarget();

        }

    }

    private void addScore()
    {
        score = score + (int)currentTime * 100;
        text_score.text = score.ToString();
    }

    private void setAngleTarget()
    {
        //sementara random
        angleTarget = UnityEngine.Random.RandomRange(50, 100);
        angleTarget_ui.transform.rotation = Quaternion.identity;
        angleTarget_ui.transform.Rotate(0, 0, -angleTarget);

        //anglePointer.transform.Rotate(0, 0, pullSpeed);

    }

    private void reset()
    {

        //set pullhand jadi 0
        //Quaternion target = Quaternion.Euler(0,0, 340f);
        //player.pullHand.transform.rotation = Quaternion.Slerp(player.pullHand.transform.rotation, target, 0);
        //player.pullHand.transform.rotation = Quaternion.identity;

        //set anglePointer jadi 0
        player.anglePointer.transform.rotation = Quaternion.identity;

        StartCoroutine(waitFor(1.0f));

        //set angle dan text_angle jadi 0
        angle = 0;
        text_angle.text = angle.ToString();

        //set waktu dan bar waktu jadi 10
        currentTime = 10;
        text_time.text = System.Math.Round(currentTime).ToString();
        bar_time.fillAmount = currentTime * 10;
        


        StartCoroutine(waitFor(1.0f));

        Debug.Log("Nunggu selesai");
        currentLevel++;
    }

    IEnumerator waitFor(float duration)
    {

        yield return new WaitForSeconds(duration);
        timeOn = true;
        //set line jadi panjang
        player.line.transform.localScale = new Vector3(1, 1f, 1);
        isAngleReached = false;
        moveFish.moveToTarget = true;
        player.myAnim.SetBool("pull", false);
        ui_strike.gameObject.SetActive(false);
        ui_failed.gameObject.SetActive(false);

    }

    IEnumerator waitForFish(float duration)
    {

        yield return new WaitForSeconds(duration);
        onHook = false;
    }

}
