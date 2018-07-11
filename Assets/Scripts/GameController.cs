using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class GameController : MonoBehaviour {

    [SerializeField]
    public Transform caughtTarget;
    public Transform angleTarget_ui;
    
    public PlayerController player;
    public MoveFish moveFish;

    public GameObject fish;
    public GameObject gameover_ui;
    
    public float speed;
    
    public bool isAngleReached = false;
    public bool onHook;
    public bool isTargetZero = false;
    public bool start;
    
    public Image ui_strike;
    public Image ui_failed;
    public Image ui_passit;

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
    int angleTargetAwal;
    int angleTargetUI;


    int fishCaught;
    int score;
    int currentLevel;
	int[] levelPoint;
    double[] actuals;

    string username;
	string shoulderType;
    
    private KinectManager manager;
    public Api cobaApi;
    public static Status CurrentStatus = Status.START;
    private double result;
    private bool handLifted;

    void Start ()
    {

        isAngleReached = false;

		// Set player
		username = PlayerPrefs.GetString("username");
		shoulderType = PlayerPrefs.GetString ("shoulder");

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
        SetAngleTarget();

        Debug.Log("IsAngleReached Old: " + isAngleReached);
        
    }

    void Update ()
    {

        UpdateKinectUser();
        CheckMaxAngle();
        CheckHandLifting();
        PlayerKeyboard();
        Gameplay();
    }

    private void CheckHandLifting()
    {
        if (currentAngle > 20)
        {
            handLifted = true;
        }
    }

    private void CheckMaxAngle()
    {
        if (currentAngle > maxAngle)
        {
            maxAngle = currentAngle;
        }

    }

    private void Gameplay ()
    {
        if (currentLevel <= 10)
        {
            CheckAngle();
            CheckAngleReset();
            
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

    private void PullFishingRod()
    {
        player.myAnim.SetBool("pull", true);
        StartCoroutine(WaitFor(1f));

        //ikan revert ke tempat awayTarget
        moveFish.transform.position = moveFish.awayTarget.position;
    }

    // proses pengecekan apakah sudut player sesuai dengan sudut targetnya
    private void CheckAngle()
    {
       
        if (CurrentStatus != Status.PLAYING) return;

        if (currentAngle == angleTarget)
        {
            isAngleReached = true;

            // kasih UI Text : "Can You Pass it?"
            ui_passit.gameObject.SetActive(true);

            // suara dapet
            SoundManager.PlaySound("strike");

            //StartCoroutine(AnimResetTarget());
        }

        if ((isAngleReached && currentAngle < 20) || (handLifted && currentAngle < 20))
        {

            CurrentStatus = Status.ANIM_TARGET;

            ui_strike.gameObject.SetActive(true);

            // suara dapet
            SoundManager.PlaySound("strike");
            

            // POST max angle to database here
            if (angleTarget != 0)
            {
                cobaApi.HitHistories(username, shoulderType, angleTarget.ToString(), maxAngle.ToString());
                Debug.Log("ActualData " +maxAngle+" | isAngleReached: " + isAngleReached + " | CurrentAngle: " + (currentAngle < 20) + " | HandLifted: " + handLifted + " | AngleTarget: " + angleTarget);
                // tambah score
                AddScore();

            }


            // animasi tarik pancingan
            PullFishingRod();
            
            // set back everything to 0
            angleTargetUI = 0;

            // set maxAngle to 0
            maxAngle = 0;
            isAngleReached = false;

            if (handLifted)
            {
                handLifted = false;
            }

            StartCoroutine(AnimResetTarget());
            Debug.Log("Kebawah | isAngleReached: " + isAngleReached + " | CurrentAngle: " + (currentAngle < 20) + " | HandLifted: " + handLifted + " | AngleTarget: " + angleTarget);


        }
        
    }

    // proses pengecekan apakah sudut player sesuai dengan sudut targetnya
    private void CheckAngleReset()
    {
        if (CurrentStatus != Status.PLAYING_RESET) return;

        if (currentAngle < 20)
        {
            CurrentStatus = Status.ANIM_TARGET;
            SetAngleTarget();
        }
    }

    private void AddScore()
    {
        score += 1000;

        text_score.text = score.ToString();
    }

    private void SetAngleTarget()
    {
        
        //angleTarget = UnityEngine.Random.Range(50, 100);

        // kalman filter
        SetKalmanFilter();

        // set target angle
        player.SetTargetAngle(angleTarget);

        // animasi set target
        angleTargetUI = 360 - angleTarget;
        StartCoroutine(AnimSetTarget());

    }

    IEnumerator ImportData()
    {
        cobaApi.AmbilData(shoulderType);
        actuals = new double[cobaApi.actualData.Length];
        bool isDebug = true;

        for (int i = 0; i < cobaApi.actualData.Length; i++)
        {
            actuals[i] = (double) cobaApi.actualData[i];
        }

        KalmanFilter kf = new KalmanFilter(actuals, isDebug);
        result = kf.process(0.1);
        angleTarget = (int)result;
        Debug.Log("Hasil Prediksi: " + angleTarget);

        yield return null;

    }

    public void SetKalmanFilter()
    {
        StartCoroutine(ImportData());
    }

    private void Reset()
    {
        StartCoroutine(WaitFor(1.0f));

        // set angle dan text_angle jadi 0
        currentAngle = 0;
        text_angle.text = currentAngle.ToString();

        StartCoroutine(WaitFor(1.0f));

        Debug.Log("Nunggu selesai");
        currentLevel++;
    }

    IEnumerator WaitFor(float duration)
    {
        yield return new WaitForSeconds(duration);
        // set line jadi panjang
        player.line.transform.localScale = new Vector3(1, 1f, 1);
        //isAngleReached = false;
        moveFish.moveToTarget = true;
        player.myAnim.SetBool("pull", false);
        ui_strike.gameObject.SetActive(false);
        ui_failed.gameObject.SetActive(false);
        ui_passit.gameObject.SetActive(false);
    }

    IEnumerator WaitForFish(float duration)
    {
        yield return new WaitForSeconds(duration);
        onHook = false;
    }

    
    private void UpdateKinectUser()
    {
        try
        {
            if (manager == null) manager = KinectManager.Instance;
            if (manager.IsUserDetected())
            {
				Debug.Log(shoulderType);
                uint kinectplayer = manager.GetPlayer1ID();
				Vector3 shoulder = new Vector3();
				Vector3 elbow = new Vector3();
				Vector3 wrist = new Vector3();


				if (shoulderType == "right") {
					//setShoulderRight(kinectplayer, shoulder, elbow, wrist);
					shoulder = manager.GetJointPosition(kinectplayer, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight);
					elbow = manager.GetJointPosition(kinectplayer, (int)KinectWrapper.NuiSkeletonPositionIndex.ElbowRight);
					wrist = manager.GetJointPosition(kinectplayer, (int)KinectWrapper.NuiSkeletonPositionIndex.WristRight);

				}
				else if (shoulderType == "left") {
					//setShoulderLeft(kinectplayer, shoulder, elbow, wrist);
					shoulder = manager.GetJointPosition(kinectplayer, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft);
					elbow = manager.GetJointPosition(kinectplayer, (int)KinectWrapper.NuiSkeletonPositionIndex.ElbowLeft);
					wrist = manager.GetJointPosition(kinectplayer, (int)KinectWrapper.NuiSkeletonPositionIndex.WristLeft);
					
				}
                
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
                    text_target.text = "Your Target: " + angleTarget.ToString();

                    text_angle.text = currentAngle.ToString();

                }
                else
                {
                    status_player.text = "YOUR HAND ISN'T STRAIGHT";
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

	public void SetShoulderRight(uint kinectplayer, Vector3 shoulder, Vector3 elbow, Vector3 wrist){
		shoulder = manager.GetJointPosition(kinectplayer, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight);
		elbow = manager.GetJointPosition(kinectplayer, (int)KinectWrapper.NuiSkeletonPositionIndex.ElbowRight);
		wrist = manager.GetJointPosition(kinectplayer, (int)KinectWrapper.NuiSkeletonPositionIndex.WristRight);
	}

	public void SetShoulderLeft(uint kinectplayer, Vector3 shoulder, Vector3 elbow, Vector3 wrist){
		shoulder = manager.GetJointPosition(kinectplayer, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft);
		elbow = manager.GetJointPosition(kinectplayer, (int)KinectWrapper.NuiSkeletonPositionIndex.ElbowLeft);
		wrist = manager.GetJointPosition(kinectplayer, (int)KinectWrapper.NuiSkeletonPositionIndex.WristLeft);
	}

}