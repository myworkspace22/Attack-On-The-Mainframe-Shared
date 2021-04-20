using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float startSpeed = 10f;

    [HideInInspector]
    public float speed;

    public float startHealth = 100;
    private float health;
    //private float healthUIpct;

    public int worth = 50;

    //SpriteRenderer spriteRenderer;

    [Header("Unity Stuff")]
    public Image healthBar;

    private bool hasDied;
    private void Start()
    {
        speed = startSpeed;
        health = startHealth;
        hasDied = false;
        //healthUIpct = 165 / startHealth;
        //spriteRenderer = GetComponent<SpriteRenderer>();
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
        speed = startSpeed * (1f - pct);
    }
    void Die()
    {
        if (hasDied)
            return;

        hasDied = true;
        PlayerStats.Money += worth;
        Destroy(gameObject);
        WaveSpawner.EnemiesAlive--;
    }
}
