using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NewEnemyUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;

    public TextMeshProUGUI descriptionText;

    public Image enemyImage;

    public string[] newEnemyTitle;

    public string[] newEnemyDescription;

    public int[] waveIndex;

    public GameObject warning;

    private WaveSpawner waveSpawned;

    private Animator animator;

    private void Start()
    {
        waveSpawned = BuildManager.instance.GetComponent<WaveSpawner>();
        animator = GetComponent<Animator>();
        waveSpawned.OnWaveEnded += WaveEnd;
    }
    private void OnDestroy()
    {
        waveSpawned.OnWaveEnded -= WaveEnd;
    }

    private void WaveEnd()
    {
        for (int i = 0; i < waveIndex.Length; i++)
        {
            if (waveSpawned.waveIndex + 1 == waveIndex[i])
            {
                warning.SetActive(true);
                titleText.text = newEnemyTitle[i];
                descriptionText.text = newEnemyDescription[i];
                enemyImage.sprite = waveSpawned.waves[waveSpawned.waveIndex].enemy.GetComponentInChildren<SpriteRenderer>().sprite;
                animator.SetTrigger("Warning");
                waveSpawned.isPaused = true;
                break;
            }
        }
    }
    public void CloseWindow()
    {
        waveSpawned.isPaused = false;
        animator.SetTrigger("Close");
    }
}
