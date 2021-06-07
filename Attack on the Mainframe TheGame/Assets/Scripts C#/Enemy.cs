using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

public class Enemy : MonoBehaviour
{
    [Header("Base Stas:")]
    public float startHealth;
    public float maxSpeed;
    public int damage;
    public int worth;
    [HideInInspector]
    public float speed;
    public float stunResistence;
    public GameObject deathEffect;

    [Header("Special Stats:")]
    public bool canFly;
    public float healAmt;
    public float slowResistence;
    //public bool slowResistence;
    public bool increaseSpeed;
    public float stealthTime;
    public int deathSpawnAmt;
    public int timedSpawnAmt;
    public float timedSpawnDelay;
    public GameObject spawn;
    public Transform[] spawnPoints;
    
    [Header("Unity Stuff")]
    public Image healthBar;
    public Transform healthTransform;
    public bool givesMoneyOnEnd;
    public GameObject spriteToRotate;

    //Privates
    
    public bool StealthMode { get; private set; }
    private float StartSpeed { get; set; }
    private float health;
    private float stunResistenceTimer;
    private Vector2 dir;
    private Color enemyBaseColor;
    private AILerp aIPath;
    private AIDestinationSetter aIDestination;
    private CameraShake shake;

    private bool slowed;
    private bool hasDied;

    private int poisonDamage;
    private float poisonTimer;

    private float stunTimer;

    private void Start()
    {
        StealthMode = false;

        stunTimer = 0;

        aIPath = GetComponent<AILerp>();
        aIDestination = GetComponent<AIDestinationSetter>();
        StartSpeed = maxSpeed;
        SetSpeed(maxSpeed);

        health = startHealth;
        hasDied = false;
        shake = GameObject.FindGameObjectWithTag("ScreenShake").GetComponent<CameraShake>();
        stunResistenceTimer = 0;

        dir = aIDestination.target.position - transform.position;
        //healthUIpct = 165 / startHealth;
        //spriteRenderer = GetComponent<SpriteRenderer>();

        poisonDamage = 0;
        poisonTimer = 0;

        if (timedSpawnAmt > 0 && timedSpawnDelay > 0)
        {
            InvokeRepeating("Spawn", timedSpawnDelay, timedSpawnDelay);
        }

        enemyBaseColor = GetComponentInChildren<SpriteRenderer>().color;

        if (canFly)
        {
            RotateTowardsEnd();
        }
    }

    private void SetSpeed(float newSpeed)
    {
        if (canFly)
        {
            aIPath.canMove = false;
            speed = newSpeed;
        }
        else
        {
            aIPath.speed = newSpeed;
        }
    }
    private void RotateTowardsEnd()
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        spriteToRotate.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    private void Update()
    {
        if (!slowed && !StealthMode)
        {
            SetSpeed(maxSpeed);
        }

        if (poisonTimer > 0)
        {
            TakeDamage(poisonDamage * Time.deltaTime);
            poisonTimer -= Time.deltaTime;
            if (poisonTimer <= 0)
            {
                SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
                Color c = sr.color;
                c = enemyBaseColor;
                sr.color = c;
            }
        }
        else if (healAmt > 0)
        {
            health = Mathf.Clamp(health + healAmt * Time.deltaTime, 0, startHealth);
            healthBar.fillAmount = health / startHealth;
        }

        if (stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0)
            {
                if (!canFly)
                {
                    aIPath.canMove = true;
                }
                SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
                Color c = sr.color;
                c = enemyBaseColor;
                sr.color = c;
            }
            return;
        }

        if (stunResistenceTimer > 0) { stunResistenceTimer -= Time.deltaTime; }

        slowed = false;

        if (StealthMode)
        {
            if(stealthTime <= 0)
            {
                SetStealtMode(false);
            }
            else
            {
                stealthTime -= Time.deltaTime;
            }
        }

        if (Vector2.Distance(transform.position, aIDestination.target.position) <= 0.25f)
        {
            EndReached();
        }
        
