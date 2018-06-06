using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CalibrateController : MonoBehaviour {

    string username;
    string shoulderType;

    float maxAngle;
    float currentAngle;

    int currentLevel;

    private KinectManager manager;
    public Api cobaApi;

    public Text status_player;
    public Text text_angle;


    public Transform anglePointer;

    // Use this for initialization
    void Start () {
        // Set player
        username = PlayerPrefs.GetString("username");
        shoulderType = PlayerPrefs.GetString("shoulder");

        // Get kinect instance
        manager = KinectManager.Instance;

        // Set max angle
        maxAngle = 0;

        currentLevel = 1;
    }
	
	// Update is called once per frame
	void Update () {

        UpdateKinectUser();
        CheckMaxAngle();
        PlayerKeyboard();
        Callibrate();
        Debug.Log("Max Angle: " + maxAngle);
    }

    private void CheckMaxAngle()
    {
        if (currentAngle > maxAngle)
        {
            maxAngle = currentAngle;
        }
    }

    private void Callibrate()
    {
        if (currentLevel <= 2)
        {

        }
    }

    private void PlayerKeyboard()
    {
        currentAngle = Mathf.RoundToInt(360 - anglePointer.transform.eulerAngles.z);
        if (currentAngle == 360) currentAngle = 0;
        text_angle.text = currentAngle.ToString();
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


                if (shoulderType == "right")
                {
                    //setShoulderRight(kinectplayer, shoulder, elbow, wrist);
                    shoulder = manager.GetJointPosition(kinectplayer, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight);
                    elbow = manager.GetJointPosition(kinectplayer, (int)KinectWrapper.NuiSkeletonPositionIndex.ElbowRight);
                    wrist = manager.GetJointPosition(kinectplayer, (int)KinectWrapper.NuiSkeletonPositionIndex.WristRight);

                }
                else if (shoulderType == "left")
                {
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

                    anglePointer.transform.localEulerAngles = new Vector3(0, 0, -currentAngle);

                    text_angle.text = currentAngle.ToString();
                    status_player.text = "Raise your hand as high as you can then slowly put it down";



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
        }
        catch
        {
            status_player.text = "Kinect Error: Please plug the kinect device";
        }
    }
}
