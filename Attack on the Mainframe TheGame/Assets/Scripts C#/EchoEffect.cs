using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EchoEffect : MonoBehaviour
{
    private float timeBtwSpawns;
    public float startTimeBtwSpawns;

    public GameObject echo;

    private void Update()
    {
        if(timeBtwSpawns <= 0)
        {
            GameObject instance = (GameObject)Instantiate(echo, transform.position, transform.rotation);
            Destroy(instance, 1.5f);
            timeBtwSpawns = startTimeBtwSpawns;
        }
        else
        {
            timeBtwSpawns -= Time.deltaTime;
        }
        if(Vector2.Distance(transform.position, GetComponent<AIDestinationSetter>().target.position) <= 0.25f)
        {
            Destroy(gameObject);
        }
    }
}
