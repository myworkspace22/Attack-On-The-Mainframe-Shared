using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    private Transform target;
    private Enemy targetEnemy;

    [Header("General")]
    public float range = 15f;
    public bool nearestTarget = false;
    //public GameObject towerRange;

    [Header("Use Bullets (defualt)")]
    public float fireRate = 1f;
    private float fireCountdown = 0f;

    [Header("Use Laser")]
    public bool useLaser = false;

    public int damageOverTime = 30;
    public float slowAmount = 0.5f;

    public LineRenderer lineRenderer;
    public ParticleSystem impactEffect;

    [Header("Unity Setup Fields M.I.S")]
    public string enemyTag = "Enemy";

    public Transform rotationPoint;

    public GameObject bulletPrefab;
    public Transform firePoint;

    private void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
        //towerRange.transform.localScale = new Vector2(range * 2, range * 2);
    }
    void UpdateTarget()
    {
        if (target != null && transform.position != null && !nearestTarget)
        {
            if (Vector2.Distance(transform.position, target.position) <= range)
                return;
        }


        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
            targetEnemy = nearestEnemy.GetComponent<Enemy>();
        } else
        {
            target = null;
        }
    }
    private void Update()
    {
        if (target == null)
        {
            if(target == null)
            {
                if (useLaser)
                {
                    if (lineRenderer.enabled)
                    {
                        lineRenderer.enabled = false;
                        impactEffect.Stop();
                    }
                }
            }
            return;
        }

        LockOnTarget();

        if (useLaser)
        {
            Laser();
        }
        else
        {
            if (fireCountdown <= 0f)
            {
                Shoot();
                fireCountdown = 1f / fireRate;
            }
            fireCountdown -= Time.deltaTime;
        }
    }
    void LockOnTarget()
    {
        Vector3 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rotationPoint.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }
    void Laser()
    {
        targetEnemy.GetComponent<Enemy>().TakeDamage(damageOverTime * Time.deltaTime);
        targetEnemy.Slow(slowAmount);

        if (!lineRenderer.enabled)
        {
            lineRenderer.enabled = true;
            impactEffect.Play();
        }
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target.position);

        Vector3 dir = firePoint.position - target.position;
        impactEffect.transform.position = target.position + dir.normalized * .18f;
        impactEffect.transform.rotation = Quaternion.LookRotation(dir);
    }
    void Shoot()
    {
        GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.Seek(target);
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
