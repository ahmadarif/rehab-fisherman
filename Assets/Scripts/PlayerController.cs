using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    private Rigidbody2D myRigidBody;
    public Transform line;
    public Transform target;
    public float jumpSpeed;
    public float hookSpeed;
    bool active = true;

    //public GameObject[] step;

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.DownArrow))
        {
            print("Space is pressed");
            line.localScale += new Vector3(0, hookSpeed, 0);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {

            line.localScale -= new Vector3(0, hookSpeed, 0);
        }
        
        
        /*if (active)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, jumpSpeed * Time.deltaTime);
        }*/
    }
}
