using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class GameController : MonoBehaviour {

    [SerializeField]
    public Transform stepObj;
    public Transform caughtTarget;
    public Transform angleTarget_ui;

    public Transform[] target;
    public PlayerController player;
    public MoveFish moveFish;

    public GameObject fish;
    public GameObject gameover_ui;
    public GameObject time_ui;

    public float currentTime;
    public float speed;

    public bool timeOn = true;
    public bool timeChallenge;
    public bool isAngleReached;
    public bool onHook;
    public bool isTargetZero = false;
    public bool isAnimTargetRun = false;

    public Image bar_time;
    public Image ui_strike;
    public Image ui_failed;

    public Text text_time;
    public Text text_score;
    public Text text_finalScore;
    public Text text_step;
    public Text text_angle;
    public Text text_fishCaught;
    public Text text_target;
    public Text status_player;

    private int current;

    float currentAngle,
        defaultTarget = 15,
        cobaSmooth;

    int angleTarget;
    int angleTargetUI;

    int fishCaught;
    int score;
    int currentLevel;

    int[] levelPoint;

    double[] data = { 40, 45, 46, 57, 58, 54, 56, 57, 58, 60 };

    private KinectManager manager;
    private bool isAnimResetTarget;

    void Start ()
    {
        // Get kinect instance
        manager = KinectManager.Instance;

        currentLevel = 1;
        currentTime = 10;
        fishCaught = 0;
        currentAngle = 0;
        score = 0;

        cobaSmooth = Time.deltaTime * 100;

        // menentukan sudut tujuan
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

        // cek kalman filter
//        KalmanFilter kf = new KalmanFilter(data, isDebug);
//        double result = kf.process(0.1);
    }

    void Update ()
    {
        UpdateKinectUser();
        PlayerKeyboard();
        Gameplay();
    }

    private void Gameplay ()
    {
        //checkAngle();

        if (currentLevel <= 10)
        {
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
        else
        {
            text_finalScore.text = "Score: " + text_score.text;
            gameover_ui.gameObject.SetActive(true);
        }
    }

    private void PlayerKeyboard()
    {
        currentAngle = Mathf.RoundToInt(360 - player.anglePointer.transform.eulerAngles.z);
        if (currentAngle == 360) currentAngle = 0;
        text_angle.text = currentAngle.ToString();
    }

    private void PlayGameWithoutTime()
    {

        time_ui.SetActive(false);

        //angleTarget_ui.transform.Rotate(0, 0, 1 * cobaSmooth);
        //StartCoroutine(AnimResetTarget());
        checkAngle();
    }

    private void PlayGameWithTime()
    {
        time_ui.SetActive(true);

        timeRun();
        updateFishOnHook();
    }

    private void updateFishOnHook()
    {
        if (onHook)
        {
            StartCoroutine(waitForFish(3.0f));
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
                moveFish.moveToTarget = false;
                SoundManager.PlaySound("failed");
                StartCoroutine(waitFor(1f));

                reset();
            }
        }
        else if (timeOn == true && isAngleReached == true)
        {
            Debug.Log("Sudut Tercapai! Lanjut Level. Target Sekarang: " + angleTarget);

            // hentikan waktu
            timeOn = false;

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
        if (currentAngle == angleTarget)
        {
            isAngleReached = true;
            ui_strike.gameObject.SetActive(true);

            // tambah score
            addScore();

            // suara dapet
            SoundManager.PlaySound("strike");

            // animasi tarik pancingan
            pullFishingRod();

            // set back everything to 0

            StartCoroutine(AnimResetTarget());
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
        // random
        angleTarget = UnityEngine.Random.Range(50, 100);

        // set target angle
        player.SetTargetAngle(angleTarget);

        // animasi set target
        angleTargetUI = 360 - angleTarget;
        StartCoroutine(AnimSetTarget());
    }

    private void reset()
    {
        // set anglePointer jadi 0
        //player.anglePointer.transform.rotation = Quaternion.identity;

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

    private void UpdateKinectUser()
    {
        try
        {
            if (manager == null) manager = KinectManager.Instance;
            if (manager.IsUserDetected())
            {
                uint kinectplayer = manager.GetPlayer1ID();

                Vector3 shoulder = manager.GetJointPosition(kinectplayer, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft);
                Vector3 elbow = manager.GetJointPosition(kinectplayer, (int)KinectWrapper.NuiSkeletonPositionIndex.ElbowLeft);
                Vector3 wrist = manager.GetJointPosition(kinectplayer, (int)KinectWrapper.NuiSkeletonPositionIndex.WristLeft);

                Vector3 spine = manager.GetJointPosition(kinectplayer, (int)KinectWrapper.NuiSkeletonPositionIndex.Spine);

                // mengubah titik spine selurus dengan shoulder
                spine.x = shoulder.x;

                //cek apabila tangan lurus
                if (Angle.isStraight(shoulder, elbow, wrist, 20))
                {
                    //status_player.text = Angle.calculate(spine, shoulder, elbow).ToString();

                    // bulatkan menjadi 0 digit di belakang koma
                    currentAngle = Mathf.RoundToInt((float)Angle.calculate(spine, shoulder, elbow));

                    player.anglePointer.transform.localEulerAngles = new Vector3(0, 0, -currentAngle);

                    player.line.localScale -= new Vector3(0, 0.01f, 0);

                    status_player.text = currentAngle.ToString();
                    text_target.text = angleTarget.ToString();

                    text_angle.text = currentAngle.ToString();

                }
                else
                {
                    status_player.text = "TANGAN TIDAK LURUS";
                }
            }
            else
            {
                status_player.text = "USER NOT DETECTED";
            }
        } catch {
            status_player.text = "Kinect Error: Please plug the kinect device";
        }
    }

    IEnumerator AnimSetTarget()
    {
        angleTarget_ui.transform.eulerAngles = new Vector3(0, 0, 0);
        isAnimTargetRun = true;
        bool arrived = false;
        float smooth = Time.deltaTime * 100;

        while (!arrived)
        {
            angleTarget_ui.transform.Rotate(0, 0, -1 * smooth);
            if (angleTarget_ui.transform.eulerAngles.z <= angleTargetUI)
            {
                angleTarget_ui.transform.eulerAngles = new Vector3(0, 0, angleTargetUI);
                isAnimTargetRun = false;
                arrived = true;
            }
            yield return null;
        }
    }

    IEnumerator AnimResetTarget()
    {

        angleTarget_ui.transform.eulerAngles = new Vector3(0, 0, angleTargetUI);
        isAnimResetTarget = true;
        bool arrived = false;
        float smooth = Time.deltaTime * 100;
        
        while (!arrived)
        {
            angleTarget_ui.transform.Rotate(0, 0, 1);

            if (angleTarget_ui.transform.eulerAngles.z != 0)
            {
                angleTarget_ui.transform.eulerAngles = new Vector3(0, 0, 0);

                isAnimResetTarget = false;
                arrived = true;

                Debug.Log("AnimeResetTarget Executed");
            }
            yield return null;
        }

    }
}
