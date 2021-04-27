using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour
{
    private Transform target;
    private int wavepointIndex = 0;

    private Enemy enemy;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
        
        target = Waypoints.points[0];
    }
    private void Update()
    {
        Vector2 dir = target.position - transform.position;
        //transform.Translate(dir.normalized * enemy.speed * Time.deltaTime, Space.World);

        if (Vector2.Distance(transform.position, target.position) <= 0.4f)
        {
            GetNextWayPoint();
        }
        //enemy.speed = enemy.startSpeed;
    }
    void GetNextWayPoint()
    {
        if (wavepointIndex >= Waypoints.points.Length - 1)
        {
            EndPath();
            return;
        }
        wavepointIndex++;
        target = Waypoints.points[wavepointIndex];
    }
    void EndPath()
    {
        PlayerStats.Lives--;
        WaveSpawner.EnemiesAlive--;
        Destroy(gameObject);
    }
}
