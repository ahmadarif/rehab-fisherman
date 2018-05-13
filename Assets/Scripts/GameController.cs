using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class GameController : MonoBehaviour {

    [SerializeField]
    public Transform stepObj,
        caughtTarget,
        angleTarget_ui;

    public Transform[] target;
    public PlayerController player;
    public MoveFish moveFish;

    public GameObject fish,
        gameover_ui,
        time_ui;

    public float currentTime,
        speed;

    public bool timeOn = true,
        timeChallenge, 
        isAngleReached,
        isTargetToZero = false,
        onHook;

    public Image bar_time,
        ui_strike,
        ui_failed;

    public Text text_time, 
        text_score,
        text_finalScore, 
        text_step, 
        text_angle,
        text_fishCaught,
        text_target,
        status_player;

    private int current;

    float currentAngle,
        defaultTarget = 15,
        angleTarget;

    int fishCaught,
        score,
        currentLevel;

    //double currentAngle;

    int[] levelPoint;

    double[] data = { 40, 45, 46, 57, 58, 54, 56, 57, 58, 60 };

    // Kinect variables
    private KinectManager manager;

    // Use this for initialization
    void Start ()
    {
        // Get kinect instance
        manager = KinectManager.Instance;

        currentLevel = 1;
        currentTime = 10;
        fishCaught = 0;
        currentAngle = 0;
        score = 0;

        isTargetToZero = false;

        //menentukan sudut tujuan
        setAngleTarget();

        //Check is time on or off from PlayerPrefs("TimeOn") from Menu scene
        string isTimeOn = PlayerPrefs.GetString("TimeOn");
        
        if (isTimeOn == "timeOnTrue")
        {
            Debug.Log("Pake Waktu");
            timeChallenge = true;
        }
        else if(isTimeOn == "timeOnFalse")
        {
            Debug.Log("Tidak Pake Waktu");
            timeChallenge = false;
        }

        bool isDebug = true;

        // cek kalman filter
        KalmanFilter kf = new KalmanFilter(data, isDebug);
        double result = kf.process(0.1);
    }

    // Update is called once per frame
    void Update ()
    {
        UpdateKinectUser();

        if (timeChallenge == true)
        {
            PlayGameWithTime();
        }
        else
        {
            time_ui.SetActive(false);
            PlayGameWithoutTime();
        }

        
    }

    private void PlayGameWithoutTime()
    {
        time_ui.SetActive(false);
        checkAngle();
    }

    private void PlayGameWithTime()
    {
        time_ui.SetActive(true);

        //Pengulangan game sebanyak 10 kali
        if (currentLevel <= 10)
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

            Debug.Log("Sudut Tercapai! Lanjut Level. Target Sekarang: " + angleTarget);
            fishCaught++;
            text_fishCaught.text = fishCaught + "/10";
            isAngleReached = false;
        }

    }

    private void pullFishingRod()
    {
        player.myAnim.SetBool("pull", true);
        StartCoroutine(waitFor(1f));

        //ikan revert ke tempat awayTarget
        moveFish.transform.position = moveFish.awayTarget.position;
    }

    private void checkAngle()
    {
        float tempTarget;

        if (!isTargetToZero)
        {

            tempTarget = angleTarget;
            
            angleTarget_ui.transform.rotation = Quaternion.identity;
            angleTarget_ui.transform.Rotate(0, 0, Mathf.RoundToInt((float)-angleTarget));
        }
        else
        {
            tempTarget = defaultTarget;

            angleTarget_ui.transform.rotation = Quaternion.identity;
            angleTarget_ui.transform.Rotate(0, 0, Mathf.RoundToInt((float)-tempTarget));
        }

        Debug.Log("Temp Target: " + tempTarget + " | Is Target Zero: "+ isTargetToZero);
        
        if (currentAngle != tempTarget)
        {
            isAngleReached = false;
            if (Input.GetKey(KeyCode.DownArrow))
            {
                currentAngle--;
                text_angle.text = currentAngle.ToString();
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                currentAngle++;
                text_angle.text = currentAngle.ToString();
            }
        }
        else
        {
            // TODO : masukan logika untuk menurunkan lengan
            

            isAngleReached = true;
            ui_strike.gameObject.SetActive(true);

            // tambah score
            addScore();

            // suara dapet
            SoundManager.PlaySound("strike");

            // animasi tarik pancingan
            pullFishingRod();

            // set back everything to 0
            reset();

            // revert menjadi titik awal
            isTargetToZero = !isTargetToZero;

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
        // sementara random *TODO : Implement Kalman Filter here


        // bulatkan menjadi 2 digit di belakang koma
        // angleTarget = Math.Round(UnityEngine.Random.RandomRange(50f, 100f), 2, MidpointRounding.AwayFromZero);


        // integer
        angleTarget = UnityEngine.Random.RandomRange(50, 100);


        //currentAngle = Math.Round(Angle.calculate(spine, shoulder, elbow), 2, MidpointRounding.AwayFromZero);

    }

   

    private void reset()
    {
        // set anglePointer jadi 0
        player.anglePointer.transform.rotation = Quaternion.identity;

        StartCoroutine(waitFor(1.0f));

        // set angle dan text_angle jadi 0
        currentAngle = 0;
        text_angle.text = currentAngle.ToString();

        // set waktu dan bar waktu jadi 10
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
        // set line jadi panjang
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

    public void LoadScene(string sceneName)
    {
        Application.LoadLevel(sceneName);
    }
    



    private void UpdateKinectUser() {
        if(manager == null)
        {
            manager = KinectManager.Instance;
        }

        Debug.Log(manager == null);

        if(manager.IsUserDetected())
        {
            uint kinectplayer = manager.GetPlayer1ID();

            Vector3 shoulder = manager.GetJointPosition(kinectplayer, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft);
            Vector3 elbow = manager.GetJointPosition(kinectplayer, (int)KinectWrapper.NuiSkeletonPositionIndex.ElbowLeft);
            Vector3 wrist = manager.GetJointPosition(kinectplayer, (int)KinectWrapper.NuiSkeletonPositionIndex.WristLeft);
            
            Vector3 spine = manager.GetJointPosition(kinectplayer, (int)KinectWrapper.NuiSkeletonPositionIndex.Spine);

            // mengubah titik spine selurus dengan shoulder
            spine.x = shoulder.x;

            //cek apabila tangan lurus
            if (Angle.isStraight(shoulder, elbow, wrist, 20)) {
                //status_player.text = Angle.calculate(spine, shoulder, elbow).ToString();

                // bulatkan menjadi 0 digit di belakang koma
                currentAngle = Mathf.RoundToInt((float)Angle.calculate(spine, shoulder, elbow));

                // bulatkan menjadi 2 digit di belakang koma
                //currentAngle = Math.Round(Angle.calculate(spine, shoulder, elbow), 2, MidpointRounding.AwayFromZero);
                
                player.anglePointer.transform.localEulerAngles = new Vector3(0, 0, -currentAngle);

                player.line.localScale -= new Vector3(0, 0.01f, 0);

                status_player.text = currentAngle.ToString();
                text_target.text = angleTarget.ToString();

                text_angle.text = currentAngle.ToString();

            } else {
                status_player.text = "TANGAN TIDAK LURUS";
            }
            
        }
        else
        {
            status_player.text = "USER NOT DETECTED";
        }
    }

}
