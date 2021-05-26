using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EchoEffect : MonoBehaviour
{
    private float timeBtwSpawns;
    public float startTimeBtwSpawns;
    public GameObject spawnPoint;

    public GameObject echo;

    private void Update()
    {
        if(timeBtwSpawns <= 0)
        {
            GameObject instance = (GameObject)Instantiate(echo, transform.position, transform.rotation);
            Destroy(instance, 8f);
            timeBtwSpawns = startTimeBtwSpawns;
        }
        else
        {
            timeBtwSpawns -= Time.deltaTime;
        }
        if(Vector2.Distance(transform.position, GetComponent<AIDestinationSetter>().target.position) <= 0.25f)
        {
            //Instantiate(echo, spawnPoint.transform.position, spawnPoint.transform.rotation);
            Destroy(gameObject);
        }
    }
}