using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pathfinding;
using System;

public class WaveSpawner : MonoBehaviour
{
    public static int EnemiesAlive = 0;

    public TextMeshProUGUI enemyName;
    public Image enemyImage;

    //public int enCount; //til at kunne se hvor mange enemies der er i banen

    public GameManager gameManager;

    public Wave[] waves;

    public Transform[] spawnPoints;
    public Transform endPoint;

    public float timeBetweenWaves = 5f;
    public float countdown = 10f;

    public TextMeshProUGUI waveCountdownText;

    private int waveIndex = 0;

    private bool waveEnded;

    public event Action OnWavePriceLocked;
    public event Action OnWaveEnded;

    public string nameOfLevel;
    public TextMeshProUGUI nameOfLevelUI;

    public bool BuildMode { get { return EnemiesAlive <= 0; } }

    private void Start()
    {
        nameOfLevelUI.text = nameOfLevel + " (wave: " + waveIndex + ")";
    }

    private void Update()
    {
        //enCount = EnemiesAlive; //til at kunne se hvor mange enemies der er i banen

        if (EnemiesAlive > 0)
        {
            waveEnded = false;
            return;
        }
        else if (!waveEnded)
        {
            waveEnded = true;
            OnWaveEnded?.Invoke();
        }
        if (waveIndex == waves.Length)
        {
            gameManager.WinLevel();
            this.enabled = false;
            return;
        }
        if (countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves;
            PlayerStats.Money += (PlayerStats.Money - PlayerStats.Money % 100) / 5;
            return;
        }
        if (waveIndex > 0)
        {
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
        } else
        {
            waveCountdownText.text = "START";
        }

        enemyName.text = "Next wave: <color=#00FF00>" + waves[waveIndex].enemy.GetComponent<Enemy>().startHealth + " HP</color>";

        enemyImage.sprite = waves[waveIndex].enemy.GetComponentInChildren<SpriteRenderer>().sprite;
    }
    IEnumerator SpawnWave()
    {
        nameOfLevelUI.text = nameOfLevel + " (wave: " + (waveIndex + 1) + ")";

        PlayerStats.Rounds++;

        OnWavePriceLocked?.Invoke();

        BuildManager.instance.ClearStack();

        Wave wave = waves[waveIndex];

        EnemiesAlive += wave.count;

        enemyName.text = "Incoming: <color=#00FF00>" + wave.enemy.GetComponent<Enemy>().startHealth + " HP</color>";

        enemyImage.sprite = wave.enemy.GetComponentInChildren<SpriteRenderer>().sprite;

        for (int i = 0; i < wave.count; i++)
        {
            SpawnEnemy(wave.enemy);
            yield return new WaitForSeconds(1f / wave.rate);
        }

        waveIndex++;
    }
    void SpawnEnemy(GameObject enemy)
    {
        GameObject e = Instantiate(enemy, spawnPoints[UnityEngine.Random.Range(0, 4)].position, spawnPoints[UnityEngine.Random.Range(0, 4)].rotation);
        e.GetComponent<AIDestinationSetter>().target = endPoint;
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
