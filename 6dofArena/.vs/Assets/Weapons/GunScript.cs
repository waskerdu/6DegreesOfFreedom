using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour {
    public int ammo = 0;
    public bool useAmmo = false;
    public bool isAutomatic = false;
    public float fireDelay = 1.0f;
    public int burst = 1;
    public float cooldown = 1.0f;
    public float damage = 1.0f;
    GameObject bullet;

    class Burst
    {
        int currentBurst;
        public Burst(int burst)
        {
            currentBurst = burst;
        }
        public void Update()
        {
            currentBurst--;
            if (currentBurst == 0)
            {

            }
        }
    }

	void Start ()
    {
		
	}


	void Update ()
    {
		
	}
}
