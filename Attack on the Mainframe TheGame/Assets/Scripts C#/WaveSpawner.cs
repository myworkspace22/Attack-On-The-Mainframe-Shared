using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pathfinding;
using Mirror;

public class WaveSpawner : NetworkBehaviour
{
    public static int EnemiesAlive = 0;

    //public int enCount; //til at kunne se hvor mange enemies der er i banen

    public Wave[] waves;

    public Transform[] spawnPoints;
    public Transform endPoint;

    public float timeBetweenWaves = 5f;
    public float countdown = 10f;

    public TextMeshProUGUI waveCountdownText;

    private int waveIndex = 0;

    public bool BuildMode { get { return EnemiesAlive <= 0; } }

    public PlayerStats playerStats;

    private void Update()
    {
        //enCount = EnemiesAlive; //til at kunne se hvor mange enemies der er i banen

        if (EnemiesAlive > 0)
        {
            return;
        }
        if (countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves;
            //PlayerStats.Money += (PlayerStats.Money - PlayerStats.Money % 100) / 5;
            int income = (playerStats.Money - playerStats.Money % 100) / 5;
            playerStats.changeMoney(income);
            return;
        }
        countdown -= Time.deltaTime;

        countdown = Mathf.Clamp(countdown, 0f, Mathf.Infinity);

        if (countdown > 10)
        {
            waveCountdownText.text = string.Format("START ({0:00})", countdown);
        }
        if (countdown <= 10)
        {
            waveCountdownText.text = string.Format("START ({0:0})", countdown);
        }
    }
    IEnumerator SpawnWave()
    {
        PlayerStats.Rounds++;

        Wave wave = waves[waveIndex];

        EnemiesAlive += wave.count;

        for (int i = 0; i < wave.count; i++)
        {
            CmdSpawnEnemy(wave.enemy);
            yield return new WaitForSeconds(1f / wave.rate);
        }

        waveIndex++;
        if (waveIndex == waves.Length)
        {
            Debug.Log("Level Won!");
            this.enabled = false;
        }
    }
    [Command]
    void CmdSpawnEnemy(GameObject enemy)
    {
        Debug.Log("Has auhority" + hasAuthority);
        if (!hasAuthority)
        {
            
            return;
        }
        GameObject e = Instantiate(enemy, spawnPoints[Random.Range(0, 4)].position, spawnPoints[Random.Range(0, 4)].rotation);
        e.GetComponent<AIDestinationSetter>().target = endPoint;
        NetworkServer.Spawn(e);
    }

    public void ReadyUp()
    {
        if (!BuildMode)
        {
            return;
        }
        countdown = 0;
        waveCountdownText.text = "Ready";
    }
}
