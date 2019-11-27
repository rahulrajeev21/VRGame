using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject[] target;
    public int index = 0;
    public GameObject player;
    public bool isPlayerDetected = false;

    public GameObject shotSound;
    public GameObject muzzlePrefab;
    public GameObject end, start;

    public GameObject bulletHole;

    float lastFired = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        //transform.forward 1st vector
        //new Vector3(player.transform.position - transform.position).normalized; 2nd vector
        //get dot product, if the value is greater than 0.8 that is 30% and sm distance apart then u have seen the player

        //put this at the place where the enemy character detects the player:
        GetComponent<Animator>().SetBool("run", true);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = player.transform.position;
        float distanceFromPlayer = Vector3.Distance(playerPos, transform.position);
        //Debug.Log(distanceFromPlayer);
        if (isPlayerDetected == false && distanceFromPlayer < 5)
        {
            isPlayerDetected = true;
        }
        if (isPlayerDetected == false)
        {
            Vector3 tempPos = new Vector3(target[index].transform.position.x, transform.position.y, target[index].transform.position.z);
            //transform.LookAt(tempPos);
            Quaternion desiredRotation = Quaternion.LookRotation(tempPos - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime);
            float dist = Vector3.Distance(tempPos, transform.position);
            if (dist < 1)
            {
                index = (index + 1) % 4;
            }
        }

        if (isPlayerDetected && distanceFromPlayer < 5)
        {
            transform.LookAt(playerPos);
            Quaternion lookAtPlayer = Quaternion.LookRotation(playerPos - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookAtPlayer, Time.deltaTime);
            GetComponent<Animator>().SetBool("fire", true);
            if (Time.time >= lastFired)
            {
                lastFired = Time.time + 0.2f;
                shotDetection();
            }
            
        }
        else
        {
            //Quaternion lookAtPlayer = Quaternion.LookRotation(playerPos - transform.position);
            //transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime);
            GetComponent<Animator>().SetBool("fire", false);
            GetComponent<Animator>().SetBool("run", true);
        }
    }

    //void addEffects() // Adding muzzle flash, shoot sound and bullet hole on the wall
    //{
    //    Destroy(Instantiate(shotSound, transform.position, transform.rotation), 2.0f);

    //    GameObject tempMuzzle = Instantiate(muzzlePrefab, end.transform.position, end.transform.rotation);

    //    tempMuzzle.GetComponent<ParticleSystem>().Play();
    //    Destroy(tempMuzzle, 2.0f);
    //}

    void shotDetection() // Detecting the object which enemy shot 
    {
        RaycastHit rayHit;
        //update the end position here using Random.Range function
        Vector3 randomizedEnd = new Vector3(end.transform.position.x + Random.Range(-0.001f, 0.001f), end.transform.position.y, end.transform.position.z + Random.Range(-0.001f, 0.001f));
        if (Physics.Raycast(end.transform.position, (randomizedEnd - start.transform.position), out rayHit, 100.0f))
        {
            if (rayHit.transform.tag == "Player")
            {
                Debug.Log("Shot the player");
                rayHit.transform.GetComponent<GunVR>().Being_shot(100.02f);
            }
            else
            {
                Destroy(Instantiate(bulletHole, rayHit.point + rayHit.transform.up * 0.01f, rayHit.transform.rotation), 2.0f);
            }
        }

    }
}
