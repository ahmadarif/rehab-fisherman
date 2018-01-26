using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    private Rigidbody2D myRigidBody;
    public Transform target;
    public float jumpSpeed;
    bool active = true;

    //public GameObject[] step;

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        if (active)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, jumpSpeed * Time.deltaTime);
        }
    }
}
