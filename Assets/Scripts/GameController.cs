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

	float currentAngle;
	float defaultTarget = 15;

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

    void Start()
    {
		CurrentStatus = Status.LOADING_DATA;

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

		// load data
		StartCoroutine(ImportData());
    }

    void Update ()
    {
		if (CurrentStatus == Status.LOADING_DATA)
		{
			Debug.Log("LOADING_DATA");
			return;
		}
		if (CurrentStatus == Status.LOADING_ALGORITHM)
		{
			Debug.Log("LOADING_ALGORITHM");
			return;
		}
		if (CurrentStatus == Status.LOADING_SET_TARGET)
		{
			Debug.Log("LOADING_SET_TARGET");
			return;
		}
		if (CurrentStatus == Status.LOADING_RESET_TARGET)
		{
			Debug.Log("LOADING_RESET_TARGET");
			return;
		}

		if (CurrentStatus == Status.PLAYING)
		{
			UpdateKinectUser();
			PlayerKeyboard();
			UpdateMaxAngle();

			CheckHandLifting();
			Gameplay();
		}
	}

    private void CheckHandLifting()
    {
        if (currentAngle > 20)
        {
            handLifted = true;
        }
    }

    private void UpdateMaxAngle()
    {
        if (currentAngle > maxAngle) maxAngle = currentAngle;
    }

    private void Gameplay()
    {
        if (currentLevel <= 10 + 1)
        {
            CheckAngle();
        }
		// selesai 10 gerakan
        else
        {
			text_finalScore.text = "Score: " + text_score.text;
            gameover_ui.gameObject.SetActive(true);

			CurrentStatus = Status.GAME_OVER;
			KinectWrapper.NuiShutdown();
			manager = null;
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
		// check apakah sudah mencapai target
        if (currentAngle >= angleTarget && !isAngleReached)
        {
            isAngleReached = true;

            // kasih UI Text : "Can You Pass it?"
            ui_passit.gameObject.SetActive(true);

            // suara dapet
            SoundManager.PlaySound("strike");
        }

		// tangan sudah dibawah
		// tangan sudah kurang dari 20 dari posisi target
        if ((isAngleReached && currentAngle < 20) || (handLifted && currentAngle < 20))
        {
            ui_strike.gameObject.SetActive(true);

            // suara dapet
            SoundManager.PlaySound("strike");
            
            // POST max angle to database here
            if (angleTarget != 0)
            {
				// tambah score
                AddScore();

				StartCoroutine(PostScore(username, shoulderType, angleTarget, maxAngle));
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

			ResetTarget();
            Debug.Log("Kebawah | isAngleReached: " + isAngleReached + " | CurrentAngle: " + (currentAngle < 20) + " | HandLifted: " + handLifted + " | AngleTarget: " + angleTarget);
        }
        
    }

    private void AddScore()
    {
		score += 1000;
        text_score.text = score.ToString();
    }

    IEnumerator ImportData()
    {
		// ambil data dari database
		CoroutineWithData cd = new CoroutineWithData(this, cobaApi.HttpGetHistories(username, shoulderType));
		yield return cd.coroutine;

		// parsing data ke kelas model
		HistoryRes myObject = new HistoryRes();
		JsonUtility.FromJsonOverwrite((string)cd.result, myObject);

		// simpan ke variabel array untuk perhitungan
		actuals = new double[myObject.data.Length];
		for (int i = 0; i < myObject.data.Length; i++)
		{
			actuals[i] = myObject.data[i].actual;
		}

		// hitung prediksi target selanjutnya
		CalcPrediction();
    }

	IEnumerator PostScore(string username, string shoulder, double prediction, double actual)
	{
		// simpan ke database
		CoroutineWithData cd = new CoroutineWithData(this, cobaApi.HttpPostHistory(username, shoulder, prediction, actual));
		yield return cd.coroutine;

		// tambah data baru untuk dihitung berikutnya
		// simpan ke variabel sementara
		double[] actualsTmp = new double[actuals.Length + 1];
		for (int i = 0; i < actuals.Length; i++) actualsTmp[i] = actuals[i];
		actualsTmp[actuals.Length] = actual;

		// simpan kembali ke variabel yang dihitung
		actuals = new double[actualsTmp.Length];
		for (int i = 0; i < actuals.Length; i++) actuals[i] = actualsTmp[i];
		//> tambah data baru untuk dihitung berikutnya
	}

	void CalcPrediction()
	{
		CurrentStatus = Status.LOADING_ALGORITHM;

		KalmanFilter kf = new KalmanFilter(actuals, false);
		result = kf.process(0.1);

		angleTarget = (int)result;

		if (angleTarget < 40) angleTarget = 40;
		angleTargetUI = 360 - angleTarget;

		currentLevel++;

		Debug.Log("Target = " + angleTarget);

		StartCoroutine(AnimSetTarget());
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
                    // bulatkan menjadi 0 digit di belakang koma
                    currentAngle = Mathf.RoundToInt((float)Angle.calculate(spine, shoulder, elbow));

					// posisi sudut pemain
                    player.anglePointer.transform.localEulerAngles = new Vector3(0, 0, -currentAngle);

					status_player.text = "";
                    text_target.text = "Your Target: " + angleTarget.ToString();
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
		CurrentStatus = Status.LOADING_SET_TARGET;

        bool arrived = false;
        float smooth = Time.deltaTime * 100;

        while (!arrived)
        {
            angleTarget_ui.transform.Rotate(0, 0, -1 * smooth);
            if (angleTarget_ui.transform.eulerAngles.z <= angleTargetUI)
            {
                angleTarget_ui.transform.eulerAngles = new Vector3(0, 0, angleTargetUI);
                arrived = true;
                CurrentStatus = Status.PLAYING;
				Debug.Log("Target = " + angleTarget);
            }
            yield return null;
        }
    }

	void ResetTarget()
	{
		maxAngle = 0;
		StartCoroutine(AnimResetTarget());
	}

	// memposisikan target ke sudut 0
	IEnumerator AnimResetTarget()
    {
		CurrentStatus = Status.LOADING_RESET_TARGET;

        bool arrived = false;
        float smooth = Time.deltaTime * 100;

        while (!arrived)
        {
            angleTarget_ui.transform.Rotate(0, 0, 1 * smooth);
            if (angleTarget_ui.transform.eulerAngles.z > 0 && angleTarget_ui.transform.eulerAngles.z < 10)
            {
                angleTarget_ui.transform.eulerAngles = new Vector3(0, 0, 0);
                arrived = true;

				CalcPrediction();
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