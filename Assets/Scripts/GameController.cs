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
    
    public float speed;
    
    public bool isAngleReached;
    public bool onHook;
    public bool isTargetZero = false;
    
    public Image ui_strike;
    public Image ui_failed;
    
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

    float maxAngle;

    int angleTarget;
    int angleTargetUI;

    int fishCaught;
    int score;
    int currentLevel;

    int[] levelPoint;

    double[] data = { 40, 45, 46, 57, 58, 54, 56, 57, 58, 60 };

    private KinectManager manager;
    public Api cobaApi;
    public static Status CurrentStatus = Status.START;

    void Start ()
    {
        // Get kinect instance
        manager = KinectManager.Instance;


        // Set max angle
        maxAngle = 0;

        currentLevel = 1;
        fishCaught = 0;
        currentAngle = 0;
        score = 0;


        cobaSmooth = Time.deltaTime * 100;

        // menentukan sudut tujuan
        setAngleTarget();
        

        // cek kalman filter
//        KalmanFilter kf = new KalmanFilter(data, isDebug);
//        double result = kf.process(0.1);
    }

    void Update ()
    {

        CheckMaxAngle();
        UpdateKinectUser();
        PlayerKeyboard();
        Gameplay();
    }

    private void CheckMaxAngle()
    {
        if (currentAngle > maxAngle)
        {
            maxAngle = currentAngle;
        }
        Debug.Log("Max Angle: " + maxAngle + "| Angle Target: "+ angleTarget);

    }

    private void Gameplay ()
    {
        if (currentLevel <= 10)
        {
            checkAngle();
            checkAngleReset();
            
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


    private void updateFishOnHook()
    {
        if (onHook)
        {
            StartCoroutine(waitForFish(3.0f));
        }
    }

    

    private void pullFishingRod()
    {
        player.myAnim.SetBool("pull", true);
        StartCoroutine(waitFor(1f));

        //ikan revert ke tempat awayTarget
        moveFish.transform.position = moveFish.awayTarget.position;
    }

    // proses pengecekan apakah sudut player sesuai dengan sudut targetnya
    private void checkAngle()
    {
        
        if (CurrentStatus != Status.PLAYING) return;

        if (currentAngle == angleTarget)
        {
            isAngleReached = true;
            ui_strike.gameObject.SetActive(true);

            // suara dapet
            SoundManager.PlaySound("strike");

            // TODO : kasih UI Text : "Kamu boleh turun"


            //StartCoroutine(AnimResetTarget());
        }

        if (isAngleReached && currentAngle < 20)
        {
            CurrentStatus = Status.ANIM_TARGET;

            //TODO : POST max angle to database here
			cobaApi.HitHistories("ecky","left",angleTarget.ToString(),maxAngle.ToString());
			Debug.Log ("Executed!");

            // tambah score
            addScore();

            // animasi tarik pancingan
            pullFishingRod();
            
            // set back everything to 0
            angleTargetUI = 0;


            // set maxAngle to 0
            maxAngle = 0;

            StartCoroutine(AnimResetTarget());
            
        }
    }

    // proses pengecekan apakah sudut player sesuai dengan sudut targetnya
    private void checkAngleReset()
    {
        if (CurrentStatus != Status.PLAYING_RESET) return;

        if (currentAngle < 20)
        {
            CurrentStatus = Status.ANIM_TARGET;
            setAngleTarget();
        }
    }

    private void addScore()
    {
        score = score + 1000;
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
        StartCoroutine(waitFor(1.0f));

        // set angle dan text_angle jadi 0
        currentAngle = 0;
        text_angle.text = currentAngle.ToString();

        StartCoroutine(waitFor(1.0f));

        Debug.Log("Nunggu selesai");
        currentLevel++;
    }

    IEnumerator waitFor(float duration)
    {
        yield return new WaitForSeconds(duration);
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

    // memposisikan target sesuai dengan sudut target
    IEnumerator AnimSetTarget()
    {
        bool arrived = false;
        float smooth = Time.deltaTime * 100;

        while (!arrived)
        {
            angleTarget_ui.transform.Rotate(0, 0, -1 * smooth);
            if (angleTarget_ui.transform.eulerAngles.z <= angleTargetUI)
            {
                CurrentStatus = Status.PLAYING;
                angleTarget_ui.transform.eulerAngles = new Vector3(0, 0, angleTargetUI);
                arrived = true;
            }
            yield return null;
        }
    }

    // memposisikan target ke sudut 0
    IEnumerator AnimResetTarget()
    {
        bool arrived = false;
        float smooth = Time.deltaTime * 100;

        while (!arrived)
        {
            angleTarget_ui.transform.Rotate(0, 0, 1 * smooth);
            if (angleTarget_ui.transform.eulerAngles.z > 0 && angleTarget_ui.transform.eulerAngles.z < 10)
            {
                angleTarget_ui.transform.eulerAngles = new Vector3(0, 0, 0);
                arrived = true;
                CurrentStatus = Status.PLAYING_RESET;
            }
            yield return null;
        }
    }
}