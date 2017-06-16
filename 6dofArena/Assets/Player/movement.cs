using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour {
    public float torque = 0.1f;
    public float force = 1.0f;
    public float xForce = 0.0f;
    public float yForce = 0.0f;
    public float zForce = 0.0f;
    public float yaw = 0.0f;
    public float pitch = 0.0f;
    public float roll = 0.0f;
    public float breakForce = 100.0f;
    public float rotBreak = 0.0f;
    public float linBreak = 0.0f;
    public bool isBreaking = false;
    public float forwardThrustMultiplier = 2;
    public float boostPower = 2;
    public bool isBoosting = true;
    public float lateralDampening = 0.1f;
    Rigidbody myRb;

    //states: braking, free
    void startBreaking()
    {
        //myRb.drag = 100;
        myRb.angularDrag = 100;
        isBreaking = true;
    }

    void endBreaking()
    {
        //myRb.drag = 0;
        myRb.angularDrag = 0;
        isBreaking = false;
    }


    // Use this for initialization
    void Start () {
        myRb = gameObject.GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        xForce = Input.GetAxis("xThrust") * force;
        yForce = Input.GetAxis("yThrust") * force;
        zForce = Input.GetAxis("zThrust") * force * forwardThrustMultiplier;

        yaw = Input.GetAxis("yaw") * torque;
        pitch = Input.GetAxis("pitch") * torque;
        roll = Input.GetAxis("roll") * torque;

        linBreak = Input.GetAxis("break") * breakForce;
        rotBreak = Input.GetAxis("break") * breakForce;
        if (Input.GetButton("angBreak")) { linBreak = 0; rotBreak = breakForce; }
        isBoosting = Input.GetButton("boost");

        //if (Input.GetButtonDown("break")) { startBreaking(); }
        //if (Input.GetButtonUp("break")) { endBreaking(); }
    }

    void FixedUpdate()
    {
        myRb.AddRelativeForce(Vector3.right * xForce);
        myRb.AddRelativeForce(Vector3.down * yForce);
        myRb.AddRelativeForce(Vector3.back * zForce);

        myRb.AddRelativeTorque(Vector3.up * yaw);
        myRb.AddRelativeTorque(Vector3.right * pitch);
        myRb.AddRelativeTorque(Vector3.forward * roll);

        myRb.drag = linBreak;
        myRb.angularDrag = rotBreak;
        if (isBoosting)
        {
            //kill lateral velocity over time while maintaining and increasing forward velocity
            //desired velocity
            Vector3 vDesired = transform.InverseTransformVector(myRb.velocity);//converts velocity to local space
            vDesired.x = 0;
            vDesired.y = 0;
            if (vDesired.z < 0) { vDesired.z = 0; }
            vDesired = transform.TransformVector(vDesired);//converts desired vec back into world space
            myRb.velocity = Vector3.Slerp(myRb.velocity, vDesired, lateralDampening);
            //if (vDesired.x < force) { myRb.AddRelativeForce()}
            myRb.AddRelativeForce(Vector3.forward * boostPower);
            myRb.angularDrag = breakForce;
        }
    }
}
