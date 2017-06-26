using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour {
    public float torque = 0.1f;
    public float force = 1.0f;
    public float forwardForce = 2;
    float xForce = 0.0f;
    float yForce = 0.0f;
    float zForce = 0.0f;
    float yaw = 0.0f;
    float pitch = 0.0f;
    float roll = 0.0f;
    public float breakForce = 100.0f;
    float rotBreak = 0.0f;
    float linBreak = 0.0f;
    bool isBreaking = false;
    public float boostPower = 2;
    bool isBoosting = true;
    public float lateralDampening = 0.1f;
    public int controller = 1;
    string xThrust;
    string yThrust;
    string zThrust;
    string yawTorque;
    string pitchTorque;
    string rollTorque;
    Rigidbody myRb;

    void setControlls()
    {
        string controllerNum = controller.ToString();
        xThrust = "lx" + controllerNum;
        yThrust = "ly" + controllerNum;
    }

    void Start () {
        myRb = gameObject.GetComponent<Rigidbody>();
        setControlls();
    }


	void Update () {
        xForce = Input.GetAxis(xThrust) * force;
        yForce = Input.GetAxis(yThrust) * force;
        zForce = Input.GetAxis("zThrust") * forwardForce;

        yaw = Input.GetAxis("yaw") * torque;
        pitch = Input.GetAxis("pitch") * torque;
        roll = Input.GetAxis("roll") * torque;

        linBreak = Input.GetAxis("break") * breakForce;
        rotBreak = Input.GetAxis("break") * breakForce;
        if (Input.GetButton("angBreak")) { linBreak = 0; rotBreak = breakForce; }
        isBoosting = Input.GetButton("boost");
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
