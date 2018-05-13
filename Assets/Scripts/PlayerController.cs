using UnityEngine;

public class PlayerController : MonoBehaviour {

    public Transform line;
    public Transform target;
    public Transform anglePointer;
    public Transform pullHand;
    public Animator myAnim;

    private int pullSpeed = 1;
    private int targetAngle;
    private int currentAngle;
    private float hooksScale;

	void Start () {
        myAnim = GetComponent<Animator>();
    }

	void Update ()
    {
        KeyboardInput();
        UpdateHooksUI();
    }

    public void SetTargetAngle(int target)
    {
        targetAngle = target;
    }

    public void SetCurrentAngle (int angle)
    {
        currentAngle = angle;
    }

    private void KeyboardInput ()
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            anglePointer.transform.Rotate(0, 0, pullSpeed);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            anglePointer.transform.Rotate(0, 0, -pullSpeed);
        }

        BoundaryFishhook();
    }

    private void BoundaryFishhook ()
    {
        if (anglePointer.transform.eulerAngles.z > 90 && anglePointer.transform.eulerAngles.z < 180)
        {
            anglePointer.transform.eulerAngles = new Vector3(0, 0, 180);
        }
        if (anglePointer.transform.eulerAngles.z > 0 && anglePointer.transform.eulerAngles.z <= 90)
        {
            anglePointer.transform.eulerAngles = new Vector3(0, 0, 0);
        }

        currentAngle = 360 - Mathf.RoundToInt(anglePointer.transform.eulerAngles.z);
        if (currentAngle == 360) currentAngle = 0;
    }

    private void UpdateHooksUI()
    {
        hooksScale = 1.0f - ((float)currentAngle / (float)targetAngle);
        line.transform.localScale = new Vector3(1, hooksScale, 0);
    }
}