        if (canFly)
        {
            transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
        }
    }

    public void TakeDamage (float amount)
    {
        if(StealthMode) { return; }

        health -= amount;

        //float alpha = 90 + (healthUIpct * health);
        //Debug.Log(alpha + " color on " + gameObject);

        //Color colorTmp = spriteRenderer.color;
        //colorTmp.a = alpha / 255;
        //Debug.Log(colorTmp.a + " pct color on " + gameObject);
        //spriteRenderer.color = colorTmp;
        if (increaseSpeed)
        {
            maxSpeed = StartSpeed * ((1 - health / startHealth) * 2 + 1);
            SetSpeed(maxSpeed);
        }

        healthBar.fillAmount = health / startHealth;

        if (health <= 0)
        {
            Die();
        }

        if (stealthTime > 0 && health <= startHealth / 2)
        {
            SetStealtMode(true);
        }
    }

    public void Poison(int poisonDmg, float poisonTme)
    {
        poisonDamage = poisonDmg;
        poisonTimer = poisonTme;
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        Color color = sr.color;
        color = Color.green;
        sr.color = color;
    }

    private void SetStealtMode(bool change)
    {
        StealthMode = change;
        tag = (change)? "Untagged": "Enemy";
        float tmpSpeed = (change)? maxSpeed * 1.5f : maxSpeed;
        SetSpeed(tmpSpeed);
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        Color tmp = sr.color;
        tmp.a = (change)? 0.6f: 1f;
        sr.color = tmp;
    }

    public void Stun(float stunTime)
    {
        if(stunResistenceTimer > 0) { return; }
        aIPath.canMove = false;
        stunTimer = stunTime;
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        Color color = sr.color;
        color = Color.cyan;
        sr.color = color;
        stunResistenceTimer = stunResistence;
    }

    public void Slow (float pct)
    {
        if (StealthMode) { return; }
        pct -= slowResistence;
        SetSpeed(maxSpeed * (1f - pct));
        slowed = true;
    }
    void Die()
    {
        if (hasDied)
            return;

        if (deathSpawnAmt > 0)
        {
            Spawn(deathSpawnAmt);
        }

        GameObject effectIns = (GameObject)Instantiate(deathEffect, transform.position, transform.rotation);
        Destroy(effectIns, 2f);

        hasDied = true;
        PlayerStats.Money += worth;
        Destroy(gameObject);
        WaveSpawner.EnemiesAlive--;
    }
    private void Spawn()
    {
        if (spawnPoints.Length >= timedSpawnAmt)
        {
            for (int i = 0; i < timedSpawnAmt; i++)
            {
                GameObject spawnee = Instantiate(spawn, spawnPoints[i].position, spawnPoints[i].rotation);
                if (healthTransform != null) { spawnee.GetComponent<Enemy>().healthTransform.rotation = healthTransform.rotation; }
                spawnee.GetComponent<AIDestinationSetter>().target = aIDestination.target;
                WaveSpawner.EnemiesAlive++;
            }
        }
        else
        {
            GameObject spawnee = Instantiate(spawn, transform.position, transform.rotation);
            if (healthTransform != null) { spawnee.GetComponent<Enemy>().healthTransform.rotation = healthTransform.rotation; }
            spawnee.GetComponent<AIDestinationSetter>().target = aIDestination.target;
            WaveSpawner.EnemiesAlive++;
        }
    }
    private void Spawn(int amt)
    {
        if (spawnPoints.Length >= amt)
        {
            for (int i = 0; i < amt; i++)
            {
                GameObject spawnee = Instantiate(spawn, spawnPoints[i].position, spawnPoints[i].rotation);
                if (healthTransform != null) { spawnee.GetComponent<Enemy>().healthTransform.rotation = healthTransform.rotation; }
                spawnee.GetComponent<AIDestinationSetter>().target = aIDestination.target;
                WaveSpawner.EnemiesAlive++;
            }
        }
        else
        {
            GameObject spawnee = Instantiate(spawn, transform.position, transform.rotation);
            if (healthTransform != null) { spawnee.GetComponent<Enemy>().healthTransform.rotation = healthTransform.rotation; }
            spawnee.GetComponent<AIDestinationSetter>().target = aIDestination.target;
            WaveSpawner.EnemiesAlive++;
        }
    }

    private void EndReached()
    {
        PlayerStats.Lives -= damage;
        WaveSpawner.EnemiesAlive--;
        if (givesMoneyOnEnd)
        {
            PlayerStats.Money += worth;
        }
        shake.CamShake();
        Destroy(gameObject);
    }
}
