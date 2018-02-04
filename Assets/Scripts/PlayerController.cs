using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    public Transform line,
        target,
        anglePointer,
        pullHand;
    public Animator myAnim;
    public Rigidbody2D myRgb;
    public GameController gameController;
    public float pullSpeed = 1;

    //public GameObject[] step;

	// Use this for initialization
	void Start () {
        myAnim = GetComponent<Animator>();
        myRgb = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.DownArrow))
        {
            print("Space is pressed");
            //line.localScale += new Vector3(0, pullSpeed, 0);
            //putar turun gauge sesuai dengan angkatan tangan
            anglePointer.transform.Rotate(0, 0, pullSpeed);
            pullHand.transform.Rotate(0, 0, -pullSpeed);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {

            //line.localScale -= new Vector3(0, pullSpeed, 0);
            //putar naik gauge sesuai dengan angkatan tangan
            anglePointer.transform.Rotate(0, 0, -pullSpeed);
            pullHand.transform.Rotate(0, 0, pullSpeed);
        }
    }

    public IEnumerator Wait(float duration)
    {
        Debug.Log("Wait for: "+duration+"s");
        yield return new WaitForSeconds(duration);
    }
}
