using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turretAi : MonoBehaviour {
    public int state = 0;//state 0: passive search, 1: tracking, 2: firing, 3: active search
    public Vector3 targetVec;
    public Vector3 mountVec = Vector3.down;
    public GameObject mount;
    public GameObject player;
    public GameObject gun;

	void Start ()
    {
        //mount.transform.LookAt(mountVec+mount.transform.position);//anchors mount
	}
	
	void Update ()
    {
        //.transform.LookAt(player.transform.position);
        //mount.transform.LookAt(player.transform);
        mount.transform.LookAt(2 * transform.position - player.transform.position);
    }

    void FixedUpdate()
    {

    }
}
