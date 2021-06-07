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
    public GameObject arrowPath;
    private GameObject currentArrow;
    public bool arrowPathDeactive;
    [HideInInspector]
    public int gameSpeed;
    [HideInInspector]
    public bool isPaused;

    //public int enCount; //til at kunne se hvor mange enemies der er i banen

    public GameManager gameManager;

    public Wave[] waves;

    public Transform[] spawnPoints;
    public Transform endPoint;

    public float timeBetweenWaves = 5f;
    public float countdown = 10f;

    public TextMeshProUGUI waveCountdownText;

    public int waveIndex = 0;

    private bool waveEnded;

    public event Action OnWavePriceLocked;
    public event Action OnWaveEnded;

    public string nameOfLevel;
    public TextMeshProUGUI nameOfLevelUI;

    public bool BuildMode { get { return EnemiesAlive <= 0; } }

    private void Start()
    {
        nameOfLevelUI.text = nameOfLevel + " (wave: " + (waveIndex) + " - " + waves.Length + ")";
        currentArrow = null;
        gameSpeed = 1;
        isPaused = false;
        EnemiesAlive = 0;
    }

    private void Update()
    {
        //enCount = EnemiesAlive; //til at kunne se hvor mange enemies der er i banen
        if (isPaused && currentArrow != null)
        {
            Destroy(currentArrow);
        }
        if (BuildMode && currentArrow == null && !arrowPathDeactive && !isPaused)
        {
            currentArrow = Instantiate(arrowPath, spawnPoints[UnityEngine.Random.Range(0, 4)].position, spawnPoints[UnityEngine.Random.Range(0, 4)].rotation);
            currentArrow.GetComponent<AIDestinationSetter>().target = endPoint;
        }

        if (EnemiesAlive < GameObject.FindGameObjectsWithTag("Enemy").Length)
        {
            Debug.LogWarning("to few enemies");
            EnemiesAlive = GameObject.FindGameObjectsWithTag("Enemy").Length;
        }

        if (EnemiesAlive > 0)
        {
            waveEnded = false;
            return;
        }
        else if (!waveEnded)
        {
            Time.timeScale = 1;
            waveEnded = true;
            OnWaveEnded?.Invoke();
        }
        if (waveIndex == waves.Length)
        {
            if(PlayerStats.Lives > 0)
            {
                gameManager.WinLevel();
            }
            this.enabled = false;
            return;
        }
        if (countdown <= 0f)
        {
            BuildManager.instance.DeselectNode();
            Time.timeScale = gameSpeed;
            waveCountdownText.text = "SPEED (" + Time.timeScale + ")";
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves;
            //PlayerStats.Money += (PlayerStats.Money - PlayerStats.Money % 100) / 5;
            return;
        }
        if (waveIndex > 0 && !isPaused)
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

        enemyImage.color = waves[waveIndex].enemy.GetComponentInChildren<SpriteRenderer>().color;

        if(waveIndex == waves.Length - 1)
        {
            enemyName.text = "Final Boss: <color=#00FF00>" + waves[waveIndex].enemy.GetComponent<Enemy>().startHealth + " HP</color>";
        }
    }
    IEnumerator SpawnWave()
    {
        nameOfLevelUI.text = nameOfLevel + " (wave: " + (waveIndex + 1) + " - " + waves.Length + ")";

        BuildManager.instance.DeselectShopItem();

        Destroy(currentArrow);

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
            if(i < wave.count - 1)
            {
                yield return new WaitForSeconds(1f / wave.rate);
            }
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
            SpeedUp();
            return;
        }
        BuildManager.instance.DeselectNode();
        countdown = 0;
        Time.timeScale = gameSpeed;
        waveCountdownText.text = "SPEED (" + Time.timeScale + ")";
    }
    public void SpeedUp()
    {
        if(gameSpeed >= 3)
        {
            gameSpeed = 1;
        }
        else
        {
            gameSpeed += 1;
        }

        Time.timeScale = gameSpeed;
        waveCountdownText.text = "SPEED (" + Time.timeScale + ")";
    }
}
