using UnityEngine;
using System.Collections;

public class MoveFish : MonoBehaviour {

	public Transform target;
	public Transform awayTarget;
    public float speed;
    public bool moveToTarget;
    private int current;

    // Use this for initialization
    void Start ()
	{
        moveToTarget = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
        //fish movement
        if (moveToTarget)
        {
            if (transform.position != target.position)
            {
                //gerak menuju posisi target
                transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
                
            }
        }
        else
        {
            if (transform.position != awayTarget.position)
            {
                //gerak menuju posisi away target
                transform.position = Vector2.MoveTowards(transform.position, awayTarget.position, speed * Time.deltaTime);
                
            }
        }   
    }
}
