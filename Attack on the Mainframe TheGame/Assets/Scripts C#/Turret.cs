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

    [Header("Upgrades")]
    public int upgradeDamage;
    public float upgradeRange;
    public float upgradeFrenquency;
    public int upgradeLaserDoT;

    [Header("Use Bullets (defualt)")]
    public int bulletDamage;
    public float fireRate = 1f;
    private float fireCountdown = 0f;

    [Header("Special Abileties")]
    public int multiTargets;
    public float multiDelay;
    public float increseFrenquencyPct;
    public bool sniper;

    private float baseFrenquency;
    private float multiCountdown;
    private Stack<Transform> mTargets;


    [Header("Use Laser")]
    public bool useLaser = false;

    public int damageOverTime = 30;
    public float slowAmount = 0.5f;

    public LineRenderer lineRenderer;
    public ParticleSystem impactEffect;

    public float laserWidth;

    [Header("Unity Setup Fields M.I.S")]
    public string enemyTag = "Enemy";

    public Transform rotationPoint;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public SpriteRenderer towerPlatform;

    private void Start()
    {
        baseFrenquency = fireRate;
        mTargets = new Stack<Transform>();
        multiCountdown = 0;
        InvokeRepeating("UpdateTarget", 0f, 0.2f); //Update Target Delay
        BuildManager.instance.GetComponent<WaveSpawner>().OnWaveEnded += ResetRotation;
        //towerRange.transform.localScale = new Vector2(range * 2, range * 2);
    }
    private void OnDestroy()
    {
        if(BuildManager.instance != null)
        {
            BuildManager.instance.GetComponent<WaveSpawner>().OnWaveEnded -= ResetRotation;
        }
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

        fireRate = baseFrenquency;
        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
            targetEnemy = nearestEnemy.GetComponent<Enemy>();
        } else
        {
            target = null;
        }
    }

    private void FindEnemiesWithinRange()
    {
        if (target == null) { return; }

        int index = multiTargets - 1;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        mTargets.Clear();

        foreach (GameObject enemy in enemies)
        {
            if (index > 0)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy <= range && enemy.transform != target)
                {
                    mTargets.Push(enemy.transform);
                    index--;
                }
            }
            else
            {
                break;
            }
        }
    }



    private void Update()
    {
        if (sniper && lineRenderer.enabled)
        {
            Color lineColor =  lineRenderer.endColor;
            lineColor.a -= (fireRate * 2) * Time.deltaTime;
            Debug.Log("line alpha: " + lineColor.a);
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
        }

        if (target != null)
        {
            if (target.GetComponent<Enemy>().StealthMode)
            {
                target = null;
            }
        }

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

        if (mTargets.Count > 0)
        {
            while (mTargets.Peek() == null)
            {
                mTargets.Pop();
                if (mTargets.Count > 0) { break; }
            }


            if (mTargets.Count > 0)
            {
                LockOnTarget(mTargets.Peek());
                if (multiCountdown <= 0f)
                {
                    Shoot(mTargets.Pop());
                    multiCountdown = multiDelay;
                }
                multiCountdown -= Time.deltaTime;
            }
        }
        else
        {
            LockOnTarget(target);
        }
        
        

        if (useLaser)
        {
            Laser();
        }
        else
        {
            if (fireCountdown <= 0f)
            {
                if (multiTargets > 0)
                {
                    FindEnemiesWithinRange();
                    multiCountdown = multiDelay;
                }
                if (increseFrenquencyPct > 0)
                {
                    if (fireRate < baseFrenquency * 3)
                    {
                        fireRate += baseFrenquency * increseFrenquencyPct;
                    }
                    
                }
                Shoot(target);
                fireCountdown = 1f / fireRate;
            }
            fireCountdown -= Time.deltaTime;
        }
    }
    void LockOnTarget(Transform newTraget)
    {
        Vector3 dir = newTraget.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rotationPoint.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }
    void Laser()
    {
        targetEnemy.GetComponent<Enemy>().TakeDamage(damageOverTime * Time.deltaTime);
        targetEnemy.Slow(slowAmount);

        if (!lineRenderer.enabled)
        {
            lineRenderer.startWidth = laserWidth;
            lineRenderer.enabled = true;
            impactEffect.Play();
        }
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target.position);

        Vector3 dir = firePoint.position - target.position;
        impactEffect.transform.position = target.position + dir.normalized * .18f;
        impactEffect.transform.rotation = Quaternion.LookRotation(dir);
    }
    void Shoot(Transform bulletTarget)
    {

        GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.damage = bulletDamage;
            bullet.Seek(bulletTarget);
            if (sniper)
            {
                bullet.transform.position = bulletTarget.position;
                bullet.HitTarget();
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, firePoint.position);
                lineRenderer.SetPosition(1, bulletTarget.position);
                Color lineColor = lineRenderer.endColor;
                lineColor.a = 1;
                lineRenderer.startColor = lineColor;
                lineRenderer.endColor = lineColor;
            }
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, range);
    }
    private void ResetRotation()
    {
        rotationPoint.rotation = Quaternion.Euler(0, 0, 0);
    }
}
