using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

public class Enemy : MonoBehaviour
{
    private AILerp aIPath;

    private CameraShake shake;

    public float startSpeed = 10f;

    public PlayerStats playerStats;

    //[HideInInspector]
    //public float speed;

    public float startHealth = 100;
    private float health;
    //private float healthUIpct;

    public int worth = 50;

    //SpriteRenderer spriteRenderer;

    [Header("Unity Stuff")]
    public Image healthBar;

    private bool slowed;
    private bool hasDied;
    private void Start()
    {
        aIPath = GetComponent<AILerp>();
        aIPath.speed = startSpeed;
        health = startHealth;
        hasDied = false;
        shake = GameObject.FindGameObjectWithTag("ScreenShake").GetComponent<CameraShake>();
        //healthUIpct = 165 / startHealth;
        //spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!slowed)
            aIPath.speed = startSpeed;

        slowed = false;

        if (Vector2.Distance(transform.position, GetComponent<AIDestinationSetter>().target.position) <= 0.25f)
        {
            EndReached();
        }
    }

    public void TakeDamage (float amount)
    {
        health -= amount;

        //float alpha = 90 + (healthUIpct * health);
        //Debug.Log(alpha + " color on " + gameObject);

        //Color colorTmp = spriteRenderer.color;
        //colorTmp.a = alpha / 255;
        //Debug.Log(colorTmp.a + " pct color on " + gameObject);
        //spriteRenderer.color = colorTmp;

        healthBar.fillAmount = health / startHealth;

        if (health <= 0)
        {
            Die();
        }
    }

    public void Slow (float pct)
    {
        aIPath.speed = startSpeed * (1f - pct);
        slowed = true;
    }

    void Die()
    {
        if (hasDied)
            return;

        hasDied = true;
        //PlayerStats.Money += worth;
        playerStats.changeMoney(worth);
        Destroy(gameObject);
        WaveSpawner.EnemiesAlive--;
    }

    private void EndReached()
    {
        //PlayerStats.Lives--;
        playerStats.takeDamage();
        WaveSpawner.EnemiesAlive--;
        shake.CamShake();
        Destroy(gameObject);
    }
}
