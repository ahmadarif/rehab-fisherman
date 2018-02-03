using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    private Rigidbody2D myRigidBody;
    public Transform line,
        target,
        anglePointer;
    public Animator myAnim;
    public GameController gameController;

    public float jumpSpeed;
    public float hookSpeed;
    bool active = true;

    //public GameObject[] step;

	// Use this for initialization
	void Start () {
        myAnim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.DownArrow))
        {
            print("Space is pressed");
            line.localScale += new Vector3(0, hookSpeed, 0);
            //putar turun gauge sesuai dengan angkatan tangan
            anglePointer.transform.Rotate(0, 0, hookSpeed);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {

            line.localScale -= new Vector3(0, hookSpeed, 0);
            //putar naik gauge sesuai dengan angkatan tangan
            anglePointer.transform.Rotate(0, 0, -hookSpeed);
        }
        
        
        /*if (active)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, jumpSpeed * Time.deltaTime);
        }*/
    }

    public IEnumerator Wait(float duration)
    {
        Debug.Log("Wait for: "+duration+"s");
        yield return new WaitForSeconds(duration);
    }
}
