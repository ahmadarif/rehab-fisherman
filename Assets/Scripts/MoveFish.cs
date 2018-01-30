using UnityEngine;
using System.Collections;

public class MoveFish : MonoBehaviour {

    public Transform[] target;
    public float speed;
    private int current;

    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        //fish movement
        if (transform.position != target[current].position)
        {
            
            Vector3 pos = Vector3.MoveTowards(transform.position, target[current].position, speed * Time.deltaTime);
            GetComponent<Rigidbody2D>().MovePosition(pos);
            print("ikan harusnya gerak");
        }
        else current = (current + 1) % target.Length;
    }
}
